using App_Dominio.Controllers;
using App_Dominio.Security;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System.Web.Mvc;

namespace DWM.Controllers
{
    public class EventosController : RootController<EventoViewModel, EventoModel>
    {
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override string getListName()
        {
            return "Listar Eventos";
        }

        #region List
        [AuthorizeFilter]
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                ListViewEvento l = new ListViewEvento();
                return this._List(index, pageSize, "Browse", l, descricao);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult _ListEventoModal(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                LookupEventoFiltroModel l = new LookupEventoFiltroModel();
                return this.ListModal(index, pageSize, l, "Descrição", descricao);
            }
            else
                return View();
        }

        #endregion

        #region edit
        [AuthorizeFilter]
        public ActionResult Edit(int eventoId)
        {
            return _Edit(new EventoViewModel() { eventoId = eventoId });
        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult Delete(int eventoId)
        {
            return Edit(eventoId);
        }
        #endregion

        public JsonResult getNames()
        {
            return JSonTypeahead(null, new ListViewEvento());
        }

    }
}