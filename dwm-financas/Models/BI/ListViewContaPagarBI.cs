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
}