using App_Dominio.Controllers;
using App_Dominio.Security;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System.Web.Mvc;

namespace DWM.Controllers
{
    public class PlanoContasController : RootController<PlanoContaViewModel, PlanoContaModel>
    {
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override string getListName()
        {
            return "Listar Plano de Contas";
        }

        #region List
        [AuthorizeFilter]
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                ListViewPlanoConta l = new ListViewPlanoConta();
                return this._List(index, pageSize, "Browse", l, descricao);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult _ListPlanoContaPaiModal(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                LookupPlanoContaPaiFiltroModel l = new LookupPlanoContaPaiFiltroModel();
                return this.ListModal(index, pageSize, l, "Descrição", descricao);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult _ListPlanoContaModal(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                LookupPlanoContaFiltroModel l = new LookupPlanoContaFiltroModel();
                return this.ListModal(index, pageSize, l, "Descrição", descricao);
            }
            else
                return View();
        }

        #endregion

        #region edit
        [AuthorizeFilter]
        public ActionResult Edit(int planoContaId)
        {
            return _Edit(new PlanoContaViewModel() { planoContaId = planoContaId });
        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult Delete(int planoContaId)
        {
            return Edit(planoContaId);
        }
        #endregion

        public JsonResult getNames()
        {
            return JSonTypeahead(null, new ListViewPlanoConta());
        }

    }
}