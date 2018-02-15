using App_Dominio.Controllers;
using App_Dominio.Security;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Web.Mvc;
using App_Dominio.Models;
using System.Collections.Generic;
using System.Linq;
using DWM.Models.Entidades;
using App_Dominio.Pattern;
using App_Dominio.Enumeracoes;
using DWM.Models.BI;

namespace DWM.Controllers
{
    public class CNABController : DwmRootController<TituloViewModel, TituloModel, ApplicationContext>
    {
        #region Herança
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override bool mustListOnLoad()
        {
            return true;
        }
        public override string getListName()
        {
            return "Listar Títulos";
        }
        #endregion

        #region List
        [AuthorizeFilter]
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                ListViewModelConvenio l = new ListViewModelConvenio();
                return this._List(index, pageSize, "Browse", l, descricao);
            }
            else
                return View();
        }
        #endregion

        public override void BeforeCreate(ref TituloViewModel value, FormCollection collection)
        {
            base.BeforeCreate(ref value, collection);
        }
    }
}