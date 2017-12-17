using App_Dominio.Controllers;
using App_Dominio.Security;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Web.Mvc;
using App_Dominio.Models;
using DWM.Models.Entidades;
using App_Dominio.Pattern;
using DWM.Models.BI;
using App_Dominio.Enumeracoes;
using App_Dominio.Contratos;
using System.Linq;

namespace DWM.Controllers
{
    public class MovimentoBancarioController : DwmRootController<MovtoBancarioViewModel,MovtoBancarioModel,ApplicationContext>
    {
        #region Herança
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override bool mustListOnLoad()
        {
            return false;
        }
        public override string getListName()
        {
            return "Listar Movimentação Bancária";
        }
        #endregion

        #region List
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {
            return ListParam(index, PageSize, Request["bancoId"], Request["data1"], Request["data2"]);
        }

        [AuthorizeFilter]
        public ActionResult ListParam(int? index, int? pageSize = 50, string bancoId = null,
                                        string data1 = null, string data2 = null)
        {
            ViewBag.ValidateRequest = true;
            if (ViewBag.ValidateRequest)
            {
                int? _null = null;
                int? _bancoId = bancoId != null && bancoId != "" ? int.Parse(bancoId) : _null;
                DateTime _data1 = data1 == null || data1 == "" ? new DateTime(Funcoes.Brasilia().Year, Funcoes.Brasilia().Month, 1) : Convert.ToDateTime(data1);
                DateTime _data2 = data2 == null || data2 == "" ? new DateTime(Funcoes.Brasilia().Year, Funcoes.Brasilia().Month, Funcoes.Brasilia().Day) : Convert.ToDateTime(data2);
                ListViewMovtoBancario list = new ListViewMovtoBancario();
                return this._List(index, pageSize, "Browse", list, _bancoId, _data1, _data2);
            }
            else
                return View();
        }
        #endregion

        #region Edit
        [AuthorizeFilter]
        public ActionResult Edit(int movtoBancarioId)
        {
            return _Edit(new MovtoBancarioViewModel() { movtoBancarioId = movtoBancarioId });
        }

