using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Models;
using App_Dominio.Security;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DWM.Models.BI
{
    public abstract class OperacaoBaixarBI<EORepo, EOPERepo, ORepo, OPRepo, OPERepo, OPEModel, OPModel, OModel, OPE, OP, O, BI> : DWMContext<ApplicationContext>, IProcess<EORepo, ApplicationContext>
        where EORepo : EditarOperacaoViewModel<EOPERepo>
        where EOPERepo : EditarOperacaoParcelaEventoViewModel
        where ORepo : OperacaoViewModel<OPRepo, OPERepo>
        where OPRepo : OperacaoParcelaViewModel<OPERepo>
        where OPERepo : OperacaoParcelaEventoViewModel
        where OPEModel : OperacaoParcelaEventoModel<OPE, OPERepo>
        where OPModel : OperacaoParcelaCrudModel<OP, OPRepo, OPE, OPERepo, OPEModel>
        where OModel : OperacaoModel<O, ORepo,OP, OPRepo, OPE, OPERepo, OPModel,OPEModel>
        where OPE : OperacaoParcelaEvento
        where OP : OperacaoParcela<OPE>
        where O : Operacao<OP, OPE>
        where BI : OperacaoEditarBI<EORepo, EOPERepo>
    {
        #region Constructor
        public OperacaoBaixarBI() { }

        public OperacaoBaixarBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }

        protected EORepo getEditarOperacaoRepositoryInstance()
        {
            Type typeInstance = typeof(EORepo);
            return (EORepo)Activator.CreateInstance(typeInstance);
        }

        protected ORepo getOperacaoRepositoryInstance()
        {
            Type typeInstance = typeof(ORepo);
            return (ORepo)Activator.CreateInstance(typeInstance);
        }
        protected OModel getOperacaoModelInstance()
        {
            Type typeInstance = typeof(OModel);
            return (OModel)Activator.CreateInstance(typeInstance);
        }
        protected OPRepo getOperacaoParcelaRepositoryInstance()
        {
            Type typeInstance = typeof(OPRepo);
            return (OPRepo)Activator.CreateInstance(typeInstance);
        }
        protected OPERepo getOperacaoParcelaEventoRepositoryInstance()
        {
            Type typeInstance = typeof(OPERepo);
            return (OPERepo)Activator.CreateInstance(typeInstance);
        }
        protected OPModel getOperacaoParcelaModelInstance()
        {
            Type typeInstance = typeof(OPModel);
            return (OPModel)Activator.CreateInstance(typeInstance);
        }
        protected OPEModel getOperacaoParcelaEventoModelInstance()
        {
            Type typeInstance = typeof(OPEModel);
            return (OPEModel)Activator.CreateInstance(typeInstance);
        }
        protected BI getOperacaoEditarBIInstance()
        {
            Type typeInstance = typeof(BI);
            return (BI)Activator.CreateInstance(typeInstance);
        }
        #endregion

        #region Abstract Methods
        protected abstract string getTipoMovto();
        #endregion

        public virtual EORepo Run(Repository value)
        {
            EORepo r = (EORepo)value;
            try
            {
                #region Recupera a Operação (para verificar se foi informado os juros na inclusão da parcela)
                ORepo operacaoViewModel = getOperacaoRepositoryInstance();
                operacaoViewModel.operacaoId = r.operacaoId;
                OModel operacaoModel = getOperacaoModelInstance();
                operacaoModel.Create(this.db, this.seguranca_db);
                O operacao = operacaoModel.Find(operacaoViewModel);
                #endregion

                #region Recupera os dados da parcela
                OPRepo operacaoParcela = getOperacaoParcelaRepositoryInstance();
                operacaoParcela.operacaoId = r.operacaoId;
                operacaoParcela.parcelaId = r.parcelaId;

                OPModel model = getOperacaoParcelaModelInstance();
                model.Create(this.db, this.seguranca_db);
                OP entity = model.Find(operacaoParcela);
                operacaoParcela = model.MapToRepository(entity);
                r.empresaId = sessaoCorrente.empresaId;
                r.dt_vencimento = operacaoParcela.dt_vencimento;
                #endregion

                #region Calcula o próximo dia útil em relação à data de referência
                DateTime? dt_referencia = r.dt_pagamento;
                ObterProximoDiaUtil obterProximoDiaUtil = new ObterProximoDiaUtil(this.db, this.seguranca_db, dt_referencia);
                obterProximoDiaUtil.Run(new FeriadoViewModel());
                DateTime dt_proximo_diaUtil = obterProximoDiaUtil.dt_referencia;
                #endregion

                #region Calcula o próximo dia útil em relação à data do movimento bancário
                DateTime? dt_movto_proximo_diaUtil = null;
                ObterProximoDiaUtil obterProximoDiaUtil_dt_movto = new ObterProximoDiaUtil(this.db, this.seguranca_db, r.dt_movto);
                obterProximoDiaUtil_dt_movto.Run(new FeriadoViewModel());
                dt_movto_proximo_diaUtil = obterProximoDiaUtil_dt_movto.dt_referencia;
                #endregion

                #region Validar a Baixa
                value.mensagem = Validar(r, operacaoParcela, dt_proximo_diaUtil);
                if (value.mensagem.Code > 0)
                    throw new App_DominioException(r.mensagem);
                #endregion

                #region Atualiza os dados da parcela
                operacaoParcela.bancoId = r.bancoId;
                operacaoParcela.cheque_banco = r.cheque_banco;
                operacaoParcela.cheque_agencia = r.cheque_agencia;
                operacaoParcela.cheque_numero = r.cheque_numero;
                operacaoParcela.ind_forma_pagamento = r.ind_forma_pagamento;
                operacaoParcela.uri = r.uri;
                #endregion

                Evento eve = null;

                #region Fluxo Principal: Baixa de operação
                OPERepo operacaoParcelaEventoEncargo = null;
                OPERepo operacaoParcelaEventoDesconto = null;
                OPERepo operacaoParcelaEventoAmortizacao = null;
                OPERepo operacaoParcelaEventoPagtoEncargos = null;

                decimal _vr_encargos = (r.vr_multa_atraso ?? 0) + (r.vr_encargos ?? 0);

                #region Incluir encargos (somente quando o usuário não informou na inclusão da operação o valor dos juros e mora, ou seja, esse valores estão nulos ou igual a zero)
                if (_vr_encargos > 0 && (!operacao.vr_jurosMora.HasValue || operacao.vr_jurosMora == 0) && (!operacao.vr_multa.HasValue || operacao.vr_multa == 0))
                {
                    #region Recupera o evento "1-Encargos (multa, juros, encargos, tributos, taxas, tarifa)"
                    eve = db.Eventos.Where(info => info.codigo == 1 && info.empresaId == value.empresaId).FirstOrDefault(); // 1-Encargos
                    #endregion

                    #region Incluir Evento "1-Encargos"
                    operacaoParcelaEventoEncargo = getOperacaoParcelaEventoRepositoryInstance();

                    operacaoParcelaEventoEncargo.operacaoId = r.operacaoId;
                    operacaoParcelaEventoEncargo.parcelaId = r.parcelaId;
                    operacaoParcelaEventoEncargo.dt_evento = Funcoes.Brasilia();
                    operacaoParcelaEventoEncargo.eventoId = eve.eventoId;
                    operacaoParcelaEventoEncargo.dt_ocorrencia = dt_proximo_diaUtil;
                    operacaoParcelaEventoEncargo.valor = r.vr_encargos.Value + r.vr_multa_atraso.Value;
                    operacaoParcelaEventoEncargo.ind_operacao = eve.ind_operacao;
                    operacaoParcelaEventoEncargo.ind_estorno = "N";
                    operacaoParcelaEventoEncargo.ind_tipoEvento = eve.ind_tipoEvento;

                    operacaoParcela.OperacaoParcelaEvento = operacaoParcelaEventoEncargo;
                    ((List<OPERepo>)operacaoParcela.OperacaoParcelaEventos).Add(operacaoParcelaEventoEncargo);

                    #endregion
                }
                #endregion

                #region Incluir Evento "11-Pagamento de encargos/multa"
                

                if (_vr_encargos > 0)
                {
                    #region Recupera o evento "11-Pagamento de encargos/multa"
                    eve = db.Eventos.Where(info => info.codigo == 11 && info.empresaId == value.empresaId).FirstOrDefault();
                    #endregion

                    operacaoParcelaEventoPagtoEncargos = getOperacaoParcelaEventoRepositoryInstance();

                    operacaoParcelaEventoPagtoEncargos.operacaoId = r.operacaoId;
                    operacaoParcelaEventoPagtoEncargos.parcelaId = r.parcelaId;
                    operacaoParcelaEventoPagtoEncargos.dt_evento = Funcoes.Brasilia().AddSeconds(5);
                    operacaoParcelaEventoPagtoEncargos.eventoId = eve.eventoId;
                    operacaoParcelaEventoPagtoEncargos.dt_ocorrencia = dt_proximo_diaUtil;
                    operacaoParcelaEventoPagtoEncargos.valor = _vr_encargos;
                    operacaoParcelaEventoPagtoEncargos.ind_operacao = eve.ind_operacao;
                    operacaoParcelaEventoPagtoEncargos.ind_estorno = "N";
                    operacaoParcelaEventoPagtoEncargos.ind_tipoEvento = eve.ind_tipoEvento;
                    operacaoParcelaEventoPagtoEncargos.arquivo = null;

                    if (r.vr_baixa.Value + r.vr_desconto.Value - _vr_encargos == 0)
                    {
                        #region Mapear Enquadramento para a Contabilidade
                        if (r.OperacaoParcelaEvento.enquadramentoId.HasValue && r.OperacaoParcelaEvento.enquadramentoId.Value > 0)
                        {
                            #region Mapear enquadramento para contabilidade
                            OPEModel eveModel = getOperacaoParcelaEventoModelInstance();
                            eveModel.Create(this.db, this.seguranca_db);

                            operacaoParcelaEventoPagtoEncargos.enquadramentoId = r.OperacaoParcelaEvento.enquadramentoId.Value;
                            operacaoParcelaEventoPagtoEncargos = eveModel.BeforeInsert(operacaoParcelaEventoPagtoEncargos);
                            operacaoParcelaEventoPagtoEncargos.Contabilidade.dt_lancamento = dt_movto_proximo_diaUtil.Value;
                            operacaoParcelaEventoPagtoEncargos.Contabilidade.documento = operacaoParcela.num_titulo != null && !String.IsNullOrWhiteSpace(operacaoParcela.num_titulo) ? operacaoParcela.num_titulo : r.cheque_numero;
                            #endregion
                        };
                        #endregion

                        #region Gerar Movimentação Bancária
                        MovtoBancarioViewModel movto2ViewModel = new MovtoBancarioViewModel()
                        {
                            empresaId = sessaoCorrente.empresaId,
                            bancoId = r.OperacaoParcelaEvento.MovtoBancario.bancoId,
                            historicoId = r.OperacaoParcelaEvento.MovtoBancario.historicoId,
                            complementoHist = r.OperacaoParcelaEvento.MovtoBancario.complementoHist,
                            dt_movto = dt_movto_proximo_diaUtil.Value,
                            valor = r.OperacaoParcelaEvento.MovtoBancario.valor,
                            tipoMovto = getTipoMovto()
                        };
                        operacaoParcelaEventoPagtoEncargos.MovtoBancario = movto2ViewModel;
                        #endregion
                    }

                    operacaoParcela.OperacaoParcelaEvento = operacaoParcelaEventoPagtoEncargos;
                    ((List<OPERepo>)operacaoParcela.OperacaoParcelaEventos).Add(operacaoParcelaEventoPagtoEncargos);
                }
                #endregion

                #region Incluir desconto
                if (r.vr_desconto.HasValue && r.vr_desconto.Value > 0)
                {
                    #region Recupera o evento "2-Desconto"
                    eve = db.Eventos.Where(info => info.codigo == 2 && info.empresaId == sessaoCorrente.empresaId).FirstOrDefault(); // 2-Desconto
                    #endregion

                    #region Incluir Evento "2-Desconto"
                    operacaoParcelaEventoDesconto = getOperacaoParcelaEventoRepositoryInstance();

                    operacaoParcelaEventoDesconto.operacaoId = r.operacaoId;
                    operacaoParcelaEventoDesconto.parcelaId = r.parcelaId;
                    operacaoParcelaEventoDesconto.dt_evento = Funcoes.Brasilia().AddSeconds(10);
                    operacaoParcelaEventoDesconto.eventoId = eve.eventoId;
                    operacaoParcelaEventoDesconto.dt_ocorrencia = dt_proximo_diaUtil;
                    operacaoParcelaEventoDesconto.valor = r.vr_desconto.Value;
                    operacaoParcelaEventoDesconto.ind_operacao = eve.ind_operacao;
                    operacaoParcelaEventoDesconto.ind_estorno = "N";
                    operacaoParcelaEventoDesconto.ind_tipoEvento = eve.ind_tipoEvento;

                    operacaoParcela.OperacaoParcelaEvento = operacaoParcelaEventoDesconto;
                    ((List<OPERepo>)operacaoParcela.OperacaoParcelaEventos).Add(operacaoParcelaEventoDesconto);

                    #endregion
                }
                #endregion

                #region verifica se o título foi liquidado
                r.ind_baixa = null;
                decimal _valor_para_liquidacao = entity.vr_saldo_devedor.Value;

                if (r.vr_baixa + r.vr_desconto >= _valor_para_liquidacao)
                    r.ind_baixa = "4";
                #endregion

                if (r.vr_baixa.Value + r.vr_desconto.Value - _vr_encargos > 0 || r.ind_baixa == "4")
                {
                    if (r.ind_baixa == "4")
                    {
                        #region Baixa por motivo de liquidação
                        #region Recupera o evento "4-Baixa por motivo de liquidação"
                        eve = db.Eventos.Where(info => info.codigo == 4 && info.empresaId == sessaoCorrente.empresaId).FirstOrDefault(); // 4-Baixa por motivo de liquidação
                        #endregion
                        #endregion
                    }
                    else
                    {
                        #region Amortização da parcela
                        #region Recupera o evento "3-Amortização"
                        eve = db.Eventos.Where(info => info.codigo == 3 && info.empresaId == sessaoCorrente.empresaId).FirstOrDefault(); // 3-Amortização
                        #endregion
                        #endregion
                    }

                    #region Incluir Evento de Baixa/Amortização (pagto parcial)
                    operacaoParcelaEventoAmortizacao = getOperacaoParcelaEventoRepositoryInstance();

                    operacaoParcelaEventoAmortizacao.operacaoId = r.operacaoId;
                    operacaoParcelaEventoAmortizacao.parcelaId = r.parcelaId;
                    operacaoParcelaEventoAmortizacao.dt_evento = Funcoes.Brasilia().AddSeconds(15);
                    operacaoParcelaEventoAmortizacao.eventoId = eve.eventoId;
                    operacaoParcelaEventoAmortizacao.dt_ocorrencia = dt_proximo_diaUtil;
                    operacaoParcelaEventoAmortizacao.valor = r.vr_baixa.Value + r.vr_desconto.Value - _vr_encargos;
                    operacaoParcelaEventoAmortizacao.ind_operacao = eve.ind_operacao;
                    operacaoParcelaEventoAmortizacao.ind_estorno = "N";
                    operacaoParcelaEventoAmortizacao.ind_tipoEvento = eve.ind_tipoEvento;
                    operacaoParcelaEventoAmortizacao.arquivo = r.OperacaoParcelaEvento.arquivo;
                    #endregion

                    #region Mapear Enquadramento para a Contabilidade
                    if (r.OperacaoParcelaEvento.enquadramentoId.HasValue && r.OperacaoParcelaEvento.enquadramentoId.Value > 0)
                    {
                        #region Mapear enquadramento para contabilidade
                        OPEModel eveModel = getOperacaoParcelaEventoModelInstance();
                        eveModel.Create(this.db, this.seguranca_db);

                        operacaoParcelaEventoAmortizacao.enquadramentoId = r.OperacaoParcelaEvento.enquadramentoId.Value;
                        operacaoParcelaEventoAmortizacao = eveModel.BeforeInsert(operacaoParcelaEventoAmortizacao);
                        operacaoParcelaEventoAmortizacao.Contabilidade.dt_lancamento = dt_movto_proximo_diaUtil.Value;
                        operacaoParcelaEventoAmortizacao.Contabilidade.documento = operacaoParcela.num_titulo != null && !String.IsNullOrWhiteSpace(operacaoParcela.num_titulo) ? operacaoParcela.num_titulo : r.cheque_numero;
                        #endregion
                    };
                    #endregion

                    #region Gerar Movimentação Bancária
                    MovtoBancarioViewModel movtoViewModel = new MovtoBancarioViewModel()
                    {
                        empresaId = sessaoCorrente.empresaId,
                        bancoId = r.OperacaoParcelaEvento.MovtoBancario.bancoId,
                        historicoId = r.OperacaoParcelaEvento.MovtoBancario.historicoId,
                        complementoHist = r.OperacaoParcelaEvento.MovtoBancario.complementoHist,
                        dt_movto = dt_movto_proximo_diaUtil.Value,
                        valor = r.OperacaoParcelaEvento.MovtoBancario.valor,
                        tipoMovto = getTipoMovto()
                    };
                    operacaoParcelaEventoAmortizacao.MovtoBancario = movtoViewModel;
                    #endregion

                    operacaoParcela.OperacaoParcelaEvento = operacaoParcelaEventoAmortizacao;
                    ((List<OPERepo>)operacaoParcela.OperacaoParcelaEventos).Add(operacaoParcelaEventoAmortizacao);
                }

                operacaoParcela = model.Update(operacaoParcela);
                if (operacaoParcela.mensagem.Code > 0)
                    throw new Exception(operacaoParcela.mensagem.MessageBase);

                r.vr_multa_atraso = r.vr_multa_atraso ?? 0;
                #endregion

                #region Chamar o método AfterInsert dos eventos para mover os arquivos de boleto e comprovante (e fazer a atualização dos saldos)
                OPEModel pevModel = getOperacaoParcelaEventoModelInstance();
                pevModel.Create(this.db, this.seguranca_db);
                foreach (OPERepo pev in operacaoParcela.OperacaoParcelaEventos)
                {
                    OPERepo evm = pevModel.AfterInsert(pev);
                    if (evm.mensagem.Code > 0)
                    {
                        r.mensagem = evm.mensagem;
                        throw new Exception(operacaoParcela.mensagem.MessageBase);
                    }
                }
                #endregion

                r.mensagem = new Validate() { Code = 0, Message = "Baixa realizada com sucesso" };
            }
            catch (App_DominioException ex)
            {
                r.mensagem = ex.Result;

                if (ex.InnerException != null)
                    r.mensagem.MessageBase = new App_DominioException(ex.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                else
                    r.mensagem.MessageBase = new App_DominioException(ex.Result.Message, GetType().FullName).Message;
            }
            catch (Exception ex)
            {
                r.mensagem = new Validate() { Code = 999, MessageBase = ex.Message, Message = "Erro de processamento da operação." };

                if (ex.InnerException != null)
                    r.mensagem.MessageBase = new App_DominioException(ex.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                else
                    r.mensagem.MessageBase = new App_DominioException(ex.Message, GetType().FullName).Message;
            }

            return r;
        }

        public IEnumerable<EORepo> List(params object[] param)
        {
            BI bi = getOperacaoEditarBIInstance();
            bi.Create(this.db, this.seguranca_db);
            IList<EORepo> list = new List<EORepo>();
            EORepo editarOperacaoViewModel = getEditarOperacaoRepositoryInstance();
            editarOperacaoViewModel.operacaoId = (int)param[0];
            editarOperacaoViewModel.parcelaId = (int)param[1];

            list.Add(bi.Run(editarOperacaoViewModel));
            return list;
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }

        #region Métodos customizados
        private Validate Validar(EORepo value, OPRepo operacaoParcelaViewModel, DateTime dt_proximo_diaUtilPgto)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };

            if (value.empresaId == 0)
            {
                value.mensagem.Code = 35;
                value.mensagem.MessageBase = MensagemPadrao.Message(35).ToString();
                value.mensagem.Message = "Sua sessão expirou. Faça um novo login no sistema";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            // Validar banco
            if (!value.bancoId.HasValue || value.bancoId == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Banco").ToString();
                value.mensagem.MessageBase = "Campo obrigatório: Banco";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            // Validar dt_pagamento
            if (!value.dt_pagamento.HasValue)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Dt.Pagamento").ToString();
                value.mensagem.MessageBase = "Campo obrigatório: Dt.Pagamento";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            // Validar Dt.Movto
            if (!value.dt_movto.HasValue)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Dt.Movto").ToString();
                value.mensagem.MessageBase = "Campo obrigatório: Dt.Movto";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            // Data do movimento não pode ser menor que a data do pagamento
            if (value.dt_movto.Value < value.dt_pagamento.Value)
            {
                value.mensagem.Code = 12;
                value.mensagem.Message = MensagemPadrao.Message(12, "Dt.Movto", "Dt.Pagamento").ToString();
                value.mensagem.MessageBase = "Data do pagamento e Dt.Movimento bancário inconsistentes";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            // Verifica se o saldo devedor não foi adulterado pelo usuário (F12 do browser)
            if (operacaoParcelaViewModel.vr_saldo_devedor != value.vr_saldo_devedor)
            {
                value.mensagem.Code = 4;
                value.mensagem.Message = MensagemPadrao.Message(4, "Saldo Devedor", value.vr_saldo_devedor.Value.ToString("###,###,###,##0.00")).ToString();
                value.mensagem.MessageBase = "Saldo devedor informado diverge do valor registrado no banco de dados";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            // Validar o valor pago
            if (value.vr_baixa.Value <= 0)
            {
                value.mensagem.Code = 47;
                value.mensagem.Message = MensagemPadrao.Message(47, "Valor Pago").ToString();
                value.mensagem.MessageBase = "Valor pago inválido";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            // Verifica se o valor pago é maior que o saldo devedor acrescido de encargos e diminuído do desconto
            if (operacaoParcelaViewModel.vr_saldo_devedor.Value + value.vr_encargos + value.vr_multa_atraso - value.vr_desconto.Value < value.vr_baixa)
            {
                value.mensagem.Code = 55;
                value.mensagem.Message = MensagemPadrao.Message(55).ToString();
                value.mensagem.MessageBase = "Valor do pagamento deve ser menor ou igual que o valor para liquidação.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            // Verifica se a parcela está atrasada. Se estiver no vencimento não pode preencher os valores de juros e mora

            #region Calcula o próximo dia útil em relação à data de vencimento
            ObterProximoDiaUtil obterProximoDiaUtil = new ObterProximoDiaUtil(this.db, this.seguranca_db, value.dt_vencimento);
            obterProximoDiaUtil.Run(new FeriadoViewModel());
            DateTime dt_proximo_diaUtilVcto = obterProximoDiaUtil.dt_referencia;
            #endregion

            if (dt_proximo_diaUtilPgto <= dt_proximo_diaUtilVcto &&
                ((value.vr_multa_atraso.HasValue && value.vr_multa_atraso.Value > 0) || (value.vr_encargos.HasValue && value.vr_encargos.Value > 0)))
            {
                value.mensagem.Code = 56;
                value.mensagem.Message = MensagemPadrao.Message(56).ToString();
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            // Verifica se a data do pagamento é maior que o próximo dia útil depois de hoje
            #region Calcula o próximo dia útil em relação à data atual
            obterProximoDiaUtil = new ObterProximoDiaUtil(this.db, this.seguranca_db, Funcoes.Brasilia().Date);
            obterProximoDiaUtil.Run(new FeriadoViewModel());
            DateTime dt_proximo_diaUtilBrasilia = obterProximoDiaUtil.dt_referencia;
            #endregion

            if (value.dt_pagamento > dt_proximo_diaUtilBrasilia)
            {
                value.mensagem.Code = 9;
                value.mensagem.Message = MensagemPadrao.Message(9, "Dt.Pagamento").ToString();
                value.mensagem.MessageBase = "Data de pagamento inválida.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            // Verifica se a data do pagamento é menor que a data da última amortização
            if (operacaoParcelaViewModel.dt_ultima_amortizacao.HasValue && value.dt_pagamento < operacaoParcelaViewModel.dt_ultima_amortizacao.Value)
            {
                value.mensagem.Code = 57;
                value.mensagem.Message = MensagemPadrao.Message(57, operacaoParcelaViewModel.dt_ultima_amortizacao.Value.ToString("dd/MM/yyyy")).ToString();
                value.mensagem.MessageBase = "Data de pagamento inválida.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            return value.mensagem;

        }
        #endregion

    }
}