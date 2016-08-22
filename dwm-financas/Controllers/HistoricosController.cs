using App_Dominio.Controllers;
using App_Dominio.Security;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System.Web.Mvc;

namespace DWM.Controllers
{
    public class HistoricosController : RootController<HistoricoViewModel, HistoricoModel>
    {
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override string getListName()
        {
            return "Listar Historicos";
        }

        #region List
        [AuthorizeFilter]
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                ListViewHistorico l = new ListViewHistorico();
                return this._List(index, pageSize, "Browse", l, descricao);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult _ListHistoricoContabilidadeModal(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                LookupHistoricoContabilidadeFiltroModel l = new LookupHistoricoContabilidadeFiltroModel();
                return this.ListModal(index, pageSize, l, "Descrição", descricao, "C");
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult _ListHistoricoContaPagarModal(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                LookupHistoricoContaPagarFiltroModel l = new LookupHistoricoContaPagarFiltroModel();
                return this.ListModal(index, pageSize, l, "Descrição", descricao, "P");
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult _ListHistoricoContaReceberModal(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                LookupHistoricoContaReceberFiltroModel l = new LookupHistoricoContaReceberFiltroModel();
                return this.ListModal(index, pageSize, l, "Descrição", descricao, "R");
            }
            else
                return View();
        }

        #endregion

        #region edit
        [AuthorizeFilter]
        public ActionResult Edit(int historicoId)
        {
            return _Edit(new HistoricoViewModel() { historicoId = historicoId });
        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult Delete(int historicoId)
        {
            return Edit(historicoId);
        }
        #endregion

        #region CrudHistoricoModal
        public JsonResult CrudHistoricoContabilidadeModal(string descricao)
        {
            return JSonCrud(new HistoricoViewModel() { descricao = descricao, ind_tipoHistorico = "C" });
        }

        public JsonResult CrudHistoricoContaReceberModal(string descricao)
        {
            return JSonCrud(new HistoricoViewModel() { descricao = descricao, ind_tipoHistorico = "R" });
        }

        public JsonResult CrudHistoricoContaPagarModal(string descricao)
        {
            return JSonCrud(new HistoricoViewModel() { descricao = descricao, ind_tipoHistorico = "P" });
        }
        #endregion

        public JsonResult getNames()
        {
            return JSonTypeahead(null, new ListViewHistorico());
        }
    }
}