using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using App_Dominio.Security;
using App_Dominio.Contratos;
using App_Dominio.Controllers;
using System.Data.Entity.Validation;
using App_Dominio.Entidades;
using DWM.Models;
using App_Dominio.Enumeracoes;
using App_Dominio.Repositories;
using DWM.Models.Entidades;
using DWM.Models.Repositories;
using DWM.Models.Pattern;
using DWM.Models.BI;
using App_Dominio.Pattern;

namespace DWM.Controllers
{
    [Authorize]
    public class AccountController : SuperController
    {
        #region Inheritance
        public override int _sistema_id() { return (int)DWM.Models.Enumeracoes.Sistema.DWMFINANCAS; }

        public override string getListName()
        {
            return "Login";
        }

        public override ActionResult List(int? index, int? pageSize = 40, string descricao = null)
        {
            throw new NotImplementedException();
        }
        #endregion

        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(); // RedirectToAction("Default", "Home");
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                EmpresaSecurity<App_DominioContext> security = new EmpresaSecurity<App_DominioContext>();
                try
                {
                    #region Autorizar
                    Validate result = security.Autorizar(model.UserName, model.Password, _sistema_id());
                    if (result.Code > 0)
                        throw new ArgumentException(result.Message);
                    #endregion

                    Sessao s = security.getSessaoCorrente();

                    #region Alterar sessão - Atualizar Exercício Corrente
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        Exercicio ex = null;
                        if (db.Exercicios.Where(info => info.empresaId == s.empresaId && info.encerrado == "N").Count() > 0)
                            ex = db.Exercicios.Where(info => info.empresaId == s.empresaId && info.encerrado == "N").FirstOrDefault();
                        if (ex != null)
                        {
                            result = security.AlterarSessao(ex.exercicio.ToString());
                            if (result.Code > 0)
                                throw new ArgumentException(result.Message);
                        }
                        else
                        {
                            #region Desativar sessão
                            if (System.Web.HttpContext.Current != null)
                                security.EncerrarSessao(System.Web.HttpContext.Current.Session.SessionID);
                            #endregion
                            throw new ArgumentException("Não foi possivel estabelecer as configurações de acesso. Entre em contato com a DWM Sistemas e informe o código \"001\".");
                        }
                            
                    }
                    #endregion

                    string sessaoId = result.Field;

                    return RedirectToAction("Default", "Home");
                }
                catch (ArgumentException ex)
                {
                    Error(ex.Message);
                }
                catch (App_DominioException ex)
                {
                    Error("Erro na autorização de acesso. Favor entre em contato com o administrador do sistema");
                }
                catch (DbEntityValidationException ex)
                {
                    Error("Não foi possível autorizar o seu acesso. Favor entre em contato com o administrador do sistema");
                }
                catch (Exception ex)
                {
                    Error("Erro na autorização de acesso. Favor entre em contato com o administrador do sistema");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        #region Termo de Uso e Política de Privacidade
        [AllowAnonymous]
        public ActionResult TermoUso()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Politica()
        {
            return View();
        }
        #endregion

        #region Alterar Senha
        public ActionResult AlterarSenha()
        {
            return View();
        }

        #endregion

        #region Esqueci minha senha
        [AllowAnonymous]
        public ActionResult Forgot(int id)
        {
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Forgot(UsuarioViewModel value, FormCollection collection)
        {
            try
            {
                if (string.IsNullOrEmpty(collection["empresaId"]))
                    throw new Exception("Identificador do condomínio não localizado. Favor entrar em contato com a administração.");

                value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                FactoryLocalhost<UsuarioViewModel, ApplicationContext> factory = new FactoryLocalhost<UsuarioViewModel, ApplicationContext>();
                value = factory.Execute(new EsqueciMinhaSenhaBI(), value);
                if (factory.Mensagem.Code > 0)
                    throw new App_DominioException(factory.Mensagem);

                Success("E-mail com as intruções de renovação de senha enviado com sucesso");
            }
            catch (App_DominioException ex)
            {
                ModelState.AddModelError("", ex.Result.MessageBase); // mensagem amigável ao usuário
                Error(ex.Result.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                return View(value);
            }
            catch (Exception ex)
            {
                App_DominioException.saveError(ex, GetType().FullName);
                ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                return View(value);
            }

            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public ActionResult EsqueciMinhaSenha(string id, string key)
        {
            UsuarioRepository value = new UsuarioRepository();
            if (id != null && id != "")
            {
                value.usuarioId = int.Parse(id);
                value.keyword = key;
                Factory<UsuarioRepository, ApplicationContext> factory = new Factory<UsuarioRepository, ApplicationContext>();
                value = factory.Execute(new CodigoValidacaoBI(), value);
                if (value.mensagem.Code == -1)
                    return View(value);
                else
                {
                    ModelState.AddModelError("", value.mensagem.MessageBase); // mensagem amigável ao usuário
                    Error(value.mensagem.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EsqueciMinhaSenha(UsuarioRepository value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (value.usuarioId != 0)
                    {
                        Factory<UsuarioRepository, ApplicationContext> factory = new Factory<UsuarioRepository, ApplicationContext>();
                        value = factory.Execute(new CodigoAtivacaoBI(), value);
                        if (value.mensagem.Code > 0)
                            throw new App_DominioException(value.mensagem);
                        Success("Senha alterada com sucesso. Faça seu login para acessar o sistema");
                        return RedirectToAction("Login", "Account");
                    };
                }
                catch (App_DominioException ex)
                {
                    ModelState.AddModelError("", ex.Result.MessageBase); // mensagem amigável ao usuário
                    Error(ex.Result.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                catch (Exception ex)
                {
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
            }
            else
                Error("Dados incorretos");

            return View(value);
        }
        #endregion

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel value, FormCollection collection)
        {
            if (ModelState.IsValid)
                try
                {
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

                    //AccountModel model = new AccountModel();

                    //value = model.SaveAll(value, Crud.INCLUIR);
                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    Success("Registro incluído com sucesso");
                    return RedirectToAction("Login", "Account");
                }
                catch (App_DominioException ex)
                {
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    if (ex.Result.MessageType == MsgType.ERROR)
                        Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    else
                        Attention(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                catch (Exception ex)
                {
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
            else
            {
                value.mensagem = new Validate()
                {
                    Code = 999,
                    Message = MensagemPadrao.Message(999).ToString(),
                    MessageBase = ModelState.Values.Where(erro => erro.Errors.Count > 0).First().Errors[0].ErrorMessage
                };
                ModelState.AddModelError("", value.mensagem.Message); // mensagem amigável ao usuário
                Attention(value.mensagem.MessageBase);
            }

            return View(value);
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            System.Web.HttpContext web = System.Web.HttpContext.Current;
            new EmpresaSecurity<App_DominioContext>().EncerrarSessao(web.Session.SessionID);

            return RedirectToAction("Login", "Account");
        }


    }
}
