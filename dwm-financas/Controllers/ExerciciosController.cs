using App_Dominio.Controllers;
using App_Dominio.Security;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System.Web.Mvc;

namespace DWM.Controllers
{
    public class ExerciciosController : RootController<ExercicioViewModel, ExercicioModel>
    {
        #region inheritance
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override string getListName()
        {
            return "Listar";
        }
        #endregion

        #region List
        [AuthorizeFilter]
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                ListViewExercicio l = new ListViewExercicio();
                return this._List(index, pageSize, "Browse", l, descricao);
            }
            else
                return View();
        }
        #endregion

        #region edit
        [AuthorizeFilter]
        public ActionResult Edit(int empresaId, int exercicio)
        {
            return _Edit(new ExercicioViewModel() { empresaId = empresaId, exercicio = exercicio });
        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult Delete(int empresaId, int exercicio)
        {
            return Edit(empresaId, exercicio);
        }
        #endregion
    }
}