using App_Dominio.Controllers;
using App_Dominio.Enumeracoes;
using App_Dominio.Pattern;
using App_Dominio.Security;
using DWM.Models.BI;
using DWM.Models.Entidades;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
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

        #region Open
        [AuthorizeFilter]
        public ActionResult Open(int empresaId, int exercicio)
        {
            if (ViewBag.ValidateRequest)
                try
                {
                    ExercicioViewModel value = new ExercicioViewModel() { empresaId = empresaId, exercicio = exercicio };

                    Factory<ExercicioViewModel, ApplicationContext> facade = new Factory<ExercicioViewModel, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    value = facade.Execute(new ExercicioAbrirBI(), value);
                }
                catch (App_DominioException ex)
                {
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                catch (Exception ex)
                {
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }

            return RedirectToAction("../Home/Default");
        }
        #endregion


        #endregion
    }
}