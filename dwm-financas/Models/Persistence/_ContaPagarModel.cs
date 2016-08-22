using System;
using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Models;
using App_Dominio.Security;
using DWM.Models.BI;

namespace DWM.Models.Persistence
{
    public class _ContaPagarModel : CrudModel<ContaPagar, ContaPagarViewModel, ApplicationContext>
    {
        #region Constructor
        public _ContaPagarModel() { }
        public _ContaPagarModel(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe CrudModel
        public override ContaPagarViewModel BeforeInsert(ContaPagarViewModel value) 
        {
            #region Contabilidade
            if (value.enquadramentoId.HasValue && value.enquadramentoId > 0)
            {
                #region Mapear enquadramento para contabilidade
                ContabilidadeModel contabilidadeModel = new ContabilidadeModel(this.db, this.seguranca_db);
                ContabilidadeViewModel contabilidadeViewModel = contabilidadeModel.CreateRepositoryFromEnquadramento(new EnquadramentoViewModel() { enquadramentoId = value.enquadramentoId.Value });
                contabilidadeViewModel.dt_lancamento = value.dt_emissao;
                contabilidadeViewModel.documento = value.documento;
                int contador = 0;
                while (contador <= contabilidadeViewModel.ContabilidadeItems.Count() - 1)
                {
                    if (value.complementoHist != null && value.complementoHist != "")
                        contabilidadeViewModel.ContabilidadeItems.ElementAt(contador).complementoHist = value.complementoHist;
                    contabilidadeViewModel.ContabilidadeItems.ElementAt(contador++).valor = value.ContaPagarParcelas.Select(info => info.vr_principal).Sum();
                }
                    

                value.Contabilidade = contabilidadeViewModel;
                #endregion
            };
            #endregion

            #region Eventos
            int indice = 0;

            foreach (ContaPagarParcelaViewModel p in value.ContaPagarParcelas)
            {
                #region Recupera o evento "0" Inclusão de Operação, Modalidade = "Contas a Pagar"
                Evento eve = db.Eventos.Where(info => info.codigo == 0 && info.empresaId == value.empresaId).FirstOrDefault(); // Inclusão de operação de crédito 
                #endregion

                ContaPagarParcelaEventoViewModel contaPagarParcelaEventoEncargos = null;
                ContaPagarParcelaEventoViewModel contaPagarParcelaEventoMulta = null;
                ContaPagarParcelaEventoViewModel contaPagarParcelaEventoDesconto = null;
                ContaPagarParcelaEventoViewModel contaPagarParcelaEventoPagtoEncargos = null;
                ContaPagarParcelaEventoViewModel contaPagarParcelaEventoAmortizacao = null;

                // Se estiver amortizando a parcela, calcular os encargos com referência à data da amortização. Caso contrário, toma-se como referência a data atual
                DateTime? dt_referencia = Funcoes.Brasilia().Date;
                if (p.vr_amortizacao > 0)
                    dt_referencia = p.dt_ultima_amortizacao;

                #region Calcula o próximo dia útil em relação à data de referência
                ObterProximoDiaUtil obterProximoDiaUtil = new ObterProximoDiaUtil(this.db, this.seguranca_db, dt_referencia);
                obterProximoDiaUtil.Run(new FeriadoViewModel());
                DateTime dt_proximo_diaUtil = obterProximoDiaUtil.dt_referencia;
                #endregion

                #region Calcula o próximo dia útil em relação à data do movimento bancário
                DateTime? dt_movto_proximo_diaUtil = null;
                if (value.dt_movto.HasValue)
                {
                    ObterProximoDiaUtil obterProximoDiaUtil_dt_movto = new ObterProximoDiaUtil(this.db, this.seguranca_db, value.dt_movto.Value);
                    obterProximoDiaUtil_dt_movto.Run(new FeriadoViewModel());
                    dt_movto_proximo_diaUtil = obterProximoDiaUtil_dt_movto.dt_referencia;
                }
                #endregion

                #region Incluir Evento "0-Inclusão de Contas a Pagar"
                ContaPagarParcelaEventoViewModel contaPagarParcelaEvento = new ContaPagarParcelaEventoViewModel()
                {
                    operacaoId = value.operacaoId,
                    parcelaId = p.parcelaId,
                    dt_evento = Funcoes.Brasilia(),
                    eventoId = eve.eventoId,
                    dt_ocorrencia = value.dt_emissao,
                    valor = p.vr_principal,
                    ind_operacao = eve.ind_operacao,
                    ind_estorno = "N",
                    ind_tipoEvento = eve.ind_tipoEvento,
                };
                if (indice == 0)
                    contaPagarParcelaEvento.arquivo = value.fileBoleto; // boleto
                #endregion

                #region Incluir Evento "1-Encargos"
                
                #region Calcula o total de encargos (juros de mora se a parcela estiver atrasada)
                CalcularJurosMoraParcelaBI encargosProcess = new CalcularJurosMoraParcelaBI(this.db, this.seguranca_db, value.vr_jurosMora, dt_proximo_diaUtil);
                decimal? vr_encargos_calculado = encargosProcess.Run(p).vr_encargos;
                #endregion
                #region Calcula o total de encargos (multa se a parcela estiver atrasada)
                CalcularMultaAtrasoParcelaBI multaProcess = new CalcularMultaAtrasoParcelaBI(this.db, this.seguranca_db, value.vr_multa, dt_proximo_diaUtil);
                decimal? vr_multa_calculado = multaProcess.Run(p).vr_encargos ?? 0;
                #endregion

                if ((vr_encargos_calculado.HasValue && vr_encargos_calculado > 0) || (vr_multa_calculado.HasValue && vr_multa_calculado > 0))
                {
                    #region Valida encargos da amortização inicial (considerando que está baixando o título por liquidação ou amortização parcial)
                    if (value.ContaPagarParcela.vr_amortizacao.HasValue && value.ContaPagarParcela.vr_amortizacao.Value > 0 && indice == 0)
                    {
                        if (value.ContaPagarParcela.vr_encargos.HasValue && vr_encargos_calculado + vr_multa_calculado < value.ContaPagarParcela.vr_encargos)
                            throw new ArgumentException("Valor dos encargos informado (R$ " + value.ContaPagarParcela.vr_encargos.Value.ToString("###,###,###,##0.00") + ") é maior que o valor dos encargos calculado (R$ " + vr_encargos_calculado.Value.ToString("###,###,###,##0.00") + ") pelo sistema.");
                    }
                    #endregion

                    #region Recupera o evento "1-Encargos (multa, juros, encargos, tributos, taxas, tarifa)"
                    eve = db.Eventos.Where(info => info.codigo == 1 && info.empresaId == value.empresaId).FirstOrDefault(); // 1-Encargos
                    #endregion

                    #region Incluir Evento "1-Encargos (juros de mora)"
                    contaPagarParcelaEventoEncargos = new ContaPagarParcelaEventoViewModel()
                    {
                        operacaoId = value.operacaoId,
                        parcelaId = p.parcelaId,
                        dt_evento = Funcoes.Brasilia().AddSeconds(5),
                        eventoId = eve.eventoId,
                        dt_ocorrencia = dt_proximo_diaUtil,
                        valor = vr_encargos_calculado.Value,
                        ind_operacao = eve.ind_operacao,
                        ind_estorno = "N",
                        ind_tipoEvento = eve.ind_tipoEvento
                    };
                    #endregion

                    #region Recupera o evento "1-Encargos (multa por atraso)"
                    eve = db.Eventos.Where(info => info.codigo == 10 && info.empresaId == value.empresaId).FirstOrDefault(); // 10-Multa (1-encargos)
                    #endregion

                    #region Incluir Evento "1-Encargos (multa por atraso)"
                    contaPagarParcelaEventoMulta = new ContaPagarParcelaEventoViewModel()
                    {
                        operacaoId = value.operacaoId,
                        parcelaId = p.parcelaId,
                        dt_evento = Funcoes.Brasilia().AddSeconds(10),
                        eventoId = eve.eventoId,
                        dt_ocorrencia = dt_proximo_diaUtil,
                        valor = vr_multa_calculado.Value,
                        ind_operacao = eve.ind_operacao,
                        ind_estorno = "N",
                        ind_tipoEvento = eve.ind_tipoEvento
                    };
                    #endregion
                }
                else if (value.ContaPagarParcela.vr_encargos.HasValue && value.ContaPagarParcela.vr_encargos.Value > 0 && indice == 0)
                {
                    #region Recupera o evento "1-Encargos"
                    eve = db.Eventos.Where(info => info.codigo == 1 && info.empresaId == value.empresaId).FirstOrDefault(); // 1-Encargos
                    #endregion

                    #region Incluir Evento "1-Encargos (multa + juros de mora)"
                    contaPagarParcelaEventoEncargos = new ContaPagarParcelaEventoViewModel()
                    {
                        operacaoId = value.operacaoId,
                        parcelaId = p.parcelaId,
                        dt_evento = Funcoes.Brasilia().AddSeconds(5),
                        eventoId = eve.eventoId,
                        dt_ocorrencia = dt_proximo_diaUtil,
                        valor = value.ContaPagarParcela.vr_encargos.Value,
                        ind_operacao = eve.ind_operacao,
                        ind_estorno = "N",
                        ind_tipoEvento = eve.ind_tipoEvento
                    };
                    #endregion

                    p.vr_encargos = value.ContaPagarParcela.vr_encargos.Value;
                }
                #endregion

                #region verifica se o título foi liquidado
                p.ind_baixa = null;
                if (value.ContaPagarParcela.vr_amortizacao.HasValue && value.ContaPagarParcela.vr_amortizacao.Value > 0 && indice == 0)
                {
                    decimal _valor_para_liquidacao = p.vr_principal + (vr_encargos_calculado ?? 0) + (vr_multa_calculado ?? 0);
                    if (((vr_encargos_calculado ?? 0 + vr_multa_calculado ?? 0) == 0) && p.vr_encargos.HasValue && p.vr_encargos.Value > 0)
                        _valor_para_liquidacao = p.vr_principal + p.vr_encargos.Value;

                    if (value.ContaPagarParcela.vr_amortizacao.Value +
                        (value.ContaPagarParcela.vr_encargos.HasValue ? value.ContaPagarParcela.vr_encargos.Value : 0) == _valor_para_liquidacao)
                        p.ind_baixa = "4";
                }
                #endregion

                #region Incluir Evento "4-Baixa por motivo de liquidação" ou "3-Amortização"
                if (p.vr_amortizacao.HasValue && p.vr_amortizacao.Value > 0 && indice == 0)
                {
                    if (!p.bancoId.HasValue)
                        throw new ArgumentException("Banco deve ser informado.");

                    if (value.ContaPagarParcela.vr_amortizacao + (value.ContaPagarParcela.vr_encargos ?? 0) - (value.ContaPagarParcela.vr_desconto ?? 0) <= 0)
                        throw new ArgumentException("Valor amortizado deve ser um valor maior que zero.");

                    #region Incluir Evento "11-Pagamento de encargos/multa"
                    if (value.ContaPagarParcela.vr_encargos.HasValue && value.ContaPagarParcela.vr_encargos.Value > 0)
                    {
                        #region Recupera o evento "11-Pagamento de encargos/multa"
                        eve = db.Eventos.Where(info => info.codigo == 11 && info.empresaId == value.empresaId).FirstOrDefault();
                        #endregion

                        contaPagarParcelaEventoPagtoEncargos = new ContaPagarParcelaEventoViewModel()
                        {
                            operacaoId = value.operacaoId,
                            parcelaId = p.parcelaId,
                            dt_evento = Funcoes.Brasilia().AddSeconds(15),
                            eventoId = eve.eventoId,
                            dt_ocorrencia = dt_proximo_diaUtil,
                            valor = value.ContaPagarParcela.vr_encargos.Value,
                            ind_operacao = eve.ind_operacao,
                            ind_estorno = "N",
                            ind_tipoEvento = eve.ind_tipoEvento,
                            arquivo = p.parcelaId == 1 ? value.fileComprovante : null // comprovante
                        };
                    }
                    #endregion

                    #region Incluir Evento "2-Desconto"
                    if (value.ContaPagarParcela.vr_desconto.HasValue && indice == 0)
                    {
                        #region Recupera o evento "2-Desconto"
                        eve = db.Eventos.Where(info => info.codigo == 2 && info.empresaId == value.empresaId).FirstOrDefault(); // 2-Desconto
                        #endregion

                        #region Incluir Evento "2-Desconto"
                        contaPagarParcelaEventoDesconto = new ContaPagarParcelaEventoViewModel()
                        {
                            operacaoId = value.operacaoId,
                            parcelaId = p.parcelaId,
                            dt_evento = Funcoes.Brasilia().AddSeconds(20),
                            eventoId = eve.eventoId,
                            dt_ocorrencia = dt_proximo_diaUtil,
                            valor = value.ContaPagarParcela.vr_desconto.Value,
                            ind_operacao = eve.ind_operacao,
                            ind_estorno = "N",
                            ind_tipoEvento = eve.ind_tipoEvento
                        };
                        #endregion

                        p.vr_desconto = value.ContaPagarParcela.vr_desconto;
                    }
                    #endregion

                    if (p.ind_baixa == "4") // Liquidação
                    {
                        #region Recupera o evento "4-Baixa por motivo de liquidação"
                        eve = db.Eventos.Where(info => info.codigo == 4 && info.empresaId == value.empresaId).FirstOrDefault(); // 4-Baixa por motivo de liquidação
                        #endregion
                    }
                    else if (p.vr_amortizacao.HasValue && p.vr_amortizacao.Value > 0)
                    {
                        #region Recupera o evento "3-Amortização"
                        eve = db.Eventos.Where(info => info.codigo == 3 && info.empresaId == value.empresaId).FirstOrDefault(); // 3-Amortização
                        #endregion
                    }

                    #region Incluir Evento (Baixa ou Amortização)
                    contaPagarParcelaEventoAmortizacao = new ContaPagarParcelaEventoViewModel()
                    {
                        operacaoId = value.operacaoId,
                        parcelaId = p.parcelaId,
                        dt_evento = Funcoes.Brasilia().AddSeconds(25),
                        eventoId = eve.eventoId,
                        dt_ocorrencia = dt_proximo_diaUtil,
                        valor = value.ContaPagarParcela.vr_amortizacao.Value, 
                        ind_operacao = eve.ind_operacao,
                        ind_estorno = "N",
                        ind_tipoEvento = eve.ind_tipoEvento,
                        arquivo = p.parcelaId == 1 ? value.fileComprovante : null // comprovante
                    };
                    #endregion

                    #region Mapear Enquadramento para a Contabilidade
                    if (value.enquadramento_amortizacaoId.HasValue && p.parcelaId == 1)
                    {
                        #region Mapear enquadramento para contabilidade
                        ContaPagarParcelaEventoModel eveModel = new ContaPagarParcelaEventoModel(this.db, this.seguranca_db);
                        contaPagarParcelaEventoAmortizacao.enquadramentoId = value.enquadramento_amortizacaoId.Value;
                        contaPagarParcelaEventoAmortizacao = eveModel.BeforeInsert(contaPagarParcelaEventoAmortizacao);
                        contaPagarParcelaEventoAmortizacao.Contabilidade.dt_lancamento = dt_movto_proximo_diaUtil.Value;
                        contaPagarParcelaEventoAmortizacao.Contabilidade.documento = p.num_titulo != null && !String.IsNullOrWhiteSpace(p.num_titulo) ? p.num_titulo : value.documento;
                        #endregion
                    };
                    #endregion

                    #region Gerar Movimentação Bancária
                    MovtoBancarioViewModel movtoViewModel = new MovtoBancarioViewModel()
                    {
                        empresaId = sessaoCorrente.empresaId,
                        bancoId = p.bancoId.Value,
                        historicoId = value.historicoId,
                        complementoHist = value.complementoHist,
                        dt_movto = dt_movto_proximo_diaUtil.Value,
                        valor = value.ContaPagarParcela.vr_amortizacao.Value + (value.ContaPagarParcela.vr_encargos.HasValue ? value.ContaPagarParcela.vr_encargos.Value : 0) - (value.ContaPagarParcela.vr_desconto.HasValue ? value.ContaPagarParcela.vr_desconto.Value : 0),
                        documento = p.num_titulo != null && !String.IsNullOrWhiteSpace(p.num_titulo) ? p.num_titulo : value.documento,
                        tipoMovto = "D"
                    };
                    contaPagarParcelaEventoAmortizacao.MovtoBancario = movtoViewModel;
                    #endregion

                    p.vr_amortizacao = movtoViewModel.valor;
                }
                #endregion

                value.ContaPagarParcelas.ElementAt(indice).ContaPagarParcelaEventos = new List<ContaPagarParcelaEventoViewModel>();

                ((List<ContaPagarParcelaEventoViewModel>)value.ContaPagarParcelas.ElementAt(indice).ContaPagarParcelaEventos).Add(contaPagarParcelaEvento);

                if (contaPagarParcelaEventoEncargos != null)
                    ((List<ContaPagarParcelaEventoViewModel>)value.ContaPagarParcelas.ElementAt(indice).ContaPagarParcelaEventos).Add(contaPagarParcelaEventoEncargos);

                if (contaPagarParcelaEventoMulta != null)
                    ((List<ContaPagarParcelaEventoViewModel>)value.ContaPagarParcelas.ElementAt(indice).ContaPagarParcelaEventos).Add(contaPagarParcelaEventoMulta);

                if (contaPagarParcelaEventoDesconto != null)
                    ((List<ContaPagarParcelaEventoViewModel>)value.ContaPagarParcelas.ElementAt(indice).ContaPagarParcelaEventos).Add(contaPagarParcelaEventoDesconto);

                if (contaPagarParcelaEventoAmortizacao != null)
                    ((List<ContaPagarParcelaEventoViewModel>)value.ContaPagarParcelas.ElementAt(indice).ContaPagarParcelaEventos).Add(contaPagarParcelaEventoAmortizacao);

                if (contaPagarParcelaEventoPagtoEncargos != null)
                    ((List<ContaPagarParcelaEventoViewModel>)value.ContaPagarParcelas.ElementAt(indice).ContaPagarParcelaEventos).Add(contaPagarParcelaEventoPagtoEncargos);

                p.vr_saldo_devedor = p.ContaPagarParcelaEventos.Where(info => info.ind_operacao == "C").Select(info => info.valor).Sum() - p.ContaPagarParcelaEventos.Where(info => info.ind_operacao == "D").Select(info => info.valor).Sum();

                indice++;
            }

            #endregion

            return value; 
        }

        public override ContaPagarViewModel AfterInsert(ContaPagarViewModel value)
        {
            #region Chamar o método AfterInsert dos eventos para mover os arquivos de boleto e comprovante (e fazer a atualização dos saldos)
            try
            {
                ContaPagarParcelaEventoModel pevModel = new ContaPagarParcelaEventoModel(this.db, this.seguranca_db);
                foreach (ContaPagarParcelaViewModel par in value.ContaPagarParcelas)
                {
                    foreach(ContaPagarParcelaEventoViewModel pev in par.ContaPagarParcelaEventos)
                    {
                        ContaPagarParcelaEventoViewModel evm = pevModel.AfterInsert(pev);
                        if (evm.mensagem.Code > 0)
                        {
                            value.mensagem = evm.mensagem;
                            return value;
                        }                    
                    }
                }
            }
            catch (Exception ex)
            {
                value.mensagem.Code = 17;
                value.mensagem.Message = MensagemPadrao.Message(17).ToString();
                value.mensagem.MessageBase = new App_DominioException(ex.InnerException.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                value.mensagem.MessageType = MsgType.ERROR;
            }
            #endregion

            return base.AfterInsert(value);
        }

        public override ContaPagar MapToEntity(ContaPagarViewModel value)
        {
            ContaPagar c = Find(value);

            #region Contas a Pagar
            if (c == null)
            {
                c = new ContaPagar();
                c.ContaPagarParcelas = new List<ContaPagarParcela>();
            }
            else
                c.ContaPagarParcelas.Clear(); 

            c.operacaoId = value.operacaoId;
            c.empresaId = value.empresaId;
            c.credorId = value.credorId;
            c.historicoId = value.historicoId;
            c.complementoHist = value.complementoHist;
            c.centroCustoId = value.centroCustoId;
            c.dt_emissao = value.dt_emissao;
            c.vr_jurosMora = value.vr_jurosMora;
            c.vr_multa = value.vr_multa;
            c.documento = value.documento;
            c.recorrencia = value.recorrencia_mensal ? "S" : "N";

            #region Mapear contabilidade para Entity
            if (value.Contabilidade != null)
            {
                ContabilidadeModel contabilidadeModel = new ContabilidadeModel(this.db, this.seguranca_db);
                c.Contabilidade = contabilidadeModel.MapToEntity(value.Contabilidade);
            };
            #endregion

            #endregion

            #region Parcelas do Contas a Pagar
            ContaPagarParcelaCrudModel parModel = new ContaPagarParcelaCrudModel(this.db, this.seguranca_db);

            foreach (ContaPagarParcelaViewModel p in value.ContaPagarParcelas)
            {
                ContaPagarParcela par = parModel.MapToEntity(p);
                c.ContaPagarParcelas.Add(par);
            }
            #endregion

            return c;
        }

        public override ContaPagarViewModel MapToRepository(ContaPagar entity)
        {
            #region Contas a Pagar
            ContaPagarViewModel r = new ContaPagarViewModel()
            {
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS },
                operacaoId = entity.operacaoId,
                empresaId = entity.empresaId,
                credorId = entity.credorId,
                nome_credor = db.Credores.Find(entity.credorId).nome,
                descricao_grupoCredor = db.Credores.Find(entity.credorId).grupoCredorId.HasValue ? db.GrupoCredores.Find(db.Credores.Find(entity.credorId).grupoCredorId).nome : "",
                historicoId = entity.historicoId,
                descricao_historico = db.Historicos.Find(entity.historicoId).descricao,
                complementoHist = entity.complementoHist,
                centroCustoId = entity.centroCustoId,
                descricao_centroCusto = entity.centroCustoId != null ? db.CentroCustos.Find(entity.centroCustoId).descricao : "",
                dt_emissao = entity.dt_emissao,
                vr_jurosMora = entity.vr_jurosMora,
                vr_multa = entity.vr_multa,
                documento = entity.documento,
                recorrencia = entity.recorrencia,
                recorrencia_mensal = entity.recorrencia == "S" ? true : false,
                dt_movto = Funcoes.Brasilia().Date,
                ContaPagarParcelas = new List<ContaPagarParcelaViewModel>()
            };
            #endregion

            #region Contabilidade da operação de contas a pagar
            if (entity.Contabilidade != null)
            {
                ContabilidadeModel contModel = new ContabilidadeModel(this.db, this.seguranca_db);
                r.Contabilidade = contModel.MapToRepository(entity.Contabilidade);
            }
            #endregion

            #region Parcelas do Contas a Pagar

            ContaPagarParcelaCrudModel parModel = new ContaPagarParcelaCrudModel(this.db, this.seguranca_db);

            foreach (ContaPagarParcela p in entity.ContaPagarParcelas)
            {
                ContaPagarParcelaViewModel par = parModel.MapToRepository(p);
                ((List<ContaPagarParcelaViewModel>)r.ContaPagarParcelas).Add(par);
            }
            #endregion

            #region Parcela para liquidação
            r.ContaPagarParcela = new ContaPagarParcelaViewModel()
            {
                bancoId = r.ContaPagarParcelas.FirstOrDefault().bancoId,
                nome_banco = r.ContaPagarParcelas.FirstOrDefault().nome_banco,
                ind_forma_pagamento = r.ContaPagarParcelas.FirstOrDefault().ind_forma_pagamento ?? "",
                dt_vencimento = Funcoes.Brasilia().Date,
                vr_principal = r.ContaPagarParcelas.Sum(info => info.vr_saldo_devedor).GetValueOrDefault(0),
                vr_saldo_devedor = r.ContaPagarParcelas.Sum(info => info.vr_saldo_devedor).GetValueOrDefault(0),
                dt_baixa = Funcoes.Brasilia().Date,
            };
            #endregion

            return r;
        }

        public override ContaPagar Find(ContaPagarViewModel key)
        {
            ContaPagar entity = db.ContaPagars.Find(key.operacaoId);
            if (entity != null && (entity.ContaPagarParcelas.Count() == 0 || entity.empresaId != sessaoCorrente.empresaId))
                return null;

            return entity;
        }

        public override Validate Validate(ContaPagarViewModel value, Crud operation)
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

            if (operation != Crud.INCLUIR && value.operacaoId <= 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Operação ID").ToString();
                value.mensagem.MessageBase = "Campo obrigatório: Operação ID";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (operation != Crud.EXCLUIR)
            {
                if (value.credorId == 0)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.MessageBase = MensagemPadrao.Message(5, "Fornecedor").ToString();
                    value.mensagem.Message = "Campo obrigatório: Fornecedor";
                    value.mensagem.MessageType = MsgType.WARNING;

                    return value.mensagem;
                }

                if (value.historicoId == 0)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.MessageBase = MensagemPadrao.Message(5, "Histórico").ToString();
                    value.mensagem.Message = "Campo obrigatório: Histórico";
                    value.mensagem.MessageType = MsgType.WARNING;

                    return value.mensagem;
                }

                // validar a contabilidade (contas a pagar)
                if (value.Contabilidade != null)
                {
                    ContabilidadeModel contabilidadeModel = new ContabilidadeModel(this.db, this.seguranca_db);
                    Validate validate = contabilidadeModel.Validate(value.Contabilidade, Crud.INCLUIR);
                    if (validate.Code > 0)
                        return validate;
                }

                if (value.ContaPagarParcelas.Count() == 0)
                {
                    value.mensagem.Code = 46;
                    value.mensagem.MessageBase = MensagemPadrao.Message(46, "parcealas de contas a pagar").ToString();
                    value.mensagem.Message = "Deve ser incluído pelo meno uma parcela da operação.";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                // Validar parcelas do contas a pagar
                ContaPagarParcelaCrudModel parModel = new ContaPagarParcelaCrudModel(this.db, this.seguranca_db);
                foreach (ContaPagarParcelaViewModel par in value.ContaPagarParcelas)
                {
                    Validate validate = parModel.Validate(par, operation);
                    if (validate.Code > 0)
                        return validate;
                }
            }

            return value.mensagem;
        }

