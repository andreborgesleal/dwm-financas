using App_Dominio.Controllers;
using DWM.Models.Repositories;
using System;
using System.Web.Mvc;
//using Microsoft.Reporting.WebForms;
using DWM.Models.Enumeracoes;
using DWM.Models.Report;
using App_Dominio.Security;
using App_Dominio.Models;
using App_Dominio.Pattern;
using DWM.Models.Entidades;
using DWM.Models.BI;
using DWM.Models.Persistence;

namespace DWM.Controllers
{
    public class BalanceteController : ReportController<BalanceteViewModel>
    {
        #region Herança
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }

        public override bool mustListOnLoad()
        {
            Factory<ExercicioViewModel, ApplicationContext> facade = new Factory<ExercicioViewModel, ApplicationContext>();
            ExercicioViewModel e = facade.Execute(new ExercicioBI(), new ExercicioViewModel());

            if (e.mensagem.Code == 0 && e.dt_lancamento_inicio.HasValue)
            {
                ViewData["dt_lancamento_inicio"] = e.dt_lancamento_inicio.Value;
                ViewData["dt_lancamento_fim"] = e.dt_lancamento_fim.Value;
            }
            else
            {
                ViewData["dt_lancamento_inicio"] = Convert.ToDateTime(DateTime.Today.ToString("yyyy-MM-") + "01");
                ViewData["dt_lancamento_fim"] = Convert.ToDateTime(DateTime.Today.AddMonths(1).ToString("yyyy-MM-") + "01").AddDays(-1);
            }

            return false;
        }

        public override string getListName()
        {
            return "Balancete";
        }

        //public override bool ClearBreadCrumbOnBrowse()
        //{
        //    return Request["id"] == null;
        //}
        #endregion    

        #region List
        public override ActionResult List(int? index, int? PageSize, string descricao = null)
        {
            return ListParam(index, PageSize);
        }

        [AuthorizeFilter]
        public ActionResult ListParam(int? index, int? pageSize = 100, string data1 = "", string data2 = "",
                                        int centroCustoId = 0, int grauPC = 0)
        {
            if (ViewBag.ValidateRequest)
            {
                Factory<ExercicioViewModel, ApplicationContext> facade = new Factory<ExercicioViewModel, ApplicationContext>();
                ExercicioViewModel e = facade.Execute(new ExercicioBI(), new ExercicioViewModel());

                if (data1 == "" || data1.Contains("%"))
                {

                    if (e.mensagem.Code == 0 && e.dt_lancamento_inicio.HasValue)
                    {
                        data1 = e.dt_lancamento_inicio.Value.ToString("dd/MM/yyyy");
                        data1 = e.dt_lancamento_fim.Value.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        data1 = DateTime.Today.ToString("yyyy-MM-") + "01";
                        data2 = Convert.ToDateTime(DateTime.Today.AddMonths(1).ToString("yyyy-MM-") + "01").AddDays(-1).ToString("yyyy-MM-dd");
                    }
                }

                ListViewBalanceteBI bal = new ListViewBalanceteBI();
                return this._List(index, 
                                    pageSize, 
                                    "Browse", 
                                    bal, 
                                    centroCustoId, 
                                    e.exercicio,
                                    grauPC,
                                    data1, 
                                    data2);
            }
            else
                return View();
        }

        public ActionResult _List(int? index, int? pageSize, string action, ListViewBalanceteBI model, params object[] param)
        {
            if (ViewBag.ValidateRequest)
            {
                Factory<BalanceteViewModel, ApplicationContext> facade = new Factory<BalanceteViewModel, ApplicationContext>();
                IPagedList pagedList = facade.PagedList(model, index, pageSize.Value, param);
                UpdateBreadCrumb(this.ControllerContext.RouteData.Values["controller"].ToString(), action);
                return View(pagedList);
            }
            else
                return null;
        }
        #endregion
    }
}