        public override void BeforeEdit(ref MovtoBancarioViewModel value, FormCollection collection)
        {
            base.BeforeEdit(ref value, collection);
            if (collection["valor.1"] != null && collection["valor.1"] != null)
                value.valor = decimal.Parse(collection["valor.1"]);
        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult Delete(int movtoBancarioId)
        {
            return Edit(movtoBancarioId);
        }
        #endregion

        #region Transferência Bancária
        [AuthorizeFilter]
        [HttpGet]
        public ActionResult Transferencia()
        {
            if (ViewBag.ValidateRequest)
            {
                BindBreadCrumb("Inclusão");
                return View();
            }
            else
                return null;
        }

        [AuthorizeFilter]
        [HttpPost]
        public ActionResult Transferencia(FormCollection collection)
        {
            if (ViewBag.ValidateRequest)
            {
                TransferenciaBancariaViewModel result = new TransferenciaBancariaViewModel();
                if (ModelState.IsValid)
                    try
                    {
                        int? _enquadramentoId = null;
                        int _bancoOrigem = 0;
                        int _bancoDestino = 0;
                        int _historicoId = 0;
                        decimal _valor = 0;
                        DateTime _dt_movto = new DateTime(1980, 1, 1);

                        if (collection["enquadramentoId"] != null && collection["enquadramentoId"] != "")
                            _enquadramentoId = int.Parse(collection["enquadramentoId"]);

                        if (collection["bancoId"] != null && collection["bancoId"] != "")
                            _bancoOrigem = int.Parse(collection["bancoId"]);

                        if (collection["banco2Id"] != null && collection["banco2Id"] != "")
                            _bancoDestino = int.Parse(collection["banco2Id"]);

                        if (collection["historicoId"] != null && collection["historicoId"] != "")
                            _historicoId = int.Parse(collection["historicoId"]);

                        if (collection["DECIMAL"] != null && collection["DECIMAL"] != "")
                            _valor = decimal.Parse(collection["DECIMAL"]);

                        if (collection["dt_movto"] != null && collection["dt_movto"] != "")
                            _dt_movto = Convert.ToDateTime(collection["dt_movto"]);

                        TransferenciaBancariaViewModel value = new TransferenciaBancariaViewModel()
                        {
                            movtoBancarioOrigemViewModel = new MovtoBancarioViewModel()
                            {
                                dt_movto = _dt_movto,
                                bancoId = _bancoOrigem,
                                historicoId = _historicoId,
                                complementoHist = collection["complementoHist"],
                                valor = _valor,
                                tipoMovto = "D"
                            },
                            movtoBancarioDestinoViewModel = new MovtoBancarioViewModel()
                            {
                                dt_movto = _dt_movto,
                                bancoId = _bancoDestino,
                                historicoId = _historicoId,
                                complementoHist = collection["complementoHist"],
                                valor = _valor,
                                tipoMovto = "C"
                            },
                            enquadramentoId = _enquadramentoId
                        };
                        Factory<TransferenciaBancariaViewModel, ApplicationContext> facade = new Factory<TransferenciaBancariaViewModel, ApplicationContext>();
                        value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                        value = facade.Execute(new TransferenciaBancariaBI(), value);
                        result = value;
                        if (value.mensagem.Code > 0)
                            throw new Exception(value.mensagem.MessageBase);
                        Success("Rgistro incluído com sucesso!!!");
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
                else
                {
                    result.mensagem = new Validate()
                    {
                        Code = 999,
                        Message = MensagemPadrao.Message(999).ToString(),
                        MessageBase = ModelState.Values.Where(erro => erro.Errors.Count > 0).First().Errors[0].ErrorMessage
                    };
                    ModelState.AddModelError("", result.mensagem.Message); // mensagem amigável ao usuário
                    Attention(result.mensagem.MessageBase);
                }

                return View(result);
            }
            else
            {
                Error("Acesso para esta funcionalidade negado");
                return View();
            }
        }
        #endregion

        #region Lançamentos recorrentes
        [AuthorizeFilter]
        [HttpGet]
        public ActionResult Recorrencia()
        {
            if (ViewBag.ValidateRequest)
            {
                BindBreadCrumb("Inclusão");

                Factory<ExercicioViewModel, ApplicationContext> facade = new Factory<ExercicioViewModel, ApplicationContext>();
                ExercicioViewModel value = facade.Execute(new ExercicioBI(), new ExercicioViewModel());

                return View(value);
            }
            else
                return null;
        }

        [AuthorizeFilter]
        [HttpPost]
        public ActionResult Recorrencia(FormCollection collection)
        {
            if (ViewBag.ValidateRequest)
            {
                ExercicioViewModel result = new ExercicioViewModel();
                if (ModelState.IsValid)
                    try
                    {
                        Factory<ExercicioViewModel, ApplicationContext> facade = new Factory<ExercicioViewModel, ApplicationContext>();
                        ExercicioViewModel value = facade.Execute(new ExercicioBI(), new ExercicioViewModel());

                        if (value.mensagem.Code == 0 && value.dt_lancamento_inicio.HasValue)
                        {
                            value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                            value = facade.Execute(new GerarLancamentosRecorrentesBI(), value);
                            result = value;
                            if (value.mensagem.Code > 0)
                                throw new Exception(value.mensagem.MessageBase);
                            Success("Registro incluído com sucesso!!!");
                        }
                        else
                            throw new Exception("Data de Lançamento Contábil inválida para geração da recorrência");
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
                else
                {
                    result.mensagem = new Validate()
                    {
                        Code = 999,
                        Message = MensagemPadrao.Message(999).ToString(),
                        MessageBase = ModelState.Values.Where(erro => erro.Errors.Count > 0).First().Errors[0].ErrorMessage
                    };
                    ModelState.AddModelError("", result.mensagem.Message); // mensagem amigável ao usuário
                    Attention(result.mensagem.MessageBase);
                }

                return View(result);
            }
            else
            {
                Error("Acesso para esta funcionalidade negado");
                return View();
            }
        }
        #endregion
    }
}