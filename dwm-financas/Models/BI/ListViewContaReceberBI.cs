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
    public class ListViewContaReceberBI : DWMContext<ApplicationContext>, IProcess<EditarContaReceberViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewContaReceberBI() { }

        public ListViewContaReceberBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual EditarContaReceberViewModel Run(Repository value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EditarContaReceberViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int receSize = 50, params object[] param)
        {
            int receIndex = index ?? 0;

            #region Parâmetros
            int? clienteId = (int?)param[0];
            DateTime? dt_emissao1 = (DateTime?)param[1];
            DateTime? dt_emissao2 = (DateTime?)param[2];
            #endregion

            DateTime hoje = Funcoes.Brasilia().Date;

            #region LINQ
            var q = (from rec in db.ContaRecebers
                     join cli in db.Clientes on rec.clienteId equals cli.clienteId
                     join his in db.Historicos on rec.historicoId equals his.historicoId
                     join par in db.ContaReceberParcelas on rec.operacaoId equals par.operacaoId
                     group par by new
                     {
                         rec.empresaId,
                         par.operacaoId,
                         rec.dt_emissao,
                         cli.clienteId,
                         cli.nome,
                         his.historicoId,
                         rec.complementoHist,
                         his.descricao,
                     } into PAR
                     where (PAR.Key.empresaId == sessaoCorrente.empresaId
                            && !clienteId.HasValue || PAR.Key.clienteId == clienteId)
                            && (!dt_emissao1.HasValue || PAR.Key.dt_emissao >= dt_emissao1 && PAR.Key.dt_emissao <= dt_emissao2)
                     orderby PAR.Key.nome, PAR.Key.operacaoId
                     select new EditarContaReceberViewModel
                     {
                         operacaoId = PAR.Key.operacaoId,
                         empresaId = PAR.Key.empresaId,
                         nome_cliente = PAR.Key.nome,
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
                         PageSize = receSize,
                         TotalCount = (from rec1 in db.ContaRecebers
                                       join cli1 in db.Clientes on rec1.clienteId equals cli1.clienteId
                                       join his1 in db.Historicos on rec1.historicoId equals his1.historicoId
                                       join par1 in db.ContaReceberParcelas on rec1.operacaoId equals par1.operacaoId
                                       group par1 by new
                                       {
                                           rec1.empresaId,
                                           par1.operacaoId,
                                           rec1.dt_emissao,
                                           cli1.clienteId,
                                           cli1.nome,
                                           his1.historicoId,
                                           rec1.complementoHist,
                                           his1.descricao,
                                       } into PAR1
                                       where (PAR1.Key.empresaId == sessaoCorrente.empresaId
                                              && !clienteId.HasValue || PAR1.Key.clienteId == clienteId)
                                              && (!dt_emissao1.HasValue || PAR1.Key.dt_emissao >= dt_emissao1 && PAR1.Key.dt_emissao <= dt_emissao2)
                                       orderby PAR1.Key.nome, PAR1.Key.operacaoId
                                       select PAR1).Count()
                     }).Skip((index ?? 0) * receSize).Take(receSize).ToList();
            #endregion

            return new PagedList<EditarContaReceberViewModel>(q.ToList(), receIndex, receSize, q.Count() > 0 ? q.First().TotalCount : 0, "ListOperacaoParam", null, "div-list-static");
        }

    }
}