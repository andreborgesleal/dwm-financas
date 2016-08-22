using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Models;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DWM.Models.BI
{
    public class EditarContaPagarBI : DWMContext<ApplicationContext>, IProcess<EditarContaPagarViewModel, ApplicationContext>
    {
        #region Constructor
        public EditarContaPagarBI() { }

        public EditarContaPagarBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual EditarContaPagarViewModel Run(Repository value)
        {
            EditarContaPagarViewModel r = (EditarContaPagarViewModel)value;
            try
            {
                #region Calcula o próximo dia útil em relação à data de referência
                DateTime? dt_referencia = Funcoes.Brasilia().Date;
                ObterProximoDiaUtil obterProximoDiaUtil = new ObterProximoDiaUtil(this.db, this.seguranca_db, dt_referencia);
                obterProximoDiaUtil.Run(new FeriadoViewModel());
                DateTime dt_proximo_diaUtil = obterProximoDiaUtil.dt_referencia;
                #endregion

                r = (from cp in db.ContaPagars
                     join his in db.Historicos on cp.historicoId equals his.historicoId
                     join cre in db.Credores on cp.credorId equals cre.credorId
                     join gcre in db.GrupoCredores on cre.grupoCredorId equals gcre.grupoCredorId into GCRE
                     from gcre in GCRE.DefaultIfEmpty()
                     join ccu in db.CentroCustos on cp.centroCustoId equals ccu.centroCustoId into CCU
                     from ccu in CCU.DefaultIfEmpty()
                     join par in db.ContaPagarParcelas on cp.operacaoId equals par.operacaoId
                     join ban in db.Bancos on par.bancoId equals ban.bancoId
                     where cp.operacaoId == r.operacaoId
                             && par.parcelaId == r.parcelaId
                     select new EditarContaPagarViewModel()
                     {
                         operacaoId = r.operacaoId,
                         parcelaId = r.parcelaId,
                         dt_emissao = cp.dt_emissao,
                         credorId = cp.credorId,
                         nome_credor = cre.nome,
                         grupoCredorId = gcre.grupoCredorId,
                         descricao_grupoCredor = gcre.nome,
                         historicoId = cp.historicoId,
                         descricao_historico = his.descricao,
                         complementoHist = cp.complementoHist,
                         centroCustoId = cp.centroCustoId,
                         descricao_centroCusto = ccu.descricao,
                         dt_pagamento = dt_proximo_diaUtil,
                         dt_movto = dt_proximo_diaUtil,
                         hasMultaMora = cp.vr_jurosMora > 0 || cp.vr_multa > 0,
                         vr_total = (from cp1 in db.ContaPagarParcelas where cp1.operacaoId == r.operacaoId select cp1.vr_principal).Sum(),
                         qte_parcelas = (from cp2 in db.ContaPagarParcelas where cp2.operacaoId == r.operacaoId select cp2.parcelaId).Count(),
                         Contabilidades = (from con in db.ContabilidadeItems
                                           join pc in db.PlanoContas on con.planoContaId equals pc.planoContaId
                                           where cp.contabilidadeId != null && con.contabilidadeId == cp.contabilidadeId
                                           select new ContabilidadeItemViewModel
                                           {
                                               codigoReduzido = pc.codigoReduzido,
                                               descricao_planoConta = pc.descricao,
                                               tipoLancamento = con.tipoLancamento,
                                               valor = con.valor
                                           }).ToList(),
                         editarContaPagarParcelaEventoViewModel = new EditarContaPagarParcelaEventoViewModel()
                         {
                             historicoId = cp.historicoId,
                             complementoHist = cp.complementoHist,
                             dt_ocorrencia = DateTime.Today
                         },
                         bancoId = par.bancoId,
                         nome_banco = ban.nome,
                         num_titulo = par.num_titulo,
                         dt_vencimento = par.dt_vencimento,
                         vr_principal = par.vr_principal,
                         vr_amortizacao = par.vr_amortizacao,
                         vr_total_pago = par.vr_total_pago,
                         vr_desconto = par.vr_desconto,
                         vr_encargos = par.vr_encargos,
                         vr_saldo_devedor = par.vr_saldo_devedor,
                         vr_multa_atraso_baixa = (from cpeve1 in db.ContaPagarParcelaEventos join eve1 in db.Eventos on cpeve1.eventoId equals eve1.eventoId
                                                  where cpeve1.operacaoId == r.operacaoId && cpeve1.parcelaId == r.parcelaId 
                                                        && eve1.codigo == 10
                                                        && cpeve1.ind_estorno == "N"
                                                  select cpeve1.valor).DefaultIfEmpty(0).Sum(),
                         vr_juros_mora_baixa = (from cpeve2 in db.ContaPagarParcelaEventos
                                                join eve2 in db.Eventos on cpeve2.eventoId equals eve2.eventoId
                                                where cpeve2.operacaoId == r.operacaoId && cpeve2.parcelaId == r.parcelaId
                                                      && eve2.codigo == 1 
                                                      && cpeve2.ind_estorno == "N"
                                                select cpeve2.valor).DefaultIfEmpty(0).Sum(), 
                         vr_baixa = 0,
                         vr_desconto_baixa = 0,
                         ind_forma_pagamento = par.ind_forma_pagamento,
                         codigo_barras = par.codigo_barras,
                         dt_ultima_amortizacao = par.dt_ultima_amortizacao,
                         ind_baixa = par.ind_baixa,
                         dt_baixa = par.dt_baixa,
                         cheque_banco = par.cheque_banco,
                         cheque_agencia = par.cheque_agencia,
                         cheque_numero = par.cheque_numero,
                         ContaPagarParcelaEventos = (from cpeve in db.ContaPagarParcelaEventos
                                                     join eve in db.Eventos on cpeve.eventoId equals eve.eventoId
                                                     where cpeve.operacaoId == r.operacaoId && cpeve.parcelaId == r.parcelaId
                                                     select new ContaPagarParcelaEventoViewModel
                                                     {
                                                         operacaoId = cpeve.operacaoId,
                                                         parcelaId = cpeve.parcelaId,
                                                         dt_evento = cpeve.dt_evento,
                                                         dt_ocorrencia = cpeve.dt_ocorrencia,
                                                         descricao_evento = eve.descricao,
                                                         ind_estorno = cpeve.ind_estorno,
                                                         contabilidadeId = cpeve.contabilidadeId,
                                                         movtoBancarioId = cpeve.movtoBancarioId,
                                                         ind_tipoEvento = cpeve.ind_tipoEvento,
                                                         ind_operacao = cpeve.ind_operacao == "D" ? "Débito" : "Crédito",
                                                         valor = cpeve.valor,
                                                         arquivo = cpeve.arquivo
                                                     }).ToList()

                     }).FirstOrDefault();

                if ((from cpeve3 in db.ContaPagarParcelaEventos
                     join eve3 in db.Eventos on cpeve3.eventoId equals eve3.eventoId
                     where cpeve3.operacaoId == r.operacaoId && cpeve3.parcelaId == r.parcelaId
                           && eve3.codigo == 11
                           && cpeve3.ind_estorno == "N"
                     select cpeve3.valor).DefaultIfEmpty(0).Sum() > 0)
                {
                    r.vr_juros_mora_baixa = (from cpeve2 in db.ContaPagarParcelaEventos
                                             join eve2 in db.Eventos on cpeve2.eventoId equals eve2.eventoId
                                             where cpeve2.operacaoId == r.operacaoId && cpeve2.parcelaId == r.parcelaId
                                                   && (eve2.codigo == 1 || eve2.codigo == 10)
                                                   && cpeve2.ind_estorno == "N"
                                             select cpeve2.valor).DefaultIfEmpty(0).Sum() -
                            (from cpeve3 in db.ContaPagarParcelaEventos
                             join eve3 in db.Eventos on cpeve3.eventoId equals eve3.eventoId
                             where cpeve3.operacaoId == r.operacaoId && cpeve3.parcelaId == r.parcelaId
                                   && eve3.codigo == 11
                                   && cpeve3.ind_estorno == "N"
                             select cpeve3.valor).DefaultIfEmpty(0).Sum();

                    r.vr_multa_atraso_baixa = (from cpeve1 in db.ContaPagarParcelaEventos
                                               join eve1 in db.Eventos on cpeve1.eventoId equals eve1.eventoId
                                               where cpeve1.operacaoId == r.operacaoId && cpeve1.parcelaId == r.parcelaId
                                                     && eve1.codigo == 10
                                                     && cpeve1.ind_estorno == "N"
                                               select cpeve1.valor).DefaultIfEmpty(0).Sum() -
                                                 ((from cpeve3 in db.ContaPagarParcelaEventos
                                                   join eve3 in db.Eventos on cpeve3.eventoId equals eve3.eventoId
                                                   where cpeve3.operacaoId == r.operacaoId && cpeve3.parcelaId == r.parcelaId
                                                         && eve3.codigo == 11
                                                         && cpeve3.ind_estorno == "N"
                                                   select cpeve3.valor).DefaultIfEmpty(0).Sum() -
                                                  (from cpeve4 in db.ContaPagarParcelaEventos
                                                   join eve4 in db.Eventos on cpeve4.eventoId equals eve4.eventoId
                                                   where cpeve4.operacaoId == r.operacaoId && cpeve4.parcelaId == r.parcelaId
                                                         && eve4.codigo == 1
                                                         && cpeve4.ind_estorno == "N"
                                                   select cpeve4.valor).DefaultIfEmpty(0).Sum()) -
                                               r.vr_juros_mora_baixa;
                }

                r.vr_multa_atraso = r.vr_multa_atraso ?? 0;
            }
            catch (Exception ex)
            {
                r.mensagem = new Validate() { Code = 999, MessageBase = ex.Message, Message = "Ocorreu um erro na edição da comissão" };
            }

            return r;
        }

        public IEnumerable<EditarContaPagarViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }

    }
}