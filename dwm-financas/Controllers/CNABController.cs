using App_Dominio.Controllers;
using App_Dominio.Security;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Web.Mvc;
using App_Dominio.Models;
using System.Collections.Generic;
using System.Linq;
using DWM.Models.Entidades;
using App_Dominio.Pattern;
using App_Dominio.Enumeracoes;
using DWM.Models.BI;
using App_Dominio.Entidades;
using DWM.Models.Pattern;

namespace DWM.Controllers
{
    public class CNABController : DwmRootController<TituloViewModel, TituloModel, ApplicationContext>
    {
        #region Herança
        public override int _sistema_id() { return (int)DWM.Models.Enumeracoes.Sistema.DWMFINANCAS; }
        public override bool mustListOnLoad()
        {
            return true;
        }
        public override string getListName()
        {
            return "Listar Títulos";
        }
        #endregion

        #region List
        [AuthorizeFilter]
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                return ListParam(index, PageSize, Request["titulos_vencidos_atraso"], Request["dt_vencidos_atraso1"], Request["dt_vencidos_atraso2"], Request["titulos_a_vencer"], Request["dt_vencimento1"], Request["dt_vencimento2"],
                    Request["titulos_amortizados"], Request["titulos_nao_pagos"], Request["baixa_liquidacao"], Request["baixa_cancelamento"], Request["dt_baixa1"], Request["dt_baixa2"],
                    Request["clienteId"], Request["dt_emissao1"], Request["dt_emissao2"], Request["centroCustoId"], Request["grupoClienteId"], Request["bancoId"]);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult ListParam(int? index, int? pageSize = 50, string titulos_vencidos_atraso_ = null,
                                string dt_vencidos_atraso1 = null, string dt_vencidos_atraso2 = null, string titulos_a_vencer_ = null,
                                string dt_vencimento1 = null, string dt_vencimento2 = null, string titulos_amortizados_ = null,
                                string titulos_nao_pagos_ = null, string baixa_liquidacao_ = null, string baixa_cancelamento_ = null,
                                string dt_baixa1 = null, string dt_baixa2 = null, string clienteId = null,
                                string dt_emissao1 = null, string dt_emissao2 = null, string centroCustoId = null,
                                string grupoClienteId = null, string bancoId = null)
        {
            ViewBag.ValidateRequest = true;
            if (ViewBag.ValidateRequest)
            {
                int? _null = null;
                bool _titulos_vencidos_atraso = titulos_vencidos_atraso_ == "true" ? true : false;
                DateTime? _dt_vencidos_atraso1 = Funcoes.StringToDate(dt_vencidos_atraso1);
                DateTime? _dt_vencidos_atraso2 = Funcoes.StringToDate(dt_vencidos_atraso2);
                bool _titulos_a_vencer = titulos_a_vencer_ == "true" ? true : false;
                DateTime? _dt_vencimento1 = Funcoes.StringToDate(dt_vencimento1);
                DateTime? _dt_vencimento2 = Funcoes.StringToDate(dt_vencimento2);
                bool _titulos_amortizados = titulos_amortizados_ == "true" ? true : false;
                bool _titulos_nao_pagos = titulos_nao_pagos_ == "true" ? true : false;
                bool _baixa_liquidacao = baixa_liquidacao_ == "true" ? true : false;
                bool _baixa_cancelamento = baixa_cancelamento_ == "true" ? true : false;
                DateTime? _dt_baixa1 = Funcoes.StringToDate(dt_baixa1);
                DateTime? _dt_baixa2 = Funcoes.StringToDate(dt_baixa2);
                int? _clienteId = clienteId != null && clienteId != "" ? int.Parse(clienteId) : _null;
                DateTime? _dt_emissao1 = Funcoes.StringToDate(dt_emissao1);
                DateTime? _dt_emissao2 = Funcoes.StringToDate(dt_emissao2); ;
                int? _centroCustoId = centroCustoId != null && centroCustoId != "" ? int.Parse(centroCustoId) : _null;
                int? _grupoId = grupoClienteId != null && grupoClienteId != "" ? int.Parse(grupoClienteId) : _null;
                int? _bancoId = bancoId != null && bancoId != "" ? int.Parse(bancoId) : _null;
                ListViewCNABDemonstrativoBI list = new ListViewCNABDemonstrativoBI();
                return this._List(index, pageSize, "Browse", list, _titulos_vencidos_atraso, _dt_vencidos_atraso1, _dt_vencidos_atraso2, _titulos_a_vencer,
                                    _dt_vencimento1, _dt_vencimento2, _titulos_amortizados, _titulos_nao_pagos, _baixa_liquidacao,
                                    _baixa_cancelamento, _dt_baixa1, _dt_baixa2, _clienteId, _dt_emissao1, _dt_emissao2, _centroCustoId, _grupoId, _bancoId);
            }
            else
                return View();
        }

