using App_Dominio.Controllers;
using App_Dominio.Security;
using DWM.Models.Entidades;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System.Web.Mvc;

namespace DWM.Controllers
{
    public class GrupoCobrancaController : DwmRootController<GrupoCobrancaViewModel, GrupoCobrancaModel, ApplicationContext>
    {
        #region Herança
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override string getListName()
        {
            return "Listar Grupos de Cobrança";
        }
        #endregion

        #region List
        [AuthorizeFilter]
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                ListViewGrupoCobranca l = new ListViewGrupoCobranca();
                return this._List(index, pageSize, "Browse", l, descricao);
            }
            else
                return View();
        }
        #endregion

        #region Edit
        [AuthorizeFilter]
        public ActionResult Edit(int grupoCobrancaId)
        {
            return _Edit(new GrupoCobrancaViewModel() { grupoCobrancaId = grupoCobrancaId });
        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult Delete(int grupoCobrancaId)
        {
            return Edit(grupoCobrancaId);
        }
        #endregion
    }
}