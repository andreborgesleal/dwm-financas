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
using App_Dominio.Repositories;
using System.Collections.Generic;
using App_Dominio.Entidades;

namespace DWM.Controllers
{
    public class BalanceteController : ReportController<BalanceteViewModel>
    {
        #region Herança
        public override int _sistema_id() { return (int)DWM.Models.Enumeracoes.Sistema.DWMFINANCAS; }

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

            return Request ["data1"] != null && Request ["data1"] != "";
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
            if (Request["data1"] != null && Request["data1"] != "")
                return ListParam(index, PageSize, Request ["data1"], Request["data2"], 
                                Request["centroCustoId"] != null && Request["centroCustoId"] != "" ? int.Parse(Request["centroCustoid"]) : 0, 
                                Request["grauPC"] != null && Request["grauPC"] != "" ? int.Parse(Request["grauPC"]) : 0, 
                                Request["descricao_centroCusto"], Request["RecDesp"]);
            else
                return ListParam(index, PageSize);
        }

        [AuthorizeFilter]
        public ActionResult ListParam(int? index, int? pageSize = 100, string data1 = "", string data2 = "",
                                        int centroCustoId = 0, int grauPC = 0, string descricao_centroCusto = "", string RecDesp = "N")
        {
            if (ViewBag.ValidateRequest)
            {
                Factory<ExercicioViewModel, ApplicationContext> facade = new Factory<ExercicioViewModel, ApplicationContext>();
                ExercicioViewModel e = facade.Execute(new ExercicioBI(), new ExercicioViewModel());

                if (data1 == "" || data1.Contains("%"))
                {

                    if (e.mensagem.Code == 0 && e.dt_lancamento_inicio.HasValue)
                    {
                        data1 = e.dt_lancamento_inicio.Value.ToString("yyyy-MM-dd");
                        data2 = e.dt_lancamento_fim.Value.ToString("yyyy-MM-dd");
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
                                    data2,
                                    descricao_centroCusto,
                                    RecDesp);
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

                #region Filtros
                pagedList.Filtros = new List<FiltroRepository>();

                #region Centro de Custo
                FiltroRepository CentroCustoFiltro = new FiltroRepository()
                {
                    atributo = "centroCustoId",
                    valor = param[0].ToString()
                };
                ((IList<FiltroRepository>)pagedList.Filtros).Add(CentroCustoFiltro);
                #endregion

                #region Exercicio
                FiltroRepository ExercicioFiltro = new FiltroRepository()
                {
                    atributo = "centroCustoId",
                    valor = param[1].ToString()
                };
                ((IList<FiltroRepository>)pagedList.Filtros).Add(ExercicioFiltro);
                #endregion

                #region GrauPC
                FiltroRepository GrauPCFiltro = new FiltroRepository()
                {
                    atributo = "centroCustoId",
                    valor = param[2].ToString()
                };
                ((IList<FiltroRepository>)pagedList.Filtros).Add(GrauPCFiltro);
                #endregion

                #region Data1
                FiltroRepository Data1Filtro = new FiltroRepository()
                {
                    atributo = "centroCustoId",
                    valor = (string)param[3]
                };
                ((IList<FiltroRepository>)pagedList.Filtros).Add(Data1Filtro);
                #endregion

                #region Data2
                FiltroRepository Data2Filtro = new FiltroRepository()
                {
                    atributo = "centroCustoId",
                    valor = (string)param[4]
                };
                ((IList<FiltroRepository>)pagedList.Filtros).Add(Data2Filtro);
                #endregion

                #region Empresa
                EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();
                FiltroRepository EmpresaFiltro = new FiltroRepository()
                {
                    atributo = "empresa",
                    valor = security.getEmpresa().nome
                };
                ((IList<FiltroRepository>)pagedList.Filtros).Add(EmpresaFiltro);
                #endregion

                #region Descrição Centro de Custo
                string descricao_centroCusto = (string)param[5];

                FiltroRepository DescricaoCentroCustoFiltro = new FiltroRepository()
                {
                    atributo = "DescricaoCentroCustoFiltro",
                    valor = String.IsNullOrEmpty(descricao_centroCusto) ? "<< Todos os centros de custos >>" : descricao_centroCusto
                };
                ((IList<FiltroRepository>)pagedList.Filtros).Add(DescricaoCentroCustoFiltro);
                #endregion

                #region Receitas e Despesas
                FiltroRepository RecDespFiltro = new FiltroRepository()
                {
                    atributo = "RecDesp",
                    valor = (string)param[6]
                };
                ((IList<FiltroRepository>)pagedList.Filtros).Add(RecDespFiltro);
                #endregion
                #endregion

                return View(pagedList);
            }
            else
                return null;
        }
        #endregion

        #region Imprimir
        [AuthorizeFilter]
        public ActionResult Imprimir(string data1 = "", string data2 = "", int centroCustoId = 0, int grauPC = 0, string descricao_centroCusto = "", string RecDesp = "N")
        {
            return ListParam(0, 10000, data1, data2, centroCustoId, grauPC, descricao_centroCusto, RecDesp);
        }
        #endregion

        #region Lucro/Prejuízo
        [AuthorizeFilter]
        public ActionResult LucroPrejuizo(string data1 = "", string data2 = "", int centroCustoId = 0)
        {
            return ListParam(0, 10000, data1, data2, centroCustoId, 1, "", "S");
        }
        #endregion
    }
}