        public ActionResult _List(int? index, int? pageSize, string action, ListViewCNABDemonstrativoBI model, params object[] param)
        {
            if (ViewBag.ValidateRequest)
            {
                Factory<ContaReceberDemonstrativoViewModel, ApplicationContext> facadeCob = new Factory<ContaReceberDemonstrativoViewModel, ApplicationContext>();
                IPagedList pagedList = facadeCob.PagedList(model, index, pageSize.Value, param);
                UpdateBreadCrumb(this.ControllerContext.RouteData.Values["controller"].ToString(), action);
                return View(pagedList);
            }
            else
                return null;
        }
        #endregion

        #region Create
        [AuthorizeFilter]
        public override ActionResult Create()
        {
            if (ViewBag.ValidateRequest)
            {
                GetCreate();
                EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();
                return View(new TituloIncluirViewModel() { empresaId = security.getSessaoCorrente().empresaId,
                                                           BancoID = "",
                                                           ConvenioID = "",
                                                           DataVencimento = Funcoes.Brasilia().Date,
                                                           SeuNumero="",
                                                           ValorJuros = 0,
                                                           ValorMulta = 0,
                                                           ValorAbatimento = 0,
                                                           ValorDesconto1 = 0} );
            }
            else
                return null;
        }

        [HttpPost]
        [AuthorizeFilter]
        public override ActionResult Create(TituloViewModel value, FormCollection collection)
        {
            if (ViewBag.ValidateRequest)
            {
                try
                {
                    string[] Convenio = collection["ConvenioID"].Split('|');

                    FactoryLocalhost<TituloIncluirViewModel, ApplicationContext> factory = new FactoryLocalhost<TituloIncluirViewModel, ApplicationContext>();
                    TituloIncluirViewModel repository = new TituloIncluirViewModel()
                    {
                        operacaoId = 0,
                        parcelaId = 1,
                        SequenciaID = 1,
                        BancoID = Convenio[0],
                        ConvenioID = Convenio[1],
                        empresaId = collection["empresaId"] != null && collection["empresaId"] != "" ? int.Parse(collection["empresaId"]) : 0,
                        uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString(),
                        clienteId = value.clienteId,
                        SeuNumero = value.SeuNumero,
                        DataVencimento = value.DataVencimento,
                        ValorPrincipal = value.ValorPrincipal,
                        Especie = value.Especie,
                        Aceite = value.Aceite,
                        DataEmissao = Funcoes.Brasilia().Date,
                        DataJuros = value.DataVencimento,
                        ValorJuros = value.ValorJuros,
                        DataDesconto1 = value.DataVencimento,
                        ValorDesconto1 = value.ValorDesconto1,
                        ValorAbatimento = value.ValorAbatimento,
                        MultaID = value.ValorMulta.HasValue && value.ValorMulta > 0 ? "1" : "0",
                        DataMulta = value.DataVencimento,
                        ValorMulta = value.ValorMulta,
                        InstrucaoRodape = value.InstrucaoRodape,
                        InstrucaoPagamento1 = value.InstrucaoPagamento1,
                        InstrucaoPagamento2 = value.InstrucaoPagamento2,
                        InstrucaoPagamento3 = value.InstrucaoPagamento3,
                        InstrucaoPagamento4 = value.InstrucaoPagamento4,
                        IndAtivo = 1,
                        documento = collection["documento"]
                    };
                    if (collection["historicoId"] != null && collection["historicoId"] != "")
                        repository.historicoId = int.Parse(collection["historicoId"]);

                    repository.complementoHist = collection["complementoHist"];

                    if (collection["enquadramentoId"] != null && collection["enquadramentoId"] != "")
                        repository.enquadramentoId = int.Parse(collection["enquadramentoId"]);

                    if (collection["centroCustoId"] != null && collection["centroCustoId"] != "")
                        repository.centroCustoId = int.Parse(collection["centroCustoId"]);

                    repository = factory.Execute(new TituloIncluirBI(), repository);

                    if (factory.Mensagem.Code > 0)
                        throw new App_DominioException(factory.Mensagem);

                    Success(factory.Mensagem.Message);

                    return RedirectToAction("../Cobranca/Edit", new { operacaoId = repository.operacaoId, parcelaId = 1, SequencialID = 1 });
                }
                catch (App_DominioException ex)
                {
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
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
                Error("Acesso para esta funcionalidade negado");

            return View();
        }

        public override void BeforeCreate(ref TituloViewModel value, FormCollection collection)
        {
            base.BeforeCreate(ref value, collection);
        }
        #endregion

        #region Edit
        [AuthorizeFilter]
        public ActionResult Edit(int operacaoId, int parcelaId)
        {
            BindBreadCrumb(getBreadCrumbText(null, null));
            if (ViewBag.ValidateRequest)
            {
                EditarContaReceberViewModel value = new EditarContaReceberViewModel();
                try
                {
                    value.operacaoId = operacaoId;
                    value.parcelaId = parcelaId;
                    Factory<EditarContaReceberViewModel, ApplicationContext> facade = new Factory<EditarContaReceberViewModel, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    value = facade.Execute(new TituloEditarBI(), value);
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

                return View(value);
            }
            else
                return View();
        }
        #endregion
    }
}