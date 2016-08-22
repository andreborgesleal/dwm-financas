using App_Dominio.Controllers;
using App_Dominio.Security;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System.Web.Mvc;

namespace DWM.Controllers
{
    public class FeriadosController : RootController<FeriadoViewModel, FeriadoModel>
    {
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override string getListName()
        {
            return "Listar Feriados";
        }

        #region List
        [AuthorizeFilter]
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                ListViewFeriado c = new ListViewFeriado();
                return this._List(index, pageSize, "Browse", c, descricao);
            }
            else
                return View();
        }
        #endregion

        #region Edit
        [AuthorizeFilter]
        public ActionResult Edit(int feriadoId)
        {
            return _Edit(new FeriadoViewModel() { feriadoId = feriadoId });
        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult Delete(int feriadoId)
        {
            return Edit(feriadoId);
        }
        #endregion
    }
}