using App_Dominio.Controllers;
using DWM.Models.Repositories;
using System;
using System.Web.Mvc;
using App_Dominio.Security;
using App_Dominio.Pattern;
using DWM.Models.Entidades;
using DWM.Models.BI;
using System.Collections.Generic;
using App_Dominio.Repositories;
using App_Dominio.Entidades;

namespace DWM.Controllers
{
    public class BalanceteMensalController : ReportController<BalanceteMensalViewModel>
    {
        #region Herança
        public override int _sistema_id() { return (int)DWM.Models.Enumeracoes.Sistema.DWMFINANCAS; }

        public override bool mustListOnLoad()
        {
            if (Request["TipoBalanceteMensal"] != null && Request["TipoBalanceteMensal"] != "")
                return true;
            else
                return false;
        }

        public override string getListName()
        {
            return "Balancete Mensal";
        }

        //public override bool ClearBreadCrumbOnBrowse()
        //{
        //    return Request["id"] == null;
        //}
        #endregion    

        #region List
        public override ActionResult List(int? index, int? PageSize, string descricao = null)
        {
            if (Request["TipoBalanceteMensal"] != null && Request["TipoBalanceteMensal"] != "")
                return ListParam(0, 1000, Request["TipoBalanceteMensal"], 
                                Request["centroCustoId"] != null && Request["centroCustoId"] != "" ? int.Parse(Request["centroCustoid"]) : 0,
                                Request["grauPC"] != null && Request["grauPC"] != "" ? int.Parse(Request["grauPC"]) : 0,
                                Request["descricao_centroCusto"], Request["RecDesp"]);
            else
                return ListParam(0, 1000);
        }

        [AuthorizeFilter]
        public ActionResult ListParam(int? index, int? pageSize = 100, string TipoBalanceteMensal = "T", 
                                        int centroCustoId = 0, int grauPC = 0, string descricao_centroCusto = "", string RecDesp = "N")
        {
            if (ViewBag.ValidateRequest)
            {
                Factory<ExercicioViewModel, ApplicationContext> facade = new Factory<ExercicioViewModel, ApplicationContext>();
                ExercicioViewModel e = facade.Execute(new ExercicioBI(), new ExercicioViewModel());

                ListViewBalanceteMensalBI bal = new ListViewBalanceteMensalBI();
                return this._List(index,
                                    pageSize,
                                    "Browse",
                                    bal,
                                    TipoBalanceteMensal,
                                    centroCustoId,
                                    e.exercicio,
                                    grauPC,
                                    descricao_centroCusto,
                                    RecDesp);
            }
            else
                return View();
        }

        public ActionResult _List(int? index, int? pageSize, string action, ListViewBalanceteMensalBI model, params object[] param)
        {
            if (ViewBag.ValidateRequest)
            {
                Factory<BalanceteMensalViewModel, ApplicationContext> facade = new Factory<BalanceteMensalViewModel, ApplicationContext>();
                IPagedList pagedList = facade.PagedList(model, index, pageSize.Value, param);
                UpdateBreadCrumb(this.ControllerContext.RouteData.Values["controller"].ToString(), action);

                #region Filtros
                pagedList.Filtros = new List<FiltroRepository>();

                #region Tipo de Balancete (S-Saldos ou T-Totais de Débito e Crédito)
                FiltroRepository TipoBalanceteMensalFiltro = new FiltroRepository()
                {
                    atributo = "TipoBalanceteMensal",
                    valor = param[0].ToString()
                };
                ((IList<FiltroRepository>)pagedList.Filtros).Add(TipoBalanceteMensalFiltro);
                #endregion

                #region Centro de Custo
                FiltroRepository CentroCustoFiltro = new FiltroRepository()
                {
                    atributo = "centroCustoId",
                    valor = param[1].ToString()
                };
                ((IList<FiltroRepository>)pagedList.Filtros).Add(CentroCustoFiltro);
                #endregion

                #region Exercicio
                FiltroRepository ExercicioFiltro = new FiltroRepository()
                {
                    atributo = "centroCustoId",
                    valor = param[2].ToString()
                };
                ((IList<FiltroRepository>)pagedList.Filtros).Add(ExercicioFiltro);
                #endregion

                #region GrauPC
                FiltroRepository GrauPCFiltro = new FiltroRepository()
                {
                    atributo = "centroCustoId",
                    valor = param[3].ToString()
                };
                ((IList<FiltroRepository>)pagedList.Filtros).Add(GrauPCFiltro);
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
                string descricao_centroCusto = (string)param[4];

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
                    valor = (string)param[5]
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
        public ActionResult Imprimir(string TipoBalanceteMensal = "", int centroCustoId = 0, int grauPC = 0, string descricao_centroCusto = "", string RecDesp = "N")
        {
            return ListParam(0, 10000, TipoBalanceteMensal, centroCustoId, grauPC, descricao_centroCusto, RecDesp);
        }
        #endregion

    }
}