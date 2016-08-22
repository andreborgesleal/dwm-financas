using App_Dominio.Controllers;
using App_Dominio.Security;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System.Web.Mvc;

namespace DWM.Controllers
{
    public class GrupoClientesController : RootController<GrupoClienteViewModel, GrupoClienteModel>
    {
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override string getListName()
        {
            return "Listar Grupos de Clientes";
        }

        #region List
        [AuthorizeFilter]
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                ListViewGrupoCliente l = new ListViewGrupoCliente();
                return this._List(index, pageSize, "Browse", l, descricao);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult _ListGrupoClienteModal(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                LookupGrupoClienteFiltroModel l = new LookupGrupoClienteFiltroModel();
                return this.ListModal(index, pageSize, l, "Descrição", descricao);
            }
            else
                return View();
        }

        #endregion

        #region edit
        [AuthorizeFilter]
        public ActionResult Edit(int grupoClienteId)
        {
            return _Edit(new GrupoClienteViewModel() { grupoClienteId = grupoClienteId });
        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult Delete(int grupoClienteId)
        {
            return Edit(grupoClienteId);
        }
        #endregion

        #region CrudGrupoClienteModal
        public JsonResult CrudGrupoClienteModal(string descricao)
        {
            return JSonCrud(new GrupoClienteViewModel() { nome = descricao });
        }
        #endregion

        public JsonResult getNames()
        {
            return JSonTypeahead(null, new ListViewGrupoCliente());
        }
    }
}