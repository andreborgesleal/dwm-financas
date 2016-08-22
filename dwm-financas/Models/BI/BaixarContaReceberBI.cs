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
    public class BaixarContaReceberBI : DWMContext<ApplicationContext>, IProcess<EditarContaReceberViewModel, ApplicationContext>
    {
        #region Constructor
        public BaixarContaReceberBI() { }

        public BaixarContaReceberBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual EditarContaReceberViewModel Run(Repository value)
        {
            EditarContaReceberViewModel r = (EditarContaReceberViewModel)value;
            try
            {
                #region Recupera a Conta a Receber (para verificar se foi informado os juros na inclusão da parcela)
                ContaReceberViewModel contaReceberViewModel = new ContaReceberViewModel()
                {
                    operacaoId = r.operacaoId
                };
                ContaReceberModel contaReceberModel = new ContaReceberModel(this.db, this.seguranca_db);
                ContaReceber contaReceber = contaReceberModel.Find(contaReceberViewModel);
                #endregion

                #region Recupera os dados da parcela
                ContaReceberParcelaViewModel contaReceberParcela = new ContaReceberParcelaViewModel()
                {
                    operacaoId = r.operacaoId,
                    parcelaId = r.parcelaId
                };
                ContaReceberParcelaCrudModel model = new ContaReceberParcelaCrudModel(this.db, this.seguranca_db);
                ContaReceberParcela entity = db.ContaReceberParcelas.SingleOrDefault(info => info.operacaoId == r.operacaoId && info.parcelaId == r.parcelaId); //model.Find(new ContaReceberParcelaViewModel() { operacaoId = 80,  parcelaId = 9 });
                contaReceberParcela = model.MapToRepository(entity);
                r.empresaId = sessaoCorrente.empresaId;
                r.dt_vencimento = contaReceberParcela.dt_vencimento;
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
                value.mensagem = Validar(r, contaReceberParcela, dt_proximo_diaUtil);
                if (value.mensagem.Code > 0)
                    throw new App_DominioException(r.mensagem);
                #endregion

                #region Atualiza os dados da parcela
                contaReceberParcela.bancoId = r.bancoId;
                contaReceberParcela.cheque_banco = r.cheque_banco;
                contaReceberParcela.cheque_agencia = r.cheque_agencia;
                contaReceberParcela.cheque_numero = r.cheque_numero;
                contaReceberParcela.ind_forma_pagamento = r.ind_forma_pagamento;
                contaReceberParcela.uri = r.uri;
                #endregion

                Evento eve = null;

                #region Fluxo Principal: Baixa de contas a receber
                ContaReceberParcelaEventoViewModel contaReceberParcelaEventoEncargo = null;
                ContaReceberParcelaEventoViewModel contaReceberParcelaEventoDesconto = null;
                ContaReceberParcelaEventoViewModel contaReceberParcelaEventoAmortizacao = null;
                ContaReceberParcelaEventoViewModel contaReceberParcelaEventoPagtoEncargos = null;

                #region Incluir encargos (somente quando o usuário não informou na inclusão da operação o valor dos juros e mora, ou seja, esse valores estão nulos ou igual a zero)
                if ((!contaReceber.vr_jurosMora.HasValue || contaReceber.vr_jurosMora == 0) && (!contaReceber.vr_multa.HasValue || contaReceber.vr_multa == 0))
                {
                    #region Recupera o evento "1-Encargos (multa, juros, encargos, tributos, taxas, tarifa)"
                    eve = db.Eventos.Where(info => info.codigo == 1 && info.empresaId == value.empresaId).FirstOrDefault(); // 1-Encargos
                    #endregion

                    #region Incluir Evento "1-Encargos"
                    contaReceberParcelaEventoEncargo = new ContaReceberParcelaEventoViewModel()
                    {
                        operacaoId = r.operacaoId,
                        parcelaId = r.parcelaId,
                        dt_evento = Funcoes.Brasilia(),
                        eventoId = eve.eventoId,
                        dt_ocorrencia = dt_proximo_diaUtil,
                        valor = r.vr_encargos.Value + r.vr_multa_atraso.Value,
                        ind_operacao = eve.ind_operacao,
                        ind_estorno = "N",
                        ind_tipoEvento = eve.ind_tipoEvento
                    };

                    contaReceberParcela.ContaReceberParcelaEvento = contaReceberParcelaEventoEncargo;
                    ((List<ContaReceberParcelaEventoViewModel>)contaReceberParcela.ContaReceberParcelaEventos).Add(contaReceberParcelaEventoEncargo);

                    #endregion
                }
                #endregion

                #region Incluir Evento "11-Pagamento de encargos/multa"
                decimal _vr_encargos = (r.vr_multa_atraso ?? 0) + (r.vr_encargos ?? 0);

                if (_vr_encargos > 0)
                {
                    #region Recupera o evento "11-Pagamento de encargos/multa"
                    eve = db.Eventos.Where(info => info.codigo == 11 && info.empresaId == value.empresaId).FirstOrDefault();
                    #endregion

                    contaReceberParcelaEventoPagtoEncargos = new ContaReceberParcelaEventoViewModel()
                    {
                        operacaoId = r.operacaoId,
                        parcelaId = r.parcelaId,
                        dt_evento = Funcoes.Brasilia().AddSeconds(5),
                        eventoId = eve.eventoId,
                        dt_ocorrencia = dt_proximo_diaUtil,
                        valor = _vr_encargos,
                        ind_operacao = eve.ind_operacao,
                        ind_estorno = "N",
                        ind_tipoEvento = eve.ind_tipoEvento,
                        arquivo = null
                    };

                    if (r.vr_baixa.Value + r.vr_desconto.Value - _vr_encargos == 0)
                    {
                        #region Mapear Enquadramento para a Contabilidade
                        if (r.ContaReceberParcelaEvento.enquadramentoId.HasValue && r.ContaReceberParcelaEvento.enquadramentoId.Value > 0)
                        {
                            #region Mapear enquadramento para contabilidade
                            ContaReceberParcelaEventoModel eveModel = new ContaReceberParcelaEventoModel(this.db, this.seguranca_db);
                            contaReceberParcelaEventoPagtoEncargos.enquadramentoId = r.ContaReceberParcelaEvento.enquadramentoId.Value;
                            contaReceberParcelaEventoPagtoEncargos = eveModel.BeforeInsert(contaReceberParcelaEventoPagtoEncargos);
                            contaReceberParcelaEventoPagtoEncargos.Contabilidade.dt_lancamento = dt_movto_proximo_diaUtil.Value;
                            contaReceberParcelaEventoPagtoEncargos.Contabilidade.documento = contaReceberParcela.num_titulo != null && !String.IsNullOrWhiteSpace(contaReceberParcela.num_titulo) ? contaReceberParcela.num_titulo : r.cheque_numero;
                            #endregion
                        };
                        #endregion

                        #region Gerar Movimentação Bancária
                        MovtoBancarioViewModel movto2ViewModel = new MovtoBancarioViewModel()
                        {
                            empresaId = sessaoCorrente.empresaId,
                            bancoId = r.ContaReceberParcelaEvento.MovtoBancario.bancoId,
                            historicoId = r.ContaReceberParcelaEvento.MovtoBancario.historicoId,
                            complementoHist = r.ContaReceberParcelaEvento.MovtoBancario.complementoHist,
                            dt_movto = dt_movto_proximo_diaUtil.Value,
                            valor = r.ContaReceberParcelaEvento.MovtoBancario.valor,
                            tipoMovto = "D"
                        };
                        contaReceberParcelaEventoPagtoEncargos.MovtoBancario = movto2ViewModel;
                        #endregion
                    }

                    contaReceberParcela.ContaReceberParcelaEvento = contaReceberParcelaEventoPagtoEncargos;
                    ((List<ContaReceberParcelaEventoViewModel>)contaReceberParcela.ContaReceberParcelaEventos).Add(contaReceberParcelaEventoPagtoEncargos);
                }
                #endregion

                #region Incluir desconto
                if (r.vr_desconto.HasValue && r.vr_desconto.Value > 0)
                {
                    #region Recupera o evento "2-Desconto"
                    eve = db.Eventos.Where(info => info.codigo == 2 && info.empresaId == sessaoCorrente.empresaId).FirstOrDefault(); // 2-Desconto
                    #endregion

                    #region Incluir Evento "2-Desconto"
                    contaReceberParcelaEventoDesconto = new ContaReceberParcelaEventoViewModel()
                    {
                        operacaoId = r.operacaoId,
                        parcelaId = r.parcelaId,
                        dt_evento = Funcoes.Brasilia().AddSeconds(10),
                        eventoId = eve.eventoId,
                        dt_ocorrencia = dt_proximo_diaUtil,
                        valor = r.vr_desconto.Value,
                        ind_operacao = eve.ind_operacao,
                        ind_estorno = "N",
                        ind_tipoEvento = eve.ind_tipoEvento
                    };

                    contaReceberParcela.ContaReceberParcelaEvento = contaReceberParcelaEventoDesconto;
                    ((List<ContaReceberParcelaEventoViewModel>)contaReceberParcela.ContaReceberParcelaEventos).Add(contaReceberParcelaEventoDesconto);

                    #endregion
                }
                #endregion

                #region verifica se o título foi liquidado
                r.ind_baixa = null;
                decimal _valor_para_liquidacao = entity.vr_saldo_devedor.Value;

                if (r.vr_baixa + r.vr_desconto == _valor_para_liquidacao)
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
                    contaReceberParcelaEventoAmortizacao = new ContaReceberParcelaEventoViewModel()
                    {
                        operacaoId = r.operacaoId,
                        parcelaId = r.parcelaId,
                        dt_evento = Funcoes.Brasilia().AddSeconds(15),
                        eventoId = eve.eventoId,
                        dt_ocorrencia = dt_proximo_diaUtil,
                        valor = r.vr_baixa.Value + r.vr_desconto.Value - _vr_encargos,
                        ind_operacao = eve.ind_operacao,
                        ind_estorno = "N",
                        ind_tipoEvento = eve.ind_tipoEvento,
                        arquivo = r.ContaReceberParcelaEvento.arquivo
                    };
                    #endregion

                    #region Mapear Enquadramento para a Contabilidade
                    if (r.ContaReceberParcelaEvento.enquadramentoId.HasValue && r.ContaReceberParcelaEvento.enquadramentoId.Value > 0)
                    {
                        #region Mapear enquadramento para contabilidade
                        ContaReceberParcelaEventoModel eveModel = new ContaReceberParcelaEventoModel(this.db, this.seguranca_db);
                        contaReceberParcelaEventoAmortizacao.enquadramentoId = r.ContaReceberParcelaEvento.enquadramentoId.Value;
                        contaReceberParcelaEventoAmortizacao = eveModel.BeforeInsert(contaReceberParcelaEventoAmortizacao);
                        contaReceberParcelaEventoAmortizacao.Contabilidade.dt_lancamento = dt_movto_proximo_diaUtil.Value;
                        contaReceberParcelaEventoAmortizacao.Contabilidade.documento = contaReceberParcela.num_titulo != null && !String.IsNullOrWhiteSpace(contaReceberParcela.num_titulo) ? contaReceberParcela.num_titulo : r.cheque_numero;
                        #endregion
                    };
                    #endregion

                    #region Gerar Movimentação Bancária
                    MovtoBancarioViewModel movtoViewModel = new MovtoBancarioViewModel()
                    {
                        empresaId = sessaoCorrente.empresaId,
                        bancoId = r.ContaReceberParcelaEvento.MovtoBancario.bancoId,
                        historicoId = r.ContaReceberParcelaEvento.MovtoBancario.historicoId,
                        complementoHist = r.ContaReceberParcelaEvento.MovtoBancario.complementoHist,
                        dt_movto = dt_movto_proximo_diaUtil.Value,
                        valor = r.ContaReceberParcelaEvento.MovtoBancario.valor,
                        tipoMovto = "C"
                    };
                    contaReceberParcelaEventoAmortizacao.MovtoBancario = movtoViewModel;
                    #endregion

                    contaReceberParcela.ContaReceberParcelaEvento = contaReceberParcelaEventoAmortizacao;
                    ((List<ContaReceberParcelaEventoViewModel>)contaReceberParcela.ContaReceberParcelaEventos).Add(contaReceberParcelaEventoAmortizacao);
                }

                contaReceberParcela = model.Update(contaReceberParcela);
                if (contaReceberParcela.mensagem.Code > 0)
                    throw new Exception(contaReceberParcela.mensagem.MessageBase);

                r.vr_multa_atraso = r.vr_multa_atraso ?? 0;
                #endregion

                #region Chamar o método AfterInsert dos eventos para mover os arquivos de boleto e comprovante (e fazer a atualização dos saldos)
                ContaReceberParcelaEventoModel pevModel = new ContaReceberParcelaEventoModel(this.db, this.seguranca_db);
                foreach (ContaReceberParcelaEventoViewModel pev in contaReceberParcela.ContaReceberParcelaEventos)
                {
                    ContaReceberParcelaEventoViewModel evm = pevModel.AfterInsert(pev);
                    if (evm.mensagem.Code > 0)
                    {
                        r.mensagem = evm.mensagem;
                        throw new Exception(contaReceberParcela.mensagem.MessageBase);
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

        public IEnumerable<EditarContaReceberViewModel> List(params object[] param)
        {
            ContaReceberEditarBI bi = new ContaReceberEditarBI(this.db, this.seguranca_db);
            IList<EditarContaReceberViewModel> list = new List<EditarContaReceberViewModel>();
            list.Add(bi.Run(new EditarContaReceberViewModel() { operacaoId = (int)param[0], parcelaId = (int)param[1] }));
            return list;
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }

        #region Métodos customizados
        private Validate Validar(EditarContaReceberViewModel value, ContaReceberParcelaViewModel contaReceberParcelaViewModel, DateTime dt_proximo_diaUtilPgto)
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
            if (contaReceberParcelaViewModel.vr_saldo_devedor != value.vr_saldo_devedor)
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
            if (contaReceberParcelaViewModel.vr_saldo_devedor.Value + value.vr_encargos + value.vr_multa_atraso - value.vr_desconto.Value < value.vr_baixa)
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
            if (contaReceberParcelaViewModel.dt_ultima_amortizacao.HasValue && value.dt_pagamento < contaReceberParcelaViewModel.dt_ultima_amortizacao.Value)
            {
                value.mensagem.Code = 57;
                value.mensagem.Message = MensagemPadrao.Message(57, contaReceberParcelaViewModel.dt_ultima_amortizacao.Value.ToString("dd/MM/yyyy")).ToString();
                value.mensagem.MessageBase = "Data de pagamento inválida.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            return value.mensagem;

        }
        #endregion
    }
}