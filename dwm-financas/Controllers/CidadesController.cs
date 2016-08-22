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
    public class CidadesController : RootController<CidadeViewModel, CidadeModel>
    {
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override string getListName()
        {
            return "Listar Cidades";
        }

        #region List
        [AuthorizeFilter]
        public override ActionResult List(int? index, int? pageSize = 50, string nome = null)
        {
            if (ViewBag.ValidateRequest)
            {
                ListViewCidade c = new ListViewCidade();
                return this._List(index, pageSize, "Browse", c, nome);
            }
            else
                return View();
        }
        #endregion

        #region Edit
        [AuthorizeFilter]
        public ActionResult Edit(int cidadeId)
        {
            return _Edit(new CidadeViewModel() { cidadeId = cidadeId });
        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult Delete(int cidadeId)
        {
            return Edit(cidadeId);
        }
        #endregion
    }
}