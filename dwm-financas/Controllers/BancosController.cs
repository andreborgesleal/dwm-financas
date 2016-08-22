using App_Dominio.Controllers;
using App_Dominio.Security;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System.Web.Mvc;


namespace DWM.Controllers
{
    public class BancosController : RootController<BancoViewModel, BancoModel>
    {
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override string getListName()
        {
            return "Listar Bancos";
        }

        #region List
        [AuthorizeFilter]
        public override ActionResult List(int? index, int? pageSize = 50, string nome = null)
        {
            if (ViewBag.ValidateRequest)
            {
                ListViewBanco l = new ListViewBanco();
                return this._List(index, pageSize, "Browse", l, nome);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult _ListBancoModal(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                LookupBancoFiltroModel l = new LookupBancoFiltroModel();
                return this.ListModal(index, pageSize, l, "Descrição", descricao);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult _ListBanco2Modal(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                LookupBanco2FiltroModel l = new LookupBanco2FiltroModel();
                return this.ListModal(index, pageSize, l, "Descrição", descricao);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult _ListBanco3Modal(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                LookupBanco3FiltroModel l = new LookupBanco3FiltroModel();
                return this.ListModal(index, pageSize, l, "Descrição", descricao);
            }
            else
                return View();
        }

        #endregion

        #region edit
        [AuthorizeFilter]
        public ActionResult Edit(int bancoId)
        {
            return _Edit(new BancoViewModel() { bancoId = bancoId });
        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult Delete(int bancoId)
        {
            return Edit(bancoId);
        }
        #endregion

        public JsonResult getNames()
        {
            return JSonTypeahead(null, new ListViewBanco());
        }

    }
}