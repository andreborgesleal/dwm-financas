using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Models;
using DWM.Models.Entidades;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DWM.Models.BI
{
    public class ListViewContaPagarBI : DWMContext<ApplicationContext>, IProcess<EditarContaPagarViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewContaPagarBI() { }

        public ListViewContaPagarBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual EditarContaPagarViewModel Run(Repository value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EditarContaPagarViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            int pageIndex = index ?? 0;

            #region Parâmetros
            int? credorId = (int?)param[0];
            DateTime? dt_emissao1 = (DateTime?)param[1];
            DateTime? dt_emissao2 = (DateTime?)param[2];
            #endregion

            DateTime hoje = Funcoes.Brasilia().Date; // 

            #region LINQ
            var q = (from pag in db.ContaPagars
                     join cre in db.Credores on pag.credorId equals cre.credorId
                     join his in db.Historicos on pag.historicoId equals his.historicoId
                     join par in db.ContaPagarParcelas on pag.operacaoId equals par.operacaoId
                     group par by new
                     {
                         pag.empresaId,
                         par.operacaoId,
                         pag.dt_emissao,
                         cre.credorId,
                         cre.nome,
                         his.historicoId,
                         pag.complementoHist,
                         his.descricao,
                     } into PAR
                     where (PAR.Key.empresaId == sessaoCorrente.empresaId
                            && !credorId.HasValue || PAR.Key.credorId == credorId)
                            && (!dt_emissao1.HasValue || PAR.Key.dt_emissao >= dt_emissao1 && PAR.Key.dt_emissao <= dt_emissao2)
                     orderby PAR.Key.nome, PAR.Key.operacaoId
                     select new EditarContaPagarViewModel
                     {
                         operacaoId = PAR.Key.operacaoId,
                         empresaId = PAR.Key.empresaId,
                         nome_credor = PAR.Key.nome,
                         descricao_historico = PAR.Key.descricao,
                         complementoHist = PAR.Key.complementoHist,
                         dt_emissao = PAR.Key.dt_emissao,
                         dt_baixa = PAR.Max(info => info.dt_baixa),
                         dt_ultima_amortizacao = PAR.Max(info => info.dt_ultima_amortizacao),
                         vr_principal = PAR.Sum(info => info.vr_principal),
                         vr_encargos = PAR.Sum(info => info.vr_encargos),
                         vr_desconto = PAR.Sum(info => info.vr_desconto),
                         vr_amortizacao = PAR.Sum(info => info.vr_amortizacao),
                         vr_total_pago = PAR.Sum(info => info.vr_total_pago),
                         vr_saldo_devedor = PAR.Sum(info => info.vr_saldo_devedor),
                         PageSize = pageSize,
                         TotalCount = (from pag1 in db.ContaPagars
                                       join cre1 in db.Credores on pag1.credorId equals cre1.credorId
                                       join his1 in db.Historicos on pag1.historicoId equals his1.historicoId
                                       join par1 in db.ContaPagarParcelas on pag1.operacaoId equals par1.operacaoId
                                       group par1 by new
                                       {
                                           pag1.empresaId,
                                           par1.operacaoId,
                                           pag1.dt_emissao,
                                           cre1.credorId,
                                           cre1.nome,
                                           his1.historicoId,
                                           pag1.complementoHist,
                                           his1.descricao,
                                       } into PAR1
                                       where (PAR1.Key.empresaId == sessaoCorrente.empresaId
                                              && !credorId.HasValue || PAR1.Key.credorId == credorId)
                                              && (!dt_emissao1.HasValue || PAR1.Key.dt_emissao >= dt_emissao1 && PAR1.Key.dt_emissao <= dt_emissao2)
                                       orderby PAR1.Key.nome, PAR1.Key.operacaoId
                                       select PAR1).Count()
                     }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();
            #endregion

            return new PagedList<EditarContaPagarViewModel>(q.ToList(), pageIndex, pageSize, q.Count() > 0 ? q.First().TotalCount : 0, "ListOperacaoParam", null, "div-list-static");
        }

    }

    public class ListViewContaPagarDemonstrativoBI : DWMContext<ApplicationContext>, IProcess<ContaPagarDemonstrativoViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewContaPagarDemonstrativoBI() { }

        public ListViewContaPagarDemonstrativoBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual ContaPagarDemonstrativoViewModel Run(Repository value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContaPagarDemonstrativoViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            int pageIndex = index ?? 0;

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
            var q = (from pag in db.ContaPagars
                     join par in db.ContaPagarParcelas on pag.operacaoId equals par.operacaoId
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
                     select new ContaPagarDemonstrativoViewModel
                     {
                         operacaoId = pag.operacaoId,
                         empresaId = pag.empresaId,
                         nome_credor = cre.nome,
                         dt_emissao = pag.dt_emissao,
                         documento = pag.documento,
                         recorrencia = pag.recorrencia,
                         OperacaoParcela = new ContaPagarParcelaViewModel()
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
                         TotalAmortizado = (from pag1 in db.ContaPagars
                                            join par1 in db.ContaPagarParcelas on pag1.operacaoId equals par1.operacaoId
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
                                            select par1).Select(info => info.vr_amortizacao).Sum(),
                         TotalEmAberto = (from pag1 in db.ContaPagars
                                          join par1 in db.ContaPagarParcelas on pag1.operacaoId equals par1.operacaoId
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
                                          select par1).Select(info => info.vr_saldo_devedor).Sum(),
                         TotalTitulos = 0,
                         TotalEmitido = (from pag1 in db.ContaPagars
                                         join par1 in db.ContaPagarParcelas on pag1.operacaoId equals par1.operacaoId
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
                                         select par1).Select(info => info.vr_principal).Sum(),
                         TotalCount = (from pag1 in db.ContaPagars
                                       join par1 in db.ContaPagarParcelas on pag1.operacaoId equals par1.operacaoId
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

            IPagedList pagedList = new PagedList<ContaPagarDemonstrativoViewModel>(q.ToList(), pageIndex, pageSize, q.Count() > 0 ? q.First().TotalCount : 0, "ListPanorama", null, "div-list-static");

            pagedList.action = "ListParam";
            pagedList.DivId = "div-list-static";

            return pagedList;
        }
    }

}