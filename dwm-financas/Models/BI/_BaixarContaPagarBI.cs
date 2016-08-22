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
    public class BaixarContaPagarBI : DWMContext<ApplicationContext>, IProcess<EditarContaPagarViewModel, ApplicationContext>
    {
        #region Constructor
        public BaixarContaPagarBI() { }

        public BaixarContaPagarBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual EditarContaPagarViewModel Run(Repository value)
        {
            EditarContaPagarViewModel r = (EditarContaPagarViewModel)value;
            try
            {
                #region Recupera a Conta a Pagar (para verificar se foi informado os juros na inclusão da parcela)
                ContaPagarViewModel contaPagarViewModel = new ContaPagarViewModel()
                {
                    operacaoId = r.operacaoId
                };
                ContaPagarModel contaPagarModel = new ContaPagarModel(this.db, this.seguranca_db);
                ContaPagar contaPagar = contaPagarModel.Find(contaPagarViewModel);
                #endregion

                #region Recupera os dados da parcela
                ContaPagarParcelaViewModel contaPagarParcela = new ContaPagarParcelaViewModel()
                {
                    operacaoId = r.operacaoId,
                    parcelaId = r.parcelaId
                };
                ContaPagarParcelaCrudModel model = new ContaPagarParcelaCrudModel(this.db, this.seguranca_db);
                ContaPagarParcela entity = db.ContaPagarParcelas.SingleOrDefault(info => info.operacaoId == r.operacaoId && info.parcelaId == r.parcelaId); //model.Find(new ContaPagarParcelaViewModel() { operacaoId = 80,  parcelaId = 9 });
                contaPagarParcela = model.MapToRepository(entity);
                r.empresaId = sessaoCorrente.empresaId;
                r.dt_vencimento = contaPagarParcela.dt_vencimento;
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
                value.mensagem = Validar(r, contaPagarParcela, dt_proximo_diaUtil);
                if (value.mensagem.Code > 0)
                    throw new App_DominioException(r.mensagem);
                #endregion

                #region Atualiza os dados da parcela
                contaPagarParcela.bancoId = r.bancoId;
                contaPagarParcela.cheque_banco = r.cheque_banco;
                contaPagarParcela.cheque_agencia = r.cheque_agencia;
                contaPagarParcela.cheque_numero = r.cheque_numero;
                contaPagarParcela.ind_forma_pagamento = r.ind_forma_pagamento;
                contaPagarParcela.uri = r.uri;
                #endregion

                Evento eve = null;

                #region Fluxo Principal: Baixa de contas a pagar
                ContaPagarParcelaEventoViewModel contaPagarParcelaEventoEncargo = null;
                ContaPagarParcelaEventoViewModel contaPagarParcelaEventoDesconto = null;
                ContaPagarParcelaEventoViewModel contaPagarParcelaEventoAmortizacao = null;
                ContaPagarParcelaEventoViewModel contaPagarParcelaEventoPagtoEncargos = null;

                #region Incluir encargos (somente quando o usuário não informou na inclusão da operação o valor dos juros e mora, ou seja, esse valores estão nulos ou igual a zero)
                if ((!contaPagar.vr_jurosMora.HasValue || contaPagar.vr_jurosMora == 0) && (!contaPagar.vr_multa.HasValue || contaPagar.vr_multa == 0))
                {
                    #region Recupera o evento "1-Encargos (multa, juros, encargos, tributos, taxas, tarifa)"
                    eve = db.Eventos.Where(info => info.codigo == 1 && info.empresaId == value.empresaId).FirstOrDefault(); // 1-Encargos
                    #endregion

                    #region Incluir Evento "1-Encargos"
                    contaPagarParcelaEventoEncargo = new ContaPagarParcelaEventoViewModel()
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

                    contaPagarParcela.ContaPagarParcelaEvento = contaPagarParcelaEventoEncargo;
                    ((List<ContaPagarParcelaEventoViewModel>)contaPagarParcela.ContaPagarParcelaEventos).Add(contaPagarParcelaEventoEncargo);

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

                    contaPagarParcelaEventoPagtoEncargos = new ContaPagarParcelaEventoViewModel()
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
                        if (r.ContaPagarParcelaEvento.enquadramentoId.HasValue && r.ContaPagarParcelaEvento.enquadramentoId.Value > 0)
                        {
                            #region Mapear enquadramento para contabilidade
                            ContaPagarParcelaEventoModel eveModel = new ContaPagarParcelaEventoModel(this.db, this.seguranca_db);
                            contaPagarParcelaEventoPagtoEncargos.enquadramentoId = r.ContaPagarParcelaEvento.enquadramentoId.Value;
                            contaPagarParcelaEventoPagtoEncargos = eveModel.BeforeInsert(contaPagarParcelaEventoPagtoEncargos);
                            contaPagarParcelaEventoPagtoEncargos.Contabilidade.dt_lancamento = dt_movto_proximo_diaUtil.Value;
                            contaPagarParcelaEventoPagtoEncargos.Contabilidade.documento = contaPagarParcela.num_titulo != null && !String.IsNullOrWhiteSpace(contaPagarParcela.num_titulo) ? contaPagarParcela.num_titulo : r.cheque_numero;
                            #endregion
                        };
                        #endregion

                        #region Gerar Movimentação Bancária
                        MovtoBancarioViewModel movto2ViewModel = new MovtoBancarioViewModel()
                        {
                            empresaId = sessaoCorrente.empresaId,
                            bancoId = r.ContaPagarParcelaEvento.MovtoBancario.bancoId,
                            historicoId = r.ContaPagarParcelaEvento.MovtoBancario.historicoId,
                            complementoHist = r.ContaPagarParcelaEvento.MovtoBancario.complementoHist,
                            dt_movto = dt_movto_proximo_diaUtil.Value,
                            valor = r.ContaPagarParcelaEvento.MovtoBancario.valor,
                            tipoMovto = "D"
                        };
                        contaPagarParcelaEventoPagtoEncargos.MovtoBancario = movto2ViewModel;
                        #endregion
                    }

                    contaPagarParcela.ContaPagarParcelaEvento = contaPagarParcelaEventoPagtoEncargos;
                    ((List<ContaPagarParcelaEventoViewModel>)contaPagarParcela.ContaPagarParcelaEventos).Add(contaPagarParcelaEventoPagtoEncargos);
                }
                #endregion

                #region Incluir desconto
                if (r.vr_desconto.HasValue && r.vr_desconto.Value > 0)
                {
                    #region Recupera o evento "2-Desconto"
                    eve = db.Eventos.Where(info => info.codigo == 2 && info.empresaId == sessaoCorrente.empresaId).FirstOrDefault(); // 2-Desconto
                    #endregion

                    #region Incluir Evento "2-Desconto"
                    contaPagarParcelaEventoDesconto = new ContaPagarParcelaEventoViewModel()
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

                    contaPagarParcela.ContaPagarParcelaEvento = contaPagarParcelaEventoDesconto;
                    ((List<ContaPagarParcelaEventoViewModel>)contaPagarParcela.ContaPagarParcelaEventos).Add(contaPagarParcelaEventoDesconto);

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
                    contaPagarParcelaEventoAmortizacao = new ContaPagarParcelaEventoViewModel()
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
                        arquivo = r.ContaPagarParcelaEvento.arquivo
                    };
                    #endregion

                    #region Mapear Enquadramento para a Contabilidade
                    if (r.ContaPagarParcelaEvento.enquadramentoId.HasValue && r.ContaPagarParcelaEvento.enquadramentoId.Value > 0)
                    {
                        #region Mapear enquadramento para contabilidade
                        ContaPagarParcelaEventoModel eveModel = new ContaPagarParcelaEventoModel(this.db, this.seguranca_db);
                        contaPagarParcelaEventoAmortizacao.enquadramentoId = r.ContaPagarParcelaEvento.enquadramentoId.Value;
                        contaPagarParcelaEventoAmortizacao = eveModel.BeforeInsert(contaPagarParcelaEventoAmortizacao);
                        contaPagarParcelaEventoAmortizacao.Contabilidade.dt_lancamento = dt_movto_proximo_diaUtil.Value;
                        contaPagarParcelaEventoAmortizacao.Contabilidade.documento = contaPagarParcela.num_titulo != null && !String.IsNullOrWhiteSpace(contaPagarParcela.num_titulo) ? contaPagarParcela.num_titulo : r.cheque_numero;
                        #endregion
                    };
                    #endregion

                    #region Gerar Movimentação Bancária
                    MovtoBancarioViewModel movtoViewModel = new MovtoBancarioViewModel()
                    {
                        empresaId = sessaoCorrente.empresaId,
                        bancoId = r.ContaPagarParcelaEvento.MovtoBancario.bancoId,
                        historicoId = r.ContaPagarParcelaEvento.MovtoBancario.historicoId,
                        complementoHist = r.ContaPagarParcelaEvento.MovtoBancario.complementoHist,
                        dt_movto = dt_movto_proximo_diaUtil.Value,
                        valor = r.ContaPagarParcelaEvento.MovtoBancario.valor,
                        tipoMovto = "D"
                    };
                    contaPagarParcelaEventoAmortizacao.MovtoBancario = movtoViewModel;
                    #endregion

                    contaPagarParcela.ContaPagarParcelaEvento = contaPagarParcelaEventoAmortizacao;
                    ((List<ContaPagarParcelaEventoViewModel>)contaPagarParcela.ContaPagarParcelaEventos).Add(contaPagarParcelaEventoAmortizacao);
                }

                contaPagarParcela = model.Update(contaPagarParcela);
                if (contaPagarParcela.mensagem.Code > 0)
                    throw new Exception(contaPagarParcela.mensagem.MessageBase);

                r.vr_multa_atraso = r.vr_multa_atraso ?? 0;
                #endregion

                #region Chamar o método AfterInsert dos eventos para mover os arquivos de boleto e comprovante (e fazer a atualização dos saldos)
                ContaPagarParcelaEventoModel pevModel = new ContaPagarParcelaEventoModel(this.db, this.seguranca_db);
                foreach (ContaPagarParcelaEventoViewModel pev in contaPagarParcela.ContaPagarParcelaEventos)
                {
                    ContaPagarParcelaEventoViewModel evm = pevModel.AfterInsert(pev);
                    if (evm.mensagem.Code > 0)
                    {
                        r.mensagem = evm.mensagem;
                        throw new Exception(contaPagarParcela.mensagem.MessageBase);
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

        public IEnumerable<EditarContaPagarViewModel> List(params object[] param)
        {
            EditarContaPagarBI bi = new EditarContaPagarBI(this.db, this.seguranca_db);
            IList<EditarContaPagarViewModel> list = new List<EditarContaPagarViewModel>();
            list.Add(bi.Run(new EditarContaPagarViewModel() { operacaoId = (int)param[0], parcelaId = (int)param[1] }));
            return list;
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
        
        #region Métodos customizados
        private Validate Validar(EditarContaPagarViewModel value, ContaPagarParcelaViewModel contaPagarParcelaViewModel, DateTime dt_proximo_diaUtilPgto)
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
            if (contaPagarParcelaViewModel.vr_saldo_devedor != value.vr_saldo_devedor)
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
            if (contaPagarParcelaViewModel.vr_saldo_devedor.Value + value.vr_encargos + value.vr_multa_atraso - value.vr_desconto.Value < value.vr_baixa)
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
            if (contaPagarParcelaViewModel.dt_ultima_amortizacao.HasValue && value.dt_pagamento < contaPagarParcelaViewModel.dt_ultima_amortizacao.Value)
            {
                value.mensagem.Code = 57;
                value.mensagem.Message = MensagemPadrao.Message(57, contaPagarParcelaViewModel.dt_ultima_amortizacao.Value.ToString("dd/MM/yyyy")).ToString();
                value.mensagem.MessageBase = "Data de pagamento inválida.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            return value.mensagem;

        }
        #endregion
    }
}