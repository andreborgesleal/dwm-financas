using App_Dominio.Controllers;
using App_Dominio.Security;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System.Web.Mvc;

namespace DWM.Controllers
{
    public class CentroCustosController : RootController<CentroCustoViewModel, CentroCustoModel>
    {
        public override int _sistema_id() { return (int) Sistema.DWMFINANCAS; }
        public override string getListName() 
        {
            return "Listar Centros de Custos";
        }

        #region List
        [AuthorizeFilter]
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                ListViewCentroCusto c = new ListViewCentroCusto();
                return this._List(index, pageSize, "Browse", c, descricao);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult _ListCentroCustoModal(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                LookupCentroCustoFiltroModel l = new LookupCentroCustoFiltroModel();
                return this.ListModal(index, pageSize, l, "Descrição", descricao);
            }
            else
                return View();
        }
        #endregion

        #region Edit
        [AuthorizeFilter]
        public ActionResult Edit(int centroCustoId)
        {
            return _Edit(new CentroCustoViewModel() { centroCustoId = centroCustoId });
        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult Delete(int centroCustoId)
        {
            return Edit(centroCustoId);
        }
        #endregion

        #region CrudCentroCustoModal
        public JsonResult CrudCentroCustoModal(string descricao)
        {
            return JSonCrud(new CentroCustoViewModel() { descricao = descricao });
        }
        #endregion

        public JsonResult getNames()
        {
            return JSonTypeahead(null, new ListViewCentroCusto());
        }
    }
}