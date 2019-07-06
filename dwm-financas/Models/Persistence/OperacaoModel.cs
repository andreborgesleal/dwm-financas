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
    public abstract class OperacaoModel<O, ORepo, OP, OPRepo, OPE, OPERepo, OPModel, OPEModel> : CrudModel<O, ORepo, ApplicationContext>
        where O : Operacao<OP, OPE>
        where ORepo : OperacaoViewModel<OPRepo,OPERepo>
        where OP : OperacaoParcela<OPE>
        where OPRepo : OperacaoParcelaViewModel<OPERepo>
        where OPE : OperacaoParcelaEvento
        where OPERepo : OperacaoParcelaEventoViewModel
        where OPModel : OperacaoParcelaCrudModel<OP, OPRepo,OPE, OPERepo, OPEModel>
        where OPEModel : OperacaoParcelaEventoModel<OPE, OPERepo>
    {
        #region Constructor
        public OperacaoModel() { }
        public OperacaoModel(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
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
        protected OPModel getParcelaModelInstance()
        {
            Type typeInstance = typeof(OPModel);
            return (OPModel)Activator.CreateInstance(typeInstance);
        }
        protected OPEModel getModelInstance()
        {
            Type typeInstance = typeof(OPEModel);
            return (OPEModel)Activator.CreateInstance(typeInstance);
        }
        #endregion

        #region Abstract Methods
        protected abstract string getTipoMovto();
        protected abstract DateTime getDataUltimaInclusao();
        #endregion

        #region Métodos da classe CrudModel
        public override ORepo BeforeInsert(ORepo value)
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
                    if (value.complementoHist != null && value.complementoHist != "" && contabilidadeViewModel.ContabilidadeItems.ElementAt(contador).complementoHist.Trim() == "")
                        contabilidadeViewModel.ContabilidadeItems.ElementAt(contador).complementoHist = value.complementoHist;
                    if (contabilidadeViewModel.ContabilidadeItems.ElementAt(contador).valor == 0)
                        contabilidadeViewModel.ContabilidadeItems.ElementAt(contador).valor = value.OperacaoParcelas.Select(info => info.vr_principal).Sum();

                    contador++;
                }
                value.Contabilidade = contabilidadeViewModel;
                #endregion
            };
            #endregion

            #region Eventos
            int indice = 0;

            foreach (OPRepo p in value.OperacaoParcelas)
            {
                #region Recupera o evento "0" Inclusão de Operação, Modalidade = "Contas a Receber"
                Evento eve = db.Eventos.Where(info => info.codigo == 0 && info.empresaId == value.empresaId).FirstOrDefault(); // Inclusão de operação de crédito 
                #endregion

                OPERepo operacaoParcelaEventoEncargos = null;
                OPERepo operacaoParcelaEventoMulta = null;
                OPERepo operacaoParcelaEventoDesconto = null;
                OPERepo operacaoParcelaEventoPagtoEncargos = null;
                OPERepo operacaoParcelaEventoAmortizacao = null;

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

                #region Incluir Evento "0-Inclusão de Contas a Receber"
                OPERepo operacaoParcelaEvento = getOperacaoParcelaEventoRepositoryInstance();
                operacaoParcelaEvento.operacaoId = value.operacaoId;
                operacaoParcelaEvento.parcelaId = p.parcelaId;
                operacaoParcelaEvento.dt_evento = Funcoes.Brasilia();
                operacaoParcelaEvento.eventoId = eve.eventoId;
                operacaoParcelaEvento.dt_ocorrencia = value.dt_emissao;
                operacaoParcelaEvento.valor = p.vr_principal;
                operacaoParcelaEvento.ind_operacao = eve.ind_operacao;
                operacaoParcelaEvento.ind_estorno = "N";
                operacaoParcelaEvento.ind_tipoEvento = eve.ind_tipoEvento;

                if (indice == 0)
                    operacaoParcelaEvento.arquivo = value.fileBoleto; // boleto
                #endregion

                #region Incluir Evento "1-Encargos"

                #region Calcula o total de encargos (juros de mora se a parcela estiver atrasada)
                _CalcularJurosMoraParcelaBI<OPRepo,OPERepo> encargosProcess = new _CalcularJurosMoraParcelaBI<OPRepo, OPERepo>(this.db, this.seguranca_db, value.vr_jurosMora, dt_proximo_diaUtil);
                decimal? vr_encargos_calculado = encargosProcess.Run(p).vr_encargos;
                #endregion

                #region Calcula o total de encargos (multa se a parcela estiver atrasada)
                _CalcularMultaAtrasoParcelaBI<OPRepo, OPERepo> multaProcess = new _CalcularMultaAtrasoParcelaBI<OPRepo, OPERepo>(this.db, this.seguranca_db, value.vr_multa, dt_proximo_diaUtil);
                decimal? vr_multa_calculado = multaProcess.Run(p).vr_encargos ?? 0;
                #endregion

                if ((vr_encargos_calculado.HasValue && vr_encargos_calculado > 0) || (vr_multa_calculado.HasValue && vr_multa_calculado > 0))
                {
                    #region Valida encargos da amortização inicial (considerando que está baixando o título por liquidação ou amortização parcial)
                    if (value.OperacaoParcela.vr_amortizacao.HasValue && value.OperacaoParcela.vr_amortizacao.Value > 0 && indice == 0)
                    {
                        if (value.OperacaoParcela.vr_encargos.HasValue && vr_encargos_calculado + vr_multa_calculado < value.OperacaoParcela.vr_encargos)
                            throw new ArgumentException("Valor dos encargos informado (R$ " + value.OperacaoParcela.vr_encargos.Value.ToString("###,###,###,##0.00") + ") é maior que o valor dos encargos calculado (R$ " + vr_encargos_calculado.Value.ToString("###,###,###,##0.00") + ") pelo sistema.");
                    }
                    #endregion

                    #region Recupera o evento "1-Encargos (multa, juros, encargos, tributos, taxas, tarifa)"
                    eve = db.Eventos.Where(info => info.codigo == 1 && info.empresaId == value.empresaId).FirstOrDefault(); // 1-Encargos
                    #endregion

                    #region Incluir Evento "1-Encargos (juros de mora)"
                    operacaoParcelaEventoEncargos = getOperacaoParcelaEventoRepositoryInstance();
                    operacaoParcelaEventoEncargos.operacaoId = value.operacaoId;
                    operacaoParcelaEventoEncargos.parcelaId = p.parcelaId;
                    operacaoParcelaEventoEncargos.dt_evento = Funcoes.Brasilia().AddSeconds(5);
                    operacaoParcelaEventoEncargos.eventoId = eve.eventoId;
                    operacaoParcelaEventoEncargos.dt_ocorrencia = dt_proximo_diaUtil;
                    operacaoParcelaEventoEncargos.valor = vr_encargos_calculado.Value;
                    operacaoParcelaEventoEncargos.ind_operacao = eve.ind_operacao;
                    operacaoParcelaEventoEncargos.ind_estorno = "N";
                    operacaoParcelaEventoEncargos.ind_tipoEvento = eve.ind_tipoEvento;
                    #endregion

                    #region Recupera o evento "1-Encargos (multa por atraso)"
                    eve = db.Eventos.Where(info => info.codigo == 10 && info.empresaId == value.empresaId).FirstOrDefault(); // 10-Multa (1-encargos)
                    #endregion

                    #region Incluir Evento "1-Encargos (multa por atraso)"
                    operacaoParcelaEventoMulta = getOperacaoParcelaEventoRepositoryInstance();

                    operacaoParcelaEventoMulta.operacaoId = value.operacaoId;
                    operacaoParcelaEventoMulta.parcelaId = p.parcelaId;
                    operacaoParcelaEventoMulta.dt_evento = Funcoes.Brasilia().AddSeconds(10);
                    operacaoParcelaEventoMulta.eventoId = eve.eventoId;
                    operacaoParcelaEventoMulta.dt_ocorrencia = dt_proximo_diaUtil;
                    operacaoParcelaEventoMulta.valor = vr_multa_calculado.Value;
                    operacaoParcelaEventoMulta.ind_operacao = eve.ind_operacao;
                    operacaoParcelaEventoMulta.ind_estorno = "N";
                    operacaoParcelaEventoMulta.ind_tipoEvento = eve.ind_tipoEvento;
                    #endregion
                }
                else if (value.OperacaoParcela.vr_encargos.HasValue && value.OperacaoParcela.vr_encargos.Value > 0 && indice == 0)
                {
                    #region Recupera o evento "1-Encargos"
                    eve = db.Eventos.Where(info => info.codigo == 1 && info.empresaId == value.empresaId).FirstOrDefault(); // 1-Encargos
                    #endregion

                    #region Incluir Evento "1-Encargos (multa + juros de mora)"
                    operacaoParcelaEventoEncargos = getOperacaoParcelaEventoRepositoryInstance();
                    operacaoParcelaEventoEncargos.operacaoId = value.operacaoId;
                    operacaoParcelaEventoEncargos.parcelaId = p.parcelaId;
                    operacaoParcelaEventoEncargos.dt_evento = Funcoes.Brasilia().AddSeconds(5);
                    operacaoParcelaEventoEncargos.eventoId = eve.eventoId;
                    operacaoParcelaEventoEncargos.dt_ocorrencia = dt_proximo_diaUtil;
                    operacaoParcelaEventoEncargos.valor = value.OperacaoParcela.vr_encargos.Value;
                    operacaoParcelaEventoEncargos.ind_operacao = eve.ind_operacao;
                    operacaoParcelaEventoEncargos.ind_estorno = "N";
                    operacaoParcelaEventoEncargos.ind_tipoEvento = eve.ind_tipoEvento;
                    #endregion

                    p.vr_encargos = value.OperacaoParcela.vr_encargos.Value;
                }
                #endregion

                #region verifica se o título foi liquidado
                p.ind_baixa = null;
                if (value.OperacaoParcela.vr_amortizacao.HasValue && value.OperacaoParcela.vr_amortizacao.Value > 0 && indice == 0)
                {
                    decimal _valor_para_liquidacao = p.vr_principal + (vr_encargos_calculado ?? 0) + (vr_multa_calculado ?? 0);
                    if (((vr_encargos_calculado ?? 0 + vr_multa_calculado ?? 0) == 0) && p.vr_encargos.HasValue && p.vr_encargos.Value > 0)
                        _valor_para_liquidacao = p.vr_principal + p.vr_encargos.Value;

                    if (value.OperacaoParcela.vr_amortizacao.Value +
                        (value.OperacaoParcela.vr_encargos.HasValue ? value.OperacaoParcela.vr_encargos.Value : 0) == _valor_para_liquidacao)
                        p.ind_baixa = "4";
                }
                #endregion

                #region Incluir Evento "4-Baixa por motivo de liquidação" ou "3-Amortização"
                if (p.vr_amortizacao.HasValue && p.vr_amortizacao.Value > 0 && indice == 0)
                {
                    //if (!p.bancoId.HasValue)
                    //    throw new ArgumentException("Banco deve ser informado.");

                    if (value.OperacaoParcela.vr_amortizacao + (value.OperacaoParcela.vr_encargos ?? 0) - (value.OperacaoParcela.vr_desconto ?? 0) <= 0)
                        throw new ArgumentException("Valor amortizado deve ser um valor maior que zero.");

                    #region Incluir Evento "11-Pagamento de encargos/multa"
                    if (value.OperacaoParcela.vr_encargos.HasValue && value.OperacaoParcela.vr_encargos.Value > 0)
                    {
                        #region Recupera o evento "11-Pagamento de encargos/multa"
                        eve = db.Eventos.Where(info => info.codigo == 11 && info.empresaId == value.empresaId).FirstOrDefault();
                        #endregion

                        operacaoParcelaEventoPagtoEncargos = getOperacaoParcelaEventoRepositoryInstance();
                        operacaoParcelaEventoPagtoEncargos.operacaoId = value.operacaoId;
                        operacaoParcelaEventoPagtoEncargos.parcelaId = p.parcelaId;
                        operacaoParcelaEventoPagtoEncargos.dt_evento = Funcoes.Brasilia().AddSeconds(15);
                        operacaoParcelaEventoPagtoEncargos.eventoId = eve.eventoId;
                        operacaoParcelaEventoPagtoEncargos.dt_ocorrencia = dt_proximo_diaUtil;
                        operacaoParcelaEventoPagtoEncargos.valor = value.OperacaoParcela.vr_encargos.Value;
                        operacaoParcelaEventoPagtoEncargos.ind_operacao = eve.ind_operacao;
                        operacaoParcelaEventoPagtoEncargos.ind_estorno = "N";
                        operacaoParcelaEventoPagtoEncargos.ind_tipoEvento = eve.ind_tipoEvento;
                        operacaoParcelaEventoPagtoEncargos.arquivo = p.parcelaId == 1 ? value.fileComprovante : null; // comprovante
                    }
                    #endregion

                    #region Incluir Evento "2-Desconto"
                    if (value.OperacaoParcela.vr_desconto.HasValue && indice == 0)
                    {
                        #region Recupera o evento "2-Desconto"
                        eve = db.Eventos.Where(info => info.codigo == 2 && info.empresaId == value.empresaId).FirstOrDefault(); // 2-Desconto
                        #endregion

                        #region Incluir Evento "2-Desconto"
                        operacaoParcelaEventoDesconto = getOperacaoParcelaEventoRepositoryInstance();

                        operacaoParcelaEventoDesconto.operacaoId = value.operacaoId;
                        operacaoParcelaEventoDesconto.parcelaId = p.parcelaId;
                        operacaoParcelaEventoDesconto.dt_evento = Funcoes.Brasilia().AddSeconds(20);
                        operacaoParcelaEventoDesconto.eventoId = eve.eventoId;
                        operacaoParcelaEventoDesconto.dt_ocorrencia = dt_proximo_diaUtil;
                        operacaoParcelaEventoDesconto.valor = value.OperacaoParcela.vr_desconto.Value;
                        operacaoParcelaEventoDesconto.ind_operacao = eve.ind_operacao;
                        operacaoParcelaEventoDesconto.ind_estorno = "N";
                        operacaoParcelaEventoDesconto.ind_tipoEvento = eve.ind_tipoEvento;
                        #endregion

                        p.vr_desconto = value.OperacaoParcela.vr_desconto;
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
                    operacaoParcelaEventoAmortizacao = getOperacaoParcelaEventoRepositoryInstance();

                    operacaoParcelaEventoAmortizacao.operacaoId = value.operacaoId;
                    operacaoParcelaEventoAmortizacao.parcelaId = p.parcelaId;
                    operacaoParcelaEventoAmortizacao.dt_evento = Funcoes.Brasilia().AddSeconds(25);
                    operacaoParcelaEventoAmortizacao.eventoId = eve.eventoId;
                    operacaoParcelaEventoAmortizacao.dt_ocorrencia = dt_proximo_diaUtil;
                    operacaoParcelaEventoAmortizacao.valor = value.OperacaoParcela.vr_amortizacao.Value;
                    operacaoParcelaEventoAmortizacao.ind_operacao = eve.ind_operacao;
                    operacaoParcelaEventoAmortizacao.ind_estorno = "N";
                    operacaoParcelaEventoAmortizacao.ind_tipoEvento = eve.ind_tipoEvento;
                    operacaoParcelaEventoAmortizacao.arquivo = p.parcelaId == 1 ? value.fileComprovante : null; // comprovante
                    #endregion

                    #region Mapear Enquadramento para a Contabilidade
                    if (value.enquadramento_amortizacaoId.HasValue && p.parcelaId == 1)
                    {
                        #region Mapear enquadramento para contabilidade
                        OPEModel eveModel = getModelInstance();
                        eveModel.Create(this.db, this.seguranca_db);

                        operacaoParcelaEventoAmortizacao.Contabilidade = new ContabilidadeViewModel();
                        operacaoParcelaEventoAmortizacao.Contabilidade.ContabilidadeItem = new ContabilidadeItemViewModel() { complementoHist = value.complementoHist };

                        operacaoParcelaEventoAmortizacao.enquadramentoId = value.enquadramento_amortizacaoId.Value;
                        operacaoParcelaEventoAmortizacao = eveModel.BeforeInsert(operacaoParcelaEventoAmortizacao);
                        operacaoParcelaEventoAmortizacao.Contabilidade.dt_lancamento = dt_movto_proximo_diaUtil.Value;
                        operacaoParcelaEventoAmortizacao.Contabilidade.documento = p.num_titulo != null && !String.IsNullOrWhiteSpace(p.num_titulo) ? p.num_titulo : value.documento;
                        #endregion
                    };
                    #endregion

                    #region Gerar Movimentação Bancária
                    MovtoBancarioViewModel movtoViewModel = null;
                    if (p.bancoId.HasValue)
                    {
                        movtoViewModel = new MovtoBancarioViewModel()
                        {
                            empresaId = sessaoCorrente.empresaId,
                            bancoId = p.bancoId.Value,
                            historicoId = value.historicoId,
                            complementoHist = value.complementoHist,
                            dt_movto = dt_movto_proximo_diaUtil.Value,
                            valor = value.OperacaoParcela.vr_amortizacao.Value + (value.OperacaoParcela.vr_encargos.HasValue ? value.OperacaoParcela.vr_encargos.Value : 0) - (value.OperacaoParcela.vr_desconto.HasValue ? value.OperacaoParcela.vr_desconto.Value : 0),
                            documento = p.num_titulo != null && !String.IsNullOrWhiteSpace(p.num_titulo) ? p.num_titulo : value.documento,
                            tipoMovto = getTipoMovto()
                        };
                        operacaoParcelaEventoAmortizacao.MovtoBancario = movtoViewModel;
                    }
                    #endregion
                    p.vr_amortizacao = value.OperacaoParcela.vr_amortizacao.Value + (value.OperacaoParcela.vr_encargos.HasValue ? value.OperacaoParcela.vr_encargos.Value : 0) - (value.OperacaoParcela.vr_desconto.HasValue ? value.OperacaoParcela.vr_desconto.Value : 0); // movtoViewModel.valor;
                }
                #endregion

                value.OperacaoParcelas.ElementAt(indice).OperacaoParcelaEventos = new List<OPERepo>();

                ((List<OPERepo>)value.OperacaoParcelas.ElementAt(indice).OperacaoParcelaEventos).Add(operacaoParcelaEvento);

                if (operacaoParcelaEventoEncargos != null)
                    ((List<OPERepo>)value.OperacaoParcelas.ElementAt(indice).OperacaoParcelaEventos).Add(operacaoParcelaEventoEncargos);

                if (operacaoParcelaEventoMulta != null)
                    ((List<OPERepo>)value.OperacaoParcelas.ElementAt(indice).OperacaoParcelaEventos).Add(operacaoParcelaEventoMulta);

                if (operacaoParcelaEventoDesconto != null)
                    ((List<OPERepo>)value.OperacaoParcelas.ElementAt(indice).OperacaoParcelaEventos).Add(operacaoParcelaEventoDesconto);

                if (operacaoParcelaEventoAmortizacao != null)
                    ((List<OPERepo>)value.OperacaoParcelas.ElementAt(indice).OperacaoParcelaEventos).Add(operacaoParcelaEventoAmortizacao);

                if (operacaoParcelaEventoPagtoEncargos != null)
                    ((List<OPERepo>)value.OperacaoParcelas.ElementAt(indice).OperacaoParcelaEventos).Add(operacaoParcelaEventoPagtoEncargos);

                p.vr_saldo_devedor = p.OperacaoParcelaEventos.Where(info => info.ind_operacao == "C").Select(info => info.valor).Sum() - p.OperacaoParcelaEventos.Where(info => info.ind_operacao == "D").Select(info => info.valor).Sum();

                indice++;
            }

            #endregion

            return value;
        }

        public override ORepo AfterInsert(ORepo value)
        {
            #region Chamar o método AfterInsert dos eventos para mover os arquivos de boleto e comprovante (e fazer a atualização dos saldos)
            try
            {
                OPEModel pevModel = getModelInstance();
                pevModel.Create(this.db, this.seguranca_db);

                foreach (OPRepo par in value.OperacaoParcelas)
                {
                    foreach (OPERepo pev in par.OperacaoParcelaEventos)
                    {
                        OPERepo evm = pevModel.AfterInsert(pev);
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

        public override O MapToEntity(ORepo value)
        {
            O c = Find(value);

            #region Operação
            if (c == null)
            {
                c = getEntityInstance();
                c.OperacaoParcelas = new List<OP>();
            }
            else
                c.OperacaoParcelas.Clear();

            c.operacaoId = value.operacaoId;
            c.empresaId = value.empresaId;
            c.historicoId = value.historicoId;
            c.complementoHist = value.complementoHist;
            c.centroCustoId = value.centroCustoId;
            c.enquadramentoId = value.enquadramentoId;
            c.dt_emissao = value.dt_emissao;
            c.vr_jurosMora = value.vr_jurosMora ?? 0;
            c.vr_multa = value.vr_multa ?? 0;
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

            #region Parcelas da operação
            OPModel parModel = getParcelaModelInstance();
            parModel.Create(this.db, this.seguranca_db);

            foreach (OPRepo p in value.OperacaoParcelas)
            {
                OP par = parModel.MapToEntity(p);
                c.OperacaoParcelas.Add(par);
            }
            #endregion

            return c;
        }

        public override ORepo MapToRepository(O entity)
        {
            #region Operação
            ORepo r = base.CreateRepository();

            r.mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS };
            r.operacaoId = entity.operacaoId;
            r.empresaId = entity.empresaId;
            r.historicoId = entity.historicoId;
            r.descricao_historico = db.Historicos.Find(entity.historicoId).descricao;
            r.complementoHist = entity.complementoHist;
            r.centroCustoId = entity.centroCustoId;
            r.descricao_centroCusto = entity.centroCustoId != null ? db.CentroCustos.Find(entity.centroCustoId).descricao : "";
            r.enquadramentoId = entity.enquadramentoId;
            r.descricao_enquadramento = entity.enquadramentoId != null ? db.Enquadramentos.Find(entity.enquadramentoId).descricao : "";
            r.dt_emissao = entity.dt_emissao;
            r.vr_jurosMora = entity.vr_jurosMora == 0 ? null : entity.vr_jurosMora;
            r.vr_multa = entity.vr_multa == 0 ? null : entity.vr_multa;
            r.documento = entity.documento;
            r.recorrencia = entity.recorrencia;
            r.recorrencia_mensal = entity.recorrencia == "S" ? true : false;
            r.dt_movto = Funcoes.Brasilia().Date;
            r.fileBoleto = "";
            r.num_parcelas = 1;
            r.OperacaoParcelas = new List<OPRepo>();

            #endregion

            #region Contabilidade da operação
            if (entity.Contabilidade != null)
            {
                ContabilidadeModel contModel = new ContabilidadeModel(this.db, this.seguranca_db);
                r.Contabilidade = contModel.MapToRepository(entity.Contabilidade);
            }
            #endregion

            #region Parcelas da operação
            OPModel parModel = getParcelaModelInstance();
            parModel.Create(this.db, this.seguranca_db);

            foreach (OP p in entity.OperacaoParcelas)
            {
                OPRepo par = parModel.MapToRepository(p);
                ((List<OPRepo>)r.OperacaoParcelas).Add(par);
            }
            #endregion

            #region Parcela para liquidação
            r.OperacaoParcela = getOperacaoParcelaRepositoryInstance();

            if (r.OperacaoParcelas.FirstOrDefault().bancoId.HasValue)
                r.OperacaoParcela.bancoId = r.OperacaoParcelas.FirstOrDefault().bancoId;
            r.OperacaoParcela.nome_banco = r.OperacaoParcelas.FirstOrDefault().nome_banco;
            r.OperacaoParcela.ind_forma_pagamento = r.OperacaoParcelas.FirstOrDefault().ind_forma_pagamento ?? "";
            r.OperacaoParcela.dt_vencimento = r.OperacaoParcelas.FirstOrDefault().dt_vencimento;
            r.OperacaoParcela.vr_principal = r.OperacaoParcelas.Sum(info => info.vr_principal);
            //r.OperacaoParcela.vr_principal = r.OperacaoParcelas.Sum(info => info.vr_saldo_devedor).GetValueOrDefault(0);
            r.OperacaoParcela.vr_saldo_devedor = r.OperacaoParcelas.Sum(info => info.vr_saldo_devedor).GetValueOrDefault(0);
            r.OperacaoParcela.dt_baixa = Funcoes.Brasilia().Date;
            r.OperacaoParcela.cheque_agencia = r.OperacaoParcelas.FirstOrDefault().cheque_agencia;
            r.OperacaoParcela.cheque_banco = r.OperacaoParcelas.FirstOrDefault().cheque_banco;
            r.OperacaoParcela.cheque_numero = r.OperacaoParcelas.FirstOrDefault().cheque_numero;
            #endregion

            return r;
        }

        public override Validate Validate(ORepo value, Crud operation)
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
                if (value.historicoId == 0)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.MessageBase = MensagemPadrao.Message(5, "Histórico").ToString();
                    value.mensagem.Message = "Campo obrigatório: Histórico";
                    value.mensagem.MessageType = MsgType.WARNING;

                    return value.mensagem;
                }

                // validar a contabilidade (contas a recar)
                if (value.Contabilidade != null)
                {
                    ContabilidadeModel contabilidadeModel = new ContabilidadeModel(this.db, this.seguranca_db);
                    Validate validate = contabilidadeModel.Validate(value.Contabilidade, Crud.INCLUIR);
                    if (validate.Code > 0)
                        return validate;
                }

                if (value.OperacaoParcelas.Count() == 0)
                {
                    value.mensagem.Code = 46;
                    value.mensagem.MessageBase = MensagemPadrao.Message(46, "parcealas da operação").ToString();
                    value.mensagem.Message = "Deve ser incluído pelo meno uma parcela da operação.";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                // Validar parcelas da operação
                OPModel parModel = getParcelaModelInstance();
                parModel.Create(this.db, this.seguranca_db);
                foreach (OPRepo par in value.OperacaoParcelas)
                {
                    Validate validate = parModel.Validate(par, operation);
                    if (validate.Code > 0)
                        return validate;
                }
            }

            return value.mensagem;
        }

        public override ORepo CreateRepository(System.Web.HttpRequestBase Request = null)
        {
            DateTime d = getDataUltimaInclusao();

            ORepo r = base.CreateRepository(Request);

            if (Request != null && Request["operacaoId"] != null && Request["operacaoId"] != "")
            {
                r.operacaoId = int.Parse(Request["operacaoId"]);
                O entity = Find(r);
                r = MapToRepository(entity);
            }
            else
            {
                r.operacaoId = 0;
                r.empresaId = sessaoCorrente.empresaId;
                r.dt_emissao = d;
                r.recorrencia = "N";
                r.recorrencia_mensal = false;
                r.num_parcelas = 1;
                r.dt_movto = Funcoes.Brasilia().Date;
                r.documento = "";
                r.descricao_historico = "";
                r.complementoHist = "";
                r.descricao_centroCusto = "";
                    
                r.OperacaoParcela = getOperacaoParcelaRepositoryInstance();
                r.OperacaoParcela.ind_forma_pagamento = "4";
                r.OperacaoParcela.dt_vencimento = Funcoes.Brasilia().Date;
                r.OperacaoParcela.dt_baixa = Funcoes.Brasilia().Date;
            }
            r.OperacaoParcela.parcelaId = 1;

            r.OperacaoParcela.OperacaoParcelaEvento = getOperacaoParcelaEventoRepositoryInstance();

            r.OperacaoParcela.OperacaoParcelaEvento.parcelaId = 1;
            r.OperacaoParcela.OperacaoParcelaEvento.dt_ocorrencia = Funcoes.Brasilia().Date;
            r.OperacaoParcela.OperacaoParcelaEvento.dt_movto = Funcoes.Brasilia().Date;

            r.OperacaoParcelas = new List<OPRepo>();

            r.mensagem = new Validate() { Code = 0, Message = "Registro processado com sucesso !" };

            return r;
        }
        #endregion
    }
}