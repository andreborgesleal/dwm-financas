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
    public class ListViewContaPagarAutorizarBI : DWMContext<ApplicationContext>, IProcess<ContaPagarDemonstrativoViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewContaPagarAutorizarBI() { }

        public ListViewContaPagarAutorizarBI(ApplicationContext _db, SecurityContext _seguranca_db)
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

            DateTime proximos5dias = Funcoes.Brasilia().Date.AddDays(5);

            #region LINQ
            var q = (from pag in db.ContaPagars
                     join par in db.ContaPagarParcelas on pag.operacaoId equals par.operacaoId
                     join cre in db.Credores on pag.credorId equals cre.credorId
                     where pag.empresaId.Equals(sessaoCorrente.empresaId)
                           && par.dt_vencimento <= proximos5dias
                           && (par.ind_baixa == null || par.ind_baixa == "")
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
                             vr_saldo_devedor = par.vr_saldo_devedor,
                             ind_autorizacao = par.ind_autorizacao ?? "N"
                         },
                         PageSize = pageSize,
                         TotalAmortizado = (from pag1 in db.ContaPagars
                                            join par1 in db.ContaPagarParcelas on pag1.operacaoId equals par1.operacaoId
                                            join cre1 in db.Credores on pag1.credorId equals cre1.credorId
                                            where pag1.empresaId.Equals(sessaoCorrente.empresaId)
                                                  && par1.dt_vencimento <= proximos5dias
                                                  && (par1.ind_baixa == null || par1.ind_baixa == "")
                                                  && par1.ind_autorizacao == "S"
                                            orderby par1.dt_vencimento // total autorizado para pagamento
                                            select par1).Select(info => info.vr_saldo_devedor).Sum(),
                         TotalEmAberto = (from pag1 in db.ContaPagars
                                          join par1 in db.ContaPagarParcelas on pag1.operacaoId equals par1.operacaoId
                                          join cre1 in db.Credores on pag1.credorId equals cre1.credorId
                                          where pag1.empresaId.Equals(sessaoCorrente.empresaId)
                                                && par1.dt_vencimento <= proximos5dias
                                                && (par1.ind_baixa == null || par1.ind_baixa == "")
                                                && par1.ind_autorizacao != "S"
                                          orderby par1.dt_vencimento
                                          select par1).Select(info => info.vr_saldo_devedor).Sum(), // total pendente para pagamento
                         TotalTitulos = 0,
                         TotalEmitido = (from pag1 in db.ContaPagars
                                         join par1 in db.ContaPagarParcelas on pag1.operacaoId equals par1.operacaoId
                                         join cre1 in db.Credores on pag1.credorId equals cre1.credorId
                                         where pag1.empresaId.Equals(sessaoCorrente.empresaId)
                                               && par1.dt_vencimento <= proximos5dias
                                               && (par1.ind_baixa == null || par1.ind_baixa == "")
                                         orderby par1.dt_vencimento
                                         select par1).Select(info => info.vr_saldo_devedor).Sum(), // total pendente + total autorizado,
                         TotalCount = (from pag1 in db.ContaPagars
                                       join par1 in db.ContaPagarParcelas on pag1.operacaoId equals par1.operacaoId
                                       join cre1 in db.Credores on pag1.credorId equals cre1.credorId
                                       where pag1.empresaId.Equals(sessaoCorrente.empresaId)
                                             && par1.dt_vencimento <= proximos5dias
                                             && (par1.ind_baixa == null || par1.ind_baixa == "")
                                       orderby par1.dt_vencimento
                                       select pag1).Count() // quantidade de títulos pendentes + quantidade de títulos autorizados
                     }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();
            #endregion

            IPagedList pagedList = new PagedList<ContaPagarDemonstrativoViewModel>(q.ToList(), pageIndex, pageSize, q.Count() > 0 ? q.First().TotalCount : 0, "List", null, "div-list-static");

            pagedList.action = "List";
            pagedList.DivId = "div-list-static";

            return pagedList;
        }


    }
}