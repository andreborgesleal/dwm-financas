using App_Dominio.Controllers;
using App_Dominio.Security;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System.Web.Mvc;
using DWM.Models.Entidades;

namespace DWM.Controllers
{
    public class ConvenioController : DwmRootController<ConvenioViewModel, ConvenioModel, ApplicationContext>
    {
        #region Herança
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override bool mustListOnLoad()
        {
            return true;
        }
        public override string getListName()
        {
            return "Listar Convênios";
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

        #region Edit
        [AuthorizeFilter]
        public ActionResult Edit(string BancoID, string ConvenioID)
        {
            return _Edit(new ConvenioViewModel() { BancoID = BancoID, ConvenioID = ConvenioID });
        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult Delete(string BancoID, string ConvenioID)
        {
            return Edit(BancoID, ConvenioID);
        }
        #endregion
    }
}