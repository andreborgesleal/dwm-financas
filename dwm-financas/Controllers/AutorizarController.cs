using App_Dominio.Contratos;
using App_Dominio.Controllers;
using App_Dominio.Security;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using DWM.Models.BI;
using App_Dominio.Pattern;
using DWM.Models.Entidades;

namespace DWM.Controllers
{
    public class AutorizarController : RootController<ContaPagarParcelaViewModel, ContaPagarParcelaCrudModel>
    {
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override string getListName()
        {
            return "Listar Pagamentos Pendentes";
        }

        #region List
        [AuthorizeFilter]
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                ListViewContaPagarAutorizarBI list = new ListViewContaPagarAutorizarBI();
                return this._List(index, pageSize, "Browse", list);
            }
            else
                return View();
        }

        public ActionResult _List(int? index, int? pageSize, string action, ListViewContaPagarAutorizarBI model)
        {
            if (ViewBag.ValidateRequest)
            {
                Factory<ContaPagarDemonstrativoViewModel, ApplicationContext> facade = new Factory<ContaPagarDemonstrativoViewModel, ApplicationContext>();
                IPagedList pagedList = facade.PagedList(model, index, pageSize.Value);
                UpdateBreadCrumb(this.ControllerContext.RouteData.Values["controller"].ToString(), action);
                return View(pagedList);
            }
            else
                return null;
        }
        #endregion

        #region Autorizar
        [AuthorizeFilter]
        public ActionResult Edit(int operacaoId, int parcelaId)
        {
            if (ViewBag.ValidateRequest)
            {
                Factory<ContaPagarParcelaViewModel, ApplicationContext> factory = new Factory<ContaPagarParcelaViewModel, ApplicationContext>();
                IPagedList pagedList = factory.Execute(new ContaPagarAutorizarBI(), 0, 100, 
                    new ContaPagarParcelaViewModel() { operacaoId = operacaoId,
                                                       parcelaId = parcelaId, ind_autorizacao = "S",
                                                       uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString()} );
                UpdateBreadCrumb(this.ControllerContext.RouteData.Values["controller"].ToString(), "Browse");
                return View("List",pagedList);
            }
            else
                return null;
        }

        #endregion

        #region Revogar
        [AuthorizeFilter]
        public ActionResult Delete(int operacaoId, int parcelaId)
        {
            if (ViewBag.ValidateRequest)
            {
                Factory<ContaPagarParcelaViewModel, ApplicationContext> factory = new Factory<ContaPagarParcelaViewModel, ApplicationContext>();
                IPagedList pagedList = factory.Execute(new ContaPagarAutorizarBI(), 0, 100,
                    new ContaPagarParcelaViewModel()
                    {
                        operacaoId = operacaoId,
                        parcelaId = parcelaId,
                        ind_autorizacao = "N",
                        uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString()
                    });
                UpdateBreadCrumb(this.ControllerContext.RouteData.Values["controller"].ToString(), "Browse");
                return View("List", pagedList);
            }
            else
                return null;
        }
        #endregion
    }
}