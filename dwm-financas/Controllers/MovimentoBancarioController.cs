using App_Dominio.Controllers;
using App_Dominio.Security;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Web.Mvc;
using App_Dominio.Models;
using DWM.Models.Entidades;

namespace DWM.Controllers
{
    public class MovimentoBancarioController : DwmRootController<MovtoBancarioViewModel,MovtoBancarioModel,ApplicationContext>
    {
        #region Herança
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override bool mustListOnLoad()
        {
            return false;
        }
        public override string getListName()
        {
            return "Listar Movimentação Bancária";
        }
        #endregion

        #region List
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {
            return ListParam(index, PageSize, Request["bancoId"], Request["data1"], Request["data2"]);
        }

        [AuthorizeFilter]
        public ActionResult ListParam(int? index, int? pageSize = 50, string bancoId = null,
                                        string data1 = null, string data2 = null)
        {
            ViewBag.ValidateRequest = true;
            if (ViewBag.ValidateRequest)
            {
                int? _null = null;
                int? _bancoId = bancoId != null && bancoId != "" ? int.Parse(bancoId) : _null;
                DateTime _data1 = data1 == null || data1 == "" ? new DateTime(Funcoes.Brasilia().Year, Funcoes.Brasilia().Month, 1) : Convert.ToDateTime(data1);
                DateTime _data2 = data2 == null || data2 == "" ? new DateTime(Funcoes.Brasilia().Year, Funcoes.Brasilia().Month, Funcoes.Brasilia().Day) : Convert.ToDateTime(data2);
                ListViewMovtoBancario list = new ListViewMovtoBancario();
                return this._List(index, pageSize, "Browse", list, _bancoId, _data1, _data2);
            }
            else
                return View();
        }
        #endregion

        #region Edit
        [AuthorizeFilter]
        public ActionResult Edit(int movtoBancarioId)
        {
            return _Edit(new MovtoBancarioViewModel() { movtoBancarioId = movtoBancarioId });
        }

        public override void BeforeEdit(ref MovtoBancarioViewModel value, FormCollection collection)
        {
            base.BeforeEdit(ref value, collection);
            if (collection["valor.1"] != null && collection["valor.1"] != null)
                value.valor = decimal.Parse(collection["valor.1"]);
        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult Delete(int movtoBancarioId)
        {
            return Edit(movtoBancarioId);
        }
        #endregion
    }
}