        public override ContaPagarViewModel CreateRepository(System.Web.HttpRequestBase Request = null)
        {
            DateTime d = Funcoes.Brasilia().Date;
            if (db.ContaPagars.ToList().Where(info => info.empresaId.Equals(sessaoCorrente.empresaId)).Count() > 0)
                d = db.ContaPagars.ToList().Where(info => info.empresaId.Equals(sessaoCorrente.empresaId)).OrderByDescending(ord => ord.operacaoId).Take(1).Last().dt_emissao;

            ContaPagarViewModel r = new ContaPagarViewModel()
            {
                operacaoId = 0,
                empresaId = sessaoCorrente.empresaId,
                dt_emissao = d,
                recorrencia = "N",
                recorrencia_mensal = false,
                num_parcelas = 1,
                dt_movto = Funcoes.Brasilia().Date,
                documento = "",
                nome_credor = "",
                descricao_grupoCredor = "",
                descricao_historico = "",
                complementoHist = "",
                descricao_centroCusto = "",
                ContaPagarParcela = new ContaPagarParcelaViewModel()
                {
                    parcelaId = 1,
                    dt_vencimento = Funcoes.Brasilia().Date,
                    dt_baixa = Funcoes.Brasilia().Date,
                    ind_forma_pagamento = "4",
                    ContaPagarParcelaEvento = new ContaPagarParcelaEventoViewModel()
                    {
                        parcelaId = 1,
                        dt_ocorrencia = Funcoes.Brasilia().Date,
                        dt_movto = Funcoes.Brasilia().Date
                    }
                },
                ContaPagarParcelas = new List<ContaPagarParcelaViewModel>(),
                mensagem = new Validate() {  Code = 0, Message = "Registro processado com sucesso !"}
            };

            return r;
        }
        #endregion

        #region Métodos Customizados

        #endregion
    }

    public class _ListViewContaPagar : ListViewModel<ContaPagarViewModel, ApplicationContext>
    {
        #region Constructor
        public _ListViewContaPagar() { }
        public _ListViewContaPagar(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe ListViewRepository
        public override IEnumerable<ContaPagarViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            #region Parâmetros
            #region Títulos em Aberto
            bool titulos_vencidos_atraso = (bool)param[0];
            DateTime? dt_vencidos_atraso1 = (DateTime?)param[1];
            DateTime? dt_vencidos_atraso2 = (DateTime?)param[2];

            bool titulos_a_vencer = (bool)param[3];
            DateTime? dt_vencimento1 = (DateTime?)param[4];
            DateTime? dt_vencimento2 = (DateTime?)param[5];

            bool titulos_amortizados = (bool)param[6];
            bool titulos_nao_pagos = (bool)param[7];
            #endregion

            #region Títulos Baixados
            bool baixa_liquidacao = (bool)param[8];
            bool baixa_cancelamento = (bool)param[9];
            DateTime? dt_baixa1 = (DateTime?)param[10];
            DateTime? dt_baixa2 = (DateTime?)param[11];
            #endregion

            #region Demais parâmetros
            int? credorId = (int?)param[12];
            DateTime? dt_emissao1 = (DateTime?)param[13];
            DateTime? dt_emissao2 = (DateTime?)param[14];
            int? centroCustoId = (int?)param[15];
            int? grupoId = (int?)param[16];
            int? bancoId = (int?)param[17];
            #endregion
            #endregion

            DateTime hoje = Funcoes.Brasilia().Date;

            #region LINQ
            var q = (from pag in db.ContaPagars join par in db.ContaPagarParcelas on pag.operacaoId equals par.operacaoId
                     //join pge in db.ContaPagarParcelaEventos on new { par.operacaoId, par.parcelaId } equals new { pge.operacaoId, pge.parcelaId }
                     join cre in db.Credores on pag.credorId equals cre.credorId
                     where pag.empresaId.Equals(sessaoCorrente.empresaId)
                            && ((((titulos_vencidos_atraso && par.vr_saldo_devedor > 0
                                && ((dt_vencidos_atraso1.HasValue && par.dt_vencimento >= dt_vencidos_atraso1 && par.dt_vencimento <= dt_vencidos_atraso2) ||
                                     (!dt_vencidos_atraso1.HasValue && par.dt_vencimento < hoje)))
                                    || (titulos_a_vencer && par.vr_saldo_devedor > 0
                                        && ((dt_vencimento1.HasValue && par.dt_vencimento >= dt_vencimento1 && par.dt_vencimento <= dt_vencimento2) ||
                                         (!dt_vencimento1.HasValue && par.dt_vencimento >= hoje))))
                                && ((titulos_amortizados && par.vr_saldo_devedor > 0 && par.vr_amortizacao > 0)
                                    || (titulos_nao_pagos && par.vr_saldo_devedor > 0 && par.vr_amortizacao == 0)))
                                || (baixa_liquidacao && par.ind_baixa == "4" && par.dt_baixa >= dt_baixa1 && par.dt_baixa <= dt_baixa2)
                                || (baixa_cancelamento && par.ind_baixa == "5" && par.dt_baixa >= dt_baixa1 && par.dt_baixa <= dt_baixa2))
                            && (!credorId.HasValue || pag.credorId == credorId)
                            && (!centroCustoId.HasValue || pag.centroCustoId == centroCustoId)
                            && (!grupoId.HasValue || cre.grupoCredorId.Value == grupoId)
                            && (!bancoId.HasValue || par.bancoId.Value == bancoId)
                            && (!dt_emissao1.HasValue || pag.dt_emissao >= dt_emissao1 && pag.dt_emissao <= dt_emissao2)
                     orderby par.dt_vencimento
                     select new ContaPagarViewModel
                     {
                         operacaoId = pag.operacaoId,
                         empresaId = pag.empresaId,
                         nome_credor = cre.nome,
                         dt_emissao = pag.dt_emissao,
                         documento = pag.documento,
                         recorrencia = pag.recorrencia,
                         ContaPagarParcela = new ContaPagarParcelaViewModel()
                         {
                             operacaoId = par.operacaoId,
                             parcelaId = par.parcelaId,
                             dt_vencimento = par.dt_vencimento,
                             ind_baixa = par.ind_baixa,
                             dt_baixa = par.dt_baixa,
                             dt_ultima_amortizacao = par.dt_ultima_amortizacao,
                             vr_principal = par.vr_principal,
                             vr_encargos = par.vr_encargos,
                             vr_desconto = par.vr_desconto,
                             vr_amortizacao = par.vr_amortizacao,
                             vr_total_pago = par.vr_total_pago,
                             vr_saldo_devedor = par.vr_saldo_devedor
                         },
                         PageSize = pageSize,
                         TotalCount = (  from pag1 in db.ContaPagars join par1 in db.ContaPagarParcelas on pag1.operacaoId equals par1.operacaoId
                                         join cre1 in db.Credores on pag1.credorId equals cre1.credorId
                                         where pag1.empresaId.Equals(sessaoCorrente.empresaId)
                                                && ((((titulos_vencidos_atraso && par1.vr_saldo_devedor > 0
                                                    && ((dt_vencidos_atraso1.HasValue && par1.dt_vencimento >= dt_vencidos_atraso1 && par1.dt_vencimento <= dt_vencidos_atraso2) ||
                                                         (!dt_vencidos_atraso1.HasValue && par1.dt_vencimento < hoje)))
                                                        || (titulos_a_vencer && par1.vr_saldo_devedor > 0
                                                            && ((dt_vencimento1.HasValue && par1.dt_vencimento >= dt_vencimento1 && par1.dt_vencimento <= dt_vencimento2) ||
                                                             (!dt_vencimento1.HasValue && par1.dt_vencimento >= hoje))))
                                                    && ((titulos_amortizados && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao > 0)
                                                        || (titulos_nao_pagos && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao == 0)))
                                                    || (baixa_liquidacao && par1.ind_baixa == "4" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2)
                                                    || (baixa_cancelamento && par1.ind_baixa == "5" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2))
                                                && (!credorId.HasValue || pag1.credorId == credorId)
                                                && (!centroCustoId.HasValue || pag1.centroCustoId == centroCustoId)
                                                && (!grupoId.HasValue || cre1.grupoCredorId.Value == grupoId)
                                                && (!bancoId.HasValue || par1.bancoId.Value == bancoId)
                                                && (!dt_emissao1.HasValue || pag1.dt_emissao >= dt_emissao1 && pag1.dt_emissao <= dt_emissao2)
                                         orderby par1.dt_vencimento
                                         select pag1).Count()
                     }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();
            #endregion

            return q;
        }

        public override Repository getRepository(Object id)
        {
            return new ContaPagarModel(this.db, this.seguranca_db).getObject((ContaPagarViewModel)id);
        }

        public override string action()
        {
            return "ListParam";
        }


        public override string DivId()
        {
            return "div-list-static";
        }
        #endregion
    }

    public class _ListViewContaPagarParcela : ListViewModel<ContaPagarParcelaViewModel, ApplicationContext>
    {
        #region Constructor
        public _ListViewContaPagarParcela() { }
        public _ListViewContaPagarParcela(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe ListViewRepository
        public override IEnumerable<ContaPagarParcelaViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            #region Parâmetros
            string _num_titulo = param[0] != null ? param[0].ToString() : "";
            DateTime _dt_vencimento = (DateTime)param [1];
            string _ind_forma_pagamento = param[2].ToString();
            string _nome_banco = param[3] != null ? param[3].ToString() : "";
            int? _cheque_banco = (int?)param[4];
            string _cheque_agencia = param[5] != null ? param[5].ToString() : null;
            string _cheque_numero = param[6] != null ? param[6].ToString() : null;
            decimal _vr_principal = (decimal)param[7];
            decimal _vr_amortizacao = param[8] != null ? (decimal)param[8] : 0;
            decimal _vr_juros = param[9] != null ? (decimal)param[9] : 0;
            decimal _vr_mora = param[10] != null ? (decimal)param[10] : 0;
            decimal _vr_desconto = param[11] != null ? (decimal)param[11] : 0;
            int _num_parcelas = (int)param[12];
            int _bancoId = (int)(param[13]);
            #endregion

            IList<ContaPagarParcelaViewModel> Parcelas = new List<ContaPagarParcelaViewModel>();
            decimal vr_parcela = Math.Round((_vr_principal - _vr_amortizacao) / _num_parcelas, 2);

            int cont = 0;
            int dia_vencimento = _dt_vencimento.Day;
            int _num_cheque = 0;
            DateTime data_suporte;
            bool flag = false;

            if (_cheque_numero != null && _cheque_numero != "")
                try
                {
                    _num_cheque = int.Parse(_cheque_numero);
                    flag = true;
                }
                catch
                {
                    flag = false;
                };

            _num_parcelas = _vr_amortizacao == 0 ? _num_parcelas - 1 : _num_parcelas;

            decimal total = 0;
            using (ApplicationContext db = new ApplicationContext())
            {
                while (cont++ <= _num_parcelas)
                {
                    if (flag)
                    {
                        _cheque_numero = _num_cheque.ToString();
                        _num_cheque++;
                    }

                    if (cont > _num_parcelas)
                    {
                        vr_parcela = _vr_principal - total;
                    }

                    total += (cont == 1 && _vr_amortizacao > 0 ? _vr_amortizacao : vr_parcela);

                    ContaPagarParcelaViewModel p = new ContaPagarParcelaViewModel()
                    {
                        parcelaId = cont,
                        num_titulo = _num_titulo,
                        dt_vencimento = _dt_vencimento,
                        ind_forma_pagamento = _ind_forma_pagamento,
                        nome_banco = db.Bancos.Find(_bancoId).sigla ?? _nome_banco,
                        cheque_banco = _cheque_banco,
                        cheque_agencia = _cheque_agencia,
                        cheque_numero = _cheque_numero,
                        vr_principal = (cont == 1 && _vr_amortizacao > 0 ? _vr_amortizacao : vr_parcela),
                        vr_amortizacao = (cont == 1 && _vr_amortizacao > 0 ? _vr_amortizacao : 0),
                        vr_encargos = (cont == 1 && _vr_amortizacao > 0 ? _vr_juros + _vr_mora : 0),
                        vr_desconto = (cont == 1 && _vr_amortizacao > 0 ? _vr_desconto : 0),
                        vr_saldo_devedor = (cont == 1 && _vr_amortizacao > 0 ? 0 : vr_parcela)
                    };

                    Parcelas.Add(p);

                    _dt_vencimento = _dt_vencimento.AddMonths(1);

                    if (dia_vencimento > 28)
                    {
                        try
                        {
                            data_suporte = Convert.ToDateTime(_dt_vencimento.ToString("yyyy-MM-") + dia_vencimento.ToString());
                            _dt_vencimento = data_suporte;
                        }
                        catch
                        {

                        }
                    }
                }
            }

            return Parcelas;
        }

        public override Repository getRepository(Object id)
        {
            throw new NotImplementedException();
        }

        public override string DivId()
        {
            return "div-item";
        }
        #endregion

    }
}