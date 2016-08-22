using App_Dominio.Contratos;
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
using System.Linq;
using System.Collections.Generic;
using App_Dominio.Component;

namespace DWM.Controllers
{
    public class CobrancaCadController : DwmRootController<CobrancaViewModel, CobrancaModel, ApplicationContext>
    {
        #region Herança
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override string getListName()
        {
            return "Listar Cadastro de Cobrança";
        }
        #endregion

        #region List
        [AuthorizeFilter]
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                ListViewCobranca l = new ListViewCobranca();
                return this._List(index, pageSize, "Browse", l, descricao);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult ListClientes(int? index, int? pageSize = 15, string descricao = null)
        {
            ViewBag.ValidateRequest = true;
            if (ViewBag.ValidateRequest)
            {
                CobrancaViewModel repository = new CobrancaViewModel() { cobrancaId = int.Parse(Request["cobrancaId"].Split(',')[0]) };
                ListViewCobrancaByCobrancaID l = new ListViewCobrancaByCobrancaID();
                Facade<CobrancaViewModel, CobrancaModel, ApplicationContext> facade = new Facade<CobrancaViewModel, CobrancaModel, ApplicationContext>();
                repository.pagedList = facade.getPagedList(l, index, pageSize.Value, repository.cobrancaId, descricao);
                return View(repository.pagedList);
            }
            else
                return View();
        }
        #endregion

        #region Create
        public override CobrancaViewModel SetCreate(CobrancaViewModel value, FormCollection collection, string breadCrumbText = "Inclusão")
        {
            if (ModelState.IsValid)
                try
                {
                    BeforeCreate(ref value, collection);

                    Factory<CobrancaViewModel, ApplicationContext> facade = new Factory<CobrancaViewModel, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    value = facade.Execute(new CobrancaCadastrarBI(), value);
                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    Success("Registro incluído com sucesso");
                }
                catch (App_DominioException ex)
                {
                    OnCreateError(ref value, collection);
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    if (ex.Result.MessageType == MsgType.ERROR)
                        Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    else
                        Attention(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                catch (Exception ex)
                {
                    OnCreateError(ref value, collection);
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    value.mensagem = new Validate() { Code = 17, MessageBase = "Um erro foi detectado. Detalhe: " + ex.Message, MessageType = MsgType.ERROR, Message = "Erro na inclusão do registro." };
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                finally
                {
                    BindBreadCrumb(breadCrumbText);
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

            return value;
        }

        #region BeforeCreate
        public override void BeforeCreate(ref CobrancaViewModel value, FormCollection collection)
        {
            value.valor = decimal.Parse(collection["valor.1"]);
            value.vr_jurosMora = decimal.Parse(collection["vr_jurosMora.1"]);
            value.vr_multa = decimal.Parse(collection["vr_multa.1"]);
        }
        #endregion
        #endregion

        #region Edit
        [AuthorizeFilter]
        public ActionResult Edit(int cobrancaId)
        {
            return _Edit(new CobrancaViewModel() { cobrancaId = cobrancaId });
        }

        public override void OnEditError(ref CobrancaViewModel value, FormCollection collection)
        {
            CobrancaViewModel repository = new CobrancaViewModel()
            {
                cobrancaId = value.cobrancaId,
            };
            ListViewCobrancaByCobrancaID l = new ListViewCobrancaByCobrancaID();
            Facade<CobrancaViewModel, CobrancaModel, ApplicationContext> facadeCob = new Facade<CobrancaViewModel, CobrancaModel, ApplicationContext>();
            repository = facadeCob.getObject(repository);

            value.CobrancaClientes = repository.CobrancaClientes;
            value.pagedList = facadeCob.getPagedList(l, 0, 15, value.cobrancaId);
        }

        [AuthorizeFilter]
        public override ActionResult _Edit(CobrancaViewModel value, string breadCrumbText = null, IDictionary<string, string> text = null)
        {
            ViewBag.ValidateRequest = true;
            if (ViewBag.ValidateRequest)
            {
                CobrancaViewModel repository = GetEdit(value, breadCrumbText, text);
                if (repository.mensagem.Code == 202)
                    return RedirectToAction("../Home/_Error");
                ListViewCobrancaByCobrancaID l = new ListViewCobrancaByCobrancaID();
                Facade<CobrancaViewModel, CobrancaModel, ApplicationContext> facade = new Facade<CobrancaViewModel, CobrancaModel, ApplicationContext>();
                repository.pagedList = facade.getPagedList(l, 0, 15, value.cobrancaId);
                return View(repository);
            }
            else
                return null;
        }

        public override void BeforeEdit(ref CobrancaViewModel value, FormCollection collection)
        {
            BeforeCreate(ref value, collection);
        }

        [AuthorizeFilter]
        public ActionResult CreateItem(string cobrancaId, string clienteId, string dia_vencimento, string dia_vencimento2, string valor, string valor21)
        {
            if (ViewBag.ValidateRequest)
            {
                CobrancaClienteViewModel value = new CobrancaClienteViewModel();
                try
                {
                    value.cobrancaId = int.Parse(cobrancaId);

                    if (clienteId == null || clienteId == "")
                        throw new Exception("Cliente deve ser informado");
                    value.clienteId = int.Parse(clienteId);

                    if (dia_vencimento2 != null && dia_vencimento != null)
                    {
                        value.dia_vencimento = int.Parse(dia_vencimento);
                        value.dia_vencimento = value.dia_vencimento == int.Parse(dia_vencimento2) ? 0 : int.Parse(dia_vencimento2);
                    }
                    else
                        value.dia_vencimento = 0;

                    if (valor != null && valor21 != null)
                    {
                        value.valor = decimal.Parse(valor);
                        value.valor = value.valor == decimal.Parse(valor21) ? 0 : decimal.Parse(valor21);
                    }
                    else
                        value.valor = 0;

                    Facade<CobrancaClienteViewModel, CobrancaClienteModel, ApplicationContext> facade = new Facade<CobrancaClienteViewModel, CobrancaClienteModel, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    value = facade.Save(value, Crud.INCLUIR);

                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    Success("Registro incluído com sucesso");
                }
                catch (App_DominioException ex)
                {
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    OnCreateItemError(ref value,int.Parse(cobrancaId), clienteId, dia_vencimento2, valor21);
                }
                catch (Exception ex)
                {
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    OnCreateItemError(ref value, int.Parse(cobrancaId), clienteId, dia_vencimento2, valor21);
                }

                CobrancaViewModel repository = new CobrancaViewModel()
                {
                    cobrancaId = int.Parse(cobrancaId),
                };

                ListViewCobrancaByCobrancaID l = new ListViewCobrancaByCobrancaID();
                Facade<CobrancaViewModel, CobrancaModel, ApplicationContext> facadeCob = new Facade<CobrancaViewModel, CobrancaModel, ApplicationContext>();
                repository = facadeCob.getObject(repository);
                repository.CobrancaClienteViewModel.dia_vencimento = value.dia_vencimento;
                repository.CobrancaClienteViewModel.valor = value.valor;
                repository.CobrancaClienteViewModel.clienteId = value.clienteId;
                repository.pagedList = facadeCob.getPagedList(l, 0, 15, cobrancaId);

                return View(repository);
            }
            else
            {
                Error("Acesso para esta funcionalidade negado");
                return View();
            }
        }

        private void OnCreateItemError(ref CobrancaClienteViewModel value, int cobrancaId, string clienteId, string dia_vencimento, string valor)
        {
            value.cobrancaId = cobrancaId;
            value.clienteId = clienteId == null || clienteId == "" ? 0 : int.Parse(clienteId);
            value.dia_vencimento = dia_vencimento == null || dia_vencimento == "" ? 0 : int.Parse(dia_vencimento);
            value.valor = valor != null ? decimal.Parse(valor) : 0;
        }

        [AuthorizeFilter]
        public ActionResult EditItem(string cobrancaId, string clienteId, string dia_vencimento, string dia_vencimento3, string valor, string valor31)
        {
            if (ViewBag.ValidateRequest)
            {
                CobrancaClienteViewModel value = new CobrancaClienteViewModel();
                try
                {
                    value.cobrancaId = int.Parse(cobrancaId);

                    if (clienteId == null || clienteId == "")
                        throw new Exception("Cliente deve ser informado");
                    value.clienteId = int.Parse(clienteId);

                    if (dia_vencimento3 != null && dia_vencimento != null)
                    {
                        value.dia_vencimento = int.Parse(dia_vencimento);
                        value.dia_vencimento = value.dia_vencimento == int.Parse(dia_vencimento3) ? 0 : int.Parse(dia_vencimento3);
                    }
                    else
                        value.dia_vencimento = 0;

                    if (valor != null && valor31 != null)
                    {
                        value.valor = decimal.Parse(valor);
                        value.valor = value.valor == decimal.Parse(valor31) ? 0 : decimal.Parse(valor31);
                    }
                    else
                        value.valor = 0;

                    Facade<CobrancaClienteViewModel, CobrancaClienteModel, ApplicationContext> facade = new Facade<CobrancaClienteViewModel, CobrancaClienteModel, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    value = facade.Save(value, Crud.ALTERAR);

                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    Success("Registro alterado com sucesso");
                }
                catch (App_DominioException ex)
                {
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    OnCreateItemError(ref value, int.Parse(cobrancaId), clienteId, dia_vencimento3, valor31);
                }
                catch (Exception ex)
                {
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    OnCreateItemError(ref value, int.Parse(cobrancaId), clienteId, dia_vencimento3, valor31);
                }

                CobrancaViewModel repository = new CobrancaViewModel()
                {
                    cobrancaId = int.Parse(cobrancaId),
                };

                ListViewCobrancaByCobrancaID l = new ListViewCobrancaByCobrancaID();
                Facade<CobrancaViewModel, CobrancaModel, ApplicationContext> facadeCob = new Facade<CobrancaViewModel, CobrancaModel, ApplicationContext>();
                repository = facadeCob.getObject(repository);
                //repository.CobrancaClienteViewModel.dia_vencimento = value.dia_vencimento;
                //repository.CobrancaClienteViewModel.valor = value.valor;
                //repository.CobrancaClienteViewModel.clienteId = value.clienteId;
                repository.pagedList = facadeCob.getPagedList(l, 0, 15, cobrancaId);

                return View("CreateItem", repository);
            }
            else
            {
                Error("Acesso para esta funcionalidade negado");
                return View("CreateItem", null);
            }
        }

        [AuthorizeFilter]
        public ActionResult DelItem(string cobrancaId, string clienteId)
        {
            if (ViewBag.ValidateRequest)
            {
                CobrancaClienteViewModel value = new CobrancaClienteViewModel();
                try
                {
                    value.cobrancaId = int.Parse(cobrancaId);

                    if (clienteId == null || clienteId == "")
                        throw new Exception("Cliente deve ser informado");
                    value.clienteId = int.Parse(clienteId);

                    Facade<CobrancaClienteViewModel, CobrancaClienteModel, ApplicationContext> facade = new Facade<CobrancaClienteViewModel, CobrancaClienteModel, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    value = facade.Save(value, Crud.EXCLUIR);

                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    Success("Registro excluído com sucesso");
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

                CobrancaViewModel repository = new CobrancaViewModel()
                {
                    cobrancaId = int.Parse(cobrancaId),
                };

                ListViewCobrancaByCobrancaID l = new ListViewCobrancaByCobrancaID();
                Facade<CobrancaViewModel, CobrancaModel, ApplicationContext> facadeCob = new Facade<CobrancaViewModel, CobrancaModel, ApplicationContext>();
                repository = facadeCob.getObject(repository);
                repository.pagedList = facadeCob.getPagedList(l, 0, 15, cobrancaId);

                return View("CreateItem", repository);
            }
            else
            {
                Error("Acesso para esta funcionalidade negado");
                return View("CreateItem", null);
            }

        }
        #endregion

        public JsonResult getNames()
        {
            return JSonTypeahead(null, new ListViewCobranca());
        }
    }
}