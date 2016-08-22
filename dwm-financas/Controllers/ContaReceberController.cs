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

namespace DWM.Controllers
{
    public class ContaReceberController : DwmRootController<ContaReceberViewModel, ContaReceberModel, ApplicationContext>
    {
        #region Herança
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override string getListName()
        {
            return "Listar Contas a Receber";
        }
        public override bool mustListOnLoad()
        {
            return false;
        }
        #endregion

        #region Master
        #region List
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {

            return ListParam(index, PageSize, Request["titulos_vencidos_atraso"], Request["dt_vencidos_atraso1"], Request["dt_vencidos_atraso2"], Request["titulos_a_vencer"], Request["dt_vencimento1"], Request["dt_vencimento2"],
                                Request["titulos_amortizados"], Request["titulos_nao_pagos"], Request["baixa_liquidacao"], Request["baixa_cancelamento"], Request["dt_baixa1"], Request["dt_baixa2"],
                                Request["clienteId"], Request["dt_emissao1"], Request["dt_emissao2"], Request["centroCustoId"], Request["grupoClienteId"], Request["bancoId"]);
        }

        [AuthorizeFilter]
        public ActionResult ListParam(int? index, int? pageSize = 50, string titulos_vencidos_atraso_ = null,
                                        string dt_vencidos_atraso1 = null, string dt_vencidos_atraso2 = null, string titulos_a_vencer_ = null,
                                        string dt_vencimento1 = null, string dt_vencimento2 = null, string titulos_amortizados_ = null,
                                        string titulos_nao_pagos_ = null, string baixa_liquidacao_ = null, string baixa_cancelamento_ = null,
                                        string dt_baixa1 = null, string dt_baixa2 = null, string clienteId = null,
                                        string dt_emissao1 = null, string dt_emissao2 = null, string centroCustoId = null, string grupoClienteId = null,
                                        string bancoId = null)
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
                ListViewContaReceber list = new ListViewContaReceber();
                return this._List(index, pageSize, "Browse", list, _titulos_vencidos_atraso, _dt_vencidos_atraso1, _dt_vencidos_atraso2, _titulos_a_vencer, _dt_vencimento1, _dt_vencimento2, _titulos_amortizados, _titulos_nao_pagos, _baixa_liquidacao,
                                    _baixa_cancelamento, _dt_baixa1, _dt_baixa2, _clienteId, _dt_emissao1, _dt_emissao2, _centroCustoId, _grupoId, _bancoId);
            }
            else
                return View();
        }
        #endregion

        #region edit
        [AuthorizeFilter]
        public ActionResult Edit(int operacaoId, int parcelaId)
        {
            BindBreadCrumb(getBreadCrumbText(null, null));
            EditarContaReceberViewModel value = new EditarContaReceberViewModel();
            try
            {
                value.operacaoId = operacaoId;
                value.parcelaId = parcelaId;
                Factory<EditarContaReceberViewModel, ApplicationContext> facade = new Factory<EditarContaReceberViewModel, ApplicationContext>();
                value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                value = facade.Execute(new ContaReceberEditarBI(), value);
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
        #endregion

        #region Baixar
        [AuthorizeFilter(Order = 1020)]
        public ActionResult _Baixar(int operacaoId, int parcelaId, int bancoId, string dt_pagamento, string dt_movto,
                                        string ind_forma_pagamento, string vr_juros_mora, string vr_multa_atraso,
                                        string vr_desconto, string vr_saldo_devedor, string vr_liquidacao, string vr_baixa, string fileupload,
                                        string cheque_banco, string cheque_agencia, string cheque_numero, int historicoId,
                                        string complementoHist, int? enquadramentoId, string vr_juros_mora_baixa1, string vr_multa_atraso_baixa1)
        {
            if (ViewBag.ValidateRequest)
            {
                EditarContaReceberViewModel value = new EditarContaReceberViewModel();
                try
                {
                    int? _cheque_banco = null;
                    if (cheque_banco != null && cheque_banco.Trim() != "")
                        _cheque_banco = int.Parse(cheque_banco);

                    value = new EditarContaReceberViewModel()
                    {
                        operacaoId = operacaoId,
                        parcelaId = parcelaId,
                        bancoId = bancoId,
                        dt_pagamento = Funcoes.StringToDate(dt_pagamento).Value,
                        dt_movto = Funcoes.StringToDate(dt_movto).Value,
                        ind_forma_pagamento = ind_forma_pagamento,
                        vr_encargos = vr_juros_mora != null && vr_juros_mora != "" ? decimal.Parse(vr_juros_mora) : 0,
                        vr_multa_atraso = vr_multa_atraso != null && vr_multa_atraso != "" ? decimal.Parse(vr_multa_atraso) : 0,
                        vr_desconto = vr_desconto != null && vr_desconto != "" ? decimal.Parse(vr_desconto) : 0,
                        vr_saldo_devedor = vr_saldo_devedor != null && vr_saldo_devedor != "" ? decimal.Parse(vr_saldo_devedor) : 0,
                        vr_baixa = vr_baixa != null && vr_baixa != "" ? decimal.Parse(vr_baixa) : 0,
                        cheque_banco = _cheque_banco,
                        cheque_agencia = cheque_agencia,
                        cheque_numero = cheque_numero,
                        ContaReceberParcelaEvento = new ContaReceberParcelaEventoViewModel()
                        {
                            operacaoId = operacaoId,
                            parcelaId = parcelaId,
                            dt_evento = Funcoes.Brasilia(),
                            enquadramentoId = enquadramentoId,
                            MovtoBancario = new MovtoBancarioViewModel()
                            {
                                bancoId = bancoId,
                                historicoId = historicoId,
                                complementoHist = complementoHist,
                                dt_movto = Funcoes.StringToDate(dt_movto).Value,
                                valor = vr_baixa != null && vr_baixa != "" ? decimal.Parse(vr_baixa) : 0,
                                tipoMovto = "C"
                            },
                            dt_ocorrencia = Funcoes.StringToDate(dt_pagamento).Value,
                            dt_movto = Funcoes.StringToDate(dt_movto).Value,
                            arquivo = fileupload,
                            ind_operacao = "D",
                            ind_estorno = "N"
                        }
                    };

                    Factory<EditarContaReceberViewModel, ApplicationContext> facade = new Factory<EditarContaReceberViewModel, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    value = facade.Execute(new BaixarContaReceberBI(), value, operacaoId, parcelaId).FirstOrDefault();

                    if (facade.Mensagem.Code > 0)
                        throw new App_DominioException(facade.Mensagem);

                    Success("Registro incluído com sucesso");
                }
                catch (App_DominioException ex)
                {
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    OnBaixaError(ref value, bancoId, dt_pagamento, dt_movto, ind_forma_pagamento, vr_juros_mora, vr_multa_atraso,
                                 vr_desconto, vr_liquidacao, vr_baixa, fileupload, cheque_banco, cheque_agencia, cheque_numero, historicoId,
                                 complementoHist, enquadramentoId);
                }
                catch (Exception ex)
                {
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    OnBaixaError(ref value, bancoId, dt_pagamento, dt_movto, ind_forma_pagamento, vr_juros_mora, vr_multa_atraso,
                                 vr_desconto, vr_liquidacao, vr_baixa, fileupload, cheque_banco, cheque_agencia, cheque_numero, historicoId,
                                 complementoHist, enquadramentoId);
                }
                return View("_Edit", value);
            }
            else
            {
                Error("Acesso para esta funcionalidade negado");
                return View();
            }
        }

        private void OnBaixaError(ref EditarContaReceberViewModel value, int bancoId, string dt_pagamento, string dt_movto,
                                  string ind_forma_pagamento, string vr_juros_mora, string vr_multa_atraso,
                                  string vr_desconto, string vr_liquidacao, string vr_baixa, string fileupload,
                                  string cheque_banco, string cheque_agencia, string cheque_numero, int historicoId,
                                  string complementoHist, int? enquadramentoId)
        {
            int? _cheque_banco = null;
            if (cheque_banco != null && cheque_banco.Trim() != "")
                _cheque_banco = int.Parse(cheque_banco);

            value.bancoId = bancoId;
            value.dt_pagamento = Funcoes.StringToDate(dt_pagamento);
            value.dt_movto = Funcoes.StringToDate(dt_movto);
            value.ind_forma_pagamento = ind_forma_pagamento;
            value.vr_juros_mora_baixa = vr_juros_mora != null && vr_juros_mora != "" ? decimal.Parse(vr_juros_mora) : 0;
            value.vr_multa_atraso_baixa = vr_multa_atraso != null && vr_multa_atraso != "" ? decimal.Parse(vr_multa_atraso) : 0;
            value.vr_desconto_baixa = vr_desconto != null && vr_desconto != "" ? decimal.Parse(vr_desconto) : 0;
            value.vr_baixa = vr_baixa != null && vr_baixa != "" ? decimal.Parse(vr_baixa) : 0;
            value.historicoId = historicoId;
            value.complementoHist = complementoHist;
            value.enquadramentoId = enquadramentoId;
            value.cheque_banco = _cheque_banco;
            value.cheque_agencia = cheque_agencia;
            value.cheque_numero = cheque_numero;
        }
        #endregion

        #region Novo Evento
        [AuthorizeFilter(Order = 1020)]
        public ActionResult _Evento(int operacaoId, int parcelaId, int? banco2Id, string dt_ocorrencia, string valor, string fileuploadComprovante,
                                        int historicoId, string complementoHist, int? eventoId, int? enquadramento_amortizacaoId)
        {
            EditarContaReceberViewModel result = new EditarContaReceberViewModel();
            if (ViewBag.ValidateRequest)
            {
                try
                {
                    EditarContaReceberParcelaEventoViewModel value = new EditarContaReceberParcelaEventoViewModel()
                    {
                        operacaoId = operacaoId,
                        parcelaId = parcelaId,
                        eventoId = eventoId.Value,
                        bancoId = banco2Id,
                        historicoId = historicoId,
                        complementoHist = complementoHist,
                        arquivo = fileuploadComprovante,
                        enquadramentoId = enquadramento_amortizacaoId,
                        dt_ocorrencia = Funcoes.StringToDate(dt_ocorrencia).Value,
                        valor = valor != null && valor != "" ? decimal.Parse(valor) : 0,
                    };

                    Factory<EditarContaReceberViewModel, ApplicationContext> facade = new Factory<EditarContaReceberViewModel, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    result = facade.Execute(new ContaReceberParcelaEventoBI(), value, operacaoId, parcelaId).FirstOrDefault();

                    if (facade.Mensagem.Code > 0)
                        throw new App_DominioException(facade.Mensagem);

                    Success("Registro incluído com sucesso");
                }
                catch (App_DominioException ex)
                {
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    OnEventoError(ref result, operacaoId, parcelaId, banco2Id, dt_ocorrencia, valor, fileuploadComprovante,
                                    historicoId, complementoHist, eventoId, enquadramento_amortizacaoId);
                }
                catch (Exception ex)
                {
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    OnEventoError(ref result, operacaoId, parcelaId, banco2Id, dt_ocorrencia, valor, fileuploadComprovante,
                                    historicoId, complementoHist, eventoId, enquadramento_amortizacaoId);
                }
                return View("_Edit", result);
            }
            else
            {
                result.operacaoId = operacaoId;
                result.parcelaId = parcelaId;
                Factory<EditarContaReceberViewModel, ApplicationContext> facade = new Factory<EditarContaReceberViewModel, ApplicationContext>();
                result = facade.Execute(new ContaReceberEditarBI(), result);
                Error("Acesso para esta funcionalidade negado");
                return View("_Edit", result);
            }
        }

        private void OnEventoError(ref EditarContaReceberViewModel value, int operacaoId, int parcelaId, int? banco2Id, string dt_ocorrencia,
                                    string valor, string fileuploadComprovante, int historicoId, string complementoHist, int? eventoId,
                                    int? enquadramento_amortizacaoId)
        {
            value.editarContaReceberParcelaEventoViewModel.operacaoId = operacaoId;
            value.editarContaReceberParcelaEventoViewModel.parcelaId = parcelaId;
            value.editarContaReceberParcelaEventoViewModel.bancoId = banco2Id;
            value.editarContaReceberParcelaEventoViewModel.dt_ocorrencia = Funcoes.StringToDate(dt_ocorrencia).Value;
            value.editarContaReceberParcelaEventoViewModel.valor = valor != null && valor != "" ? decimal.Parse(valor) : 0;
            value.editarContaReceberParcelaEventoViewModel.historicoId = historicoId;
            value.editarContaReceberParcelaEventoViewModel.complementoHist = complementoHist;
            value.editarContaReceberParcelaEventoViewModel.eventoId = eventoId ?? 0;
            value.editarContaReceberParcelaEventoViewModel.enquadramentoId = enquadramento_amortizacaoId ?? 0;
        }

        #endregion

        #region Estornar
        [AuthorizeFilter(Order = 1020)]
        public ActionResult Estornar(int operacaoId, int parcelaId, string dt_evento)
        {
            EditarContaReceberViewModel result = new EditarContaReceberViewModel();
            if (ViewBag.ValidateRequest)
            {
                try
                {
                    EditarContaReceberParcelaEventoViewModel value = new EditarContaReceberParcelaEventoViewModel()
                    {
                        operacaoId = operacaoId,
                        parcelaId = parcelaId,
                        dt_evento = Convert.ToDateTime(dt_evento)
                    };

                    Factory<EditarContaReceberViewModel, ApplicationContext> facade = new Factory<EditarContaReceberViewModel, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    result = facade.Execute(new ContaReceberEstornoBI(), value, operacaoId, parcelaId, dt_evento).FirstOrDefault();

                    if (facade.Mensagem.Code > 0)
                        throw new App_DominioException(facade.Mensagem);

                    Success("Registro incluído com sucesso");
                }
                catch (App_DominioException ex)
                {
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    OnEstornoError(ref result, operacaoId, parcelaId, dt_evento);
                }
                catch (Exception ex)
                {
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    OnEstornoError(ref result, operacaoId, parcelaId, dt_evento);
                }
                return View("_Edit", result);
            }
            else
            {
                result.operacaoId = operacaoId;
                result.parcelaId = parcelaId;
                Factory<EditarContaReceberViewModel, ApplicationContext> facade = new Factory<EditarContaReceberViewModel, ApplicationContext>();
                result = facade.Execute(new ContaReceberEditarBI(), result);
                Error("Acesso para esta funcionalidade negado");
                return View("_Edit", result);
            }
        }

        private void OnEstornoError(ref EditarContaReceberViewModel value, int operacaoId, int parcelaId, string dt_evento)
        {
            value.editarContaReceberParcelaEventoViewModel.operacaoId = operacaoId;
            value.editarContaReceberParcelaEventoViewModel.parcelaId = parcelaId;
            value.editarContaReceberParcelaEventoViewModel.dt_evento = Convert.ToDateTime(dt_evento);
        }
        #endregion

        #region Modify
        [AuthorizeFilter(Order = 1020)]
        public ActionResult Modify(int operacaoId, int parcelaId, int banco3Id, string dt_vencimento,
                                        string ind_forma_pagamento, string vr_principal, string num_titulo,
                                        string cheque_banco, string cheque_agencia, string cheque_numero, int historicoId,
                                        string complementoHist)
        {
            if (ViewBag.ValidateRequest)
            {
                EditarContaReceberViewModel value = new EditarContaReceberViewModel();
                try
                {
                    int? _cheque_banco = null;
                    if (cheque_banco != null && cheque_banco.Trim() != "")
                        _cheque_banco = int.Parse(cheque_banco);

                    value = new EditarContaReceberViewModel()
                    {
                        operacaoId = operacaoId,
                        parcelaId = parcelaId,
                        bancoId = banco3Id,
                        dt_vencimento = Funcoes.StringToDate(dt_vencimento).Value,
                        num_titulo = num_titulo,
                        ind_forma_pagamento = ind_forma_pagamento,
                        vr_principal = vr_principal != null && vr_principal != "" ? decimal.Parse(vr_principal) : 0,
                        cheque_banco = _cheque_banco,
                        cheque_agencia = cheque_agencia,
                        cheque_numero = cheque_numero,
                    };

                    Factory<EditarContaReceberViewModel, ApplicationContext> facade = new Factory<EditarContaReceberViewModel, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    value = facade.Execute(new ModifyContaReceberBI(), value, operacaoId, parcelaId).FirstOrDefault();

                    if (facade.Mensagem.Code > 0)
                        throw new App_DominioException(facade.Mensagem);

                    Success("Registro alterado com sucesso");
                }
                catch (App_DominioException ex)
                {
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    OnModifyError(ref value, banco3Id, dt_vencimento, num_titulo, ind_forma_pagamento, vr_principal,
                                    cheque_banco, cheque_agencia, cheque_numero, historicoId, complementoHist);
                }
                catch (Exception ex)
                {
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    OnModifyError(ref value, banco3Id, dt_vencimento, num_titulo, ind_forma_pagamento, vr_principal,
                                    cheque_banco, cheque_agencia, cheque_numero, historicoId, complementoHist);
                }
                return View("_Edit", value);
            }
            else
            {
                Error("Acesso para esta funcionalidade negado");
                return View();
            }
        }

        private void OnModifyError(ref EditarContaReceberViewModel value, int bancoId, string dt_vencimento, string num_titulo,
                                  string ind_forma_pagamento, string vr_principal,
                                  string cheque_banco, string cheque_agencia, string cheque_numero, int historicoId,
                                  string complementoHist)
        {
            int? _cheque_banco = null;
            if (cheque_banco != null && cheque_banco.Trim() != "")
                _cheque_banco = int.Parse(cheque_banco);

            value.bancoId = bancoId;
            value.dt_vencimento = Funcoes.StringToDate(dt_vencimento).Value;
            value.num_titulo = num_titulo;
            value.ind_forma_pagamento = ind_forma_pagamento;
            value.vr_principal = vr_principal != null && vr_principal != "" ? decimal.Parse(vr_principal) : 0;
            value.historicoId = historicoId;
            value.complementoHist = complementoHist;
            value.cheque_banco = _cheque_banco;
            value.cheque_agencia = cheque_agencia;
            value.cheque_numero = cheque_numero;
        }

        #endregion

        #region Index (Editar Operação)
        [AuthorizeFilter]
        public ActionResult Index()
        {
            if (ViewBag.ValidateRequest)
            {
                IDictionary<string, string> text = new Dictionary<string, string>();
                text.Add("edit", "Editar");
                text.Add("_index", "Editar Operação");
                BindBreadCrumb(getBreadCrumbText("Editar", text));

                return View();
            }
            else
            {
                Error("Acesso para esta funcionalidade negado");
                return View();
            }
        }

        [AuthorizeFilter]
        public ActionResult _Index(int operacaoId)
        {
            IDictionary<string, string> text = new Dictionary<string, string>();
            text.Add("edit", "Editar");
            text.Add("_index", "Editar Operação");
            BindBreadCrumb(getBreadCrumbText(null, text));

            if (ViewBag.ValidateRequest)
            {
                ContaReceberViewModel value = new ContaReceberViewModel() { operacaoId = operacaoId };
                Facade<ContaReceberViewModel, ContaReceberModel, ApplicationContext> facade = new Facade<ContaReceberViewModel, ContaReceberModel, ApplicationContext>();
                value = facade.getObject(value);
                return View(value);
            }
            else
            {
                Error("Acesso para esta funcionalidade negado");
                return View();
            }
        }

        [AuthorizeFilter]
        public ActionResult ListOperacaoParam(int? index, int? pageSize = 50, string clienteId = null, string dt_emissao1 = null, string dt_emissao2 = null)
        {
            ViewBag.ValidateRequest = true;
            if (ViewBag.ValidateRequest)
            {
                int? _null = null;
                int? _clienteId = clienteId != null && clienteId != "" ? int.Parse(clienteId) : _null;
                DateTime? _dt_emissao1 = Funcoes.StringToDate(dt_emissao1);
                DateTime? _dt_emissao2 = Funcoes.StringToDate(dt_emissao2);
                ListViewContaReceberBI list = new ListViewContaReceberBI();

                Factory<EditarContaReceberViewModel, ApplicationContext> factory = new Factory<EditarContaReceberViewModel, ApplicationContext>();
                IPagedList pagedList = factory.PagedList(list, index, pageSize.Value, _clienteId, _dt_emissao1, _dt_emissao2);
                return View(pagedList);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult Cancel(int operacaoId)
        {
            if (ViewBag.ValidateRequest)
            {
                ContaReceberViewModel value = new ContaReceberViewModel() { operacaoId = operacaoId };
                try
                {
                    Factory<ContaReceberViewModel, ApplicationContext> factory = new Factory<ContaReceberViewModel, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    value = factory.Execute(new ContaReceberCancelarOperacaoBI(), value, operacaoId).FirstOrDefault();
                    if (factory.Mensagem.Code > 0)
                        throw new App_DominioException(factory.Mensagem);

                    Success(factory.Mensagem.Message);
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

                BindBreadCrumb("Cancelar Operação");
                return View("_Index", value);
            }
            else
            {
                Error("Acesso para esta funcionalidade negado");
                return View("_EditarOperacao");
            }
        }

        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult DelOp(int operacaoId)
        {
            if (ViewBag.ValidateRequest)
            {
                ContaReceberViewModel value = new ContaReceberViewModel() { operacaoId = operacaoId };
                try
                {
                    Factory<ContaReceberViewModel, ApplicationContext> factory = new Factory<ContaReceberViewModel, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    value = factory.Execute(new ContaReceberExcluirOperacaoBI(), value, operacaoId).FirstOrDefault();
                    if (factory.Mensagem.Code > 0)
                        throw new App_DominioException(factory.Mensagem);

                    Success(factory.Mensagem.Message);
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

                return RedirectToAction("Browse");

                //    BindBreadCrumb("Excluir operação");
                //return View("Index");
            }
            else
            {
                Error("Acesso para esta funcionalidade negado");
                return View("_EditarOperacao");
            }
        }
        #endregion

        #region Liquidar
        [AuthorizeFilter]
        public ActionResult Liquidar(int operacaoId, int? bancoId, string dt_pagamento, string dt_movto,
                                        int? cheque_banco, string cheque_agencia, string cheque_numero, string vr_desconto, string vr_multa_atraso,
                                        string vr_juros_mora, int? enquadramentoId, string arquivo)
        {
            if (ViewBag.ValidateRequest)
            {
                ContaReceberViewModel value = new ContaReceberViewModel()
                {
                    operacaoId = operacaoId,
                    vr_jurosMora = vr_juros_mora != null && vr_juros_mora != "" ? decimal.Parse(vr_juros_mora) : 0,
                    vr_multa = vr_multa_atraso != null && vr_multa_atraso != "" ? decimal.Parse(vr_multa_atraso) : 0,
                    dt_movto = Funcoes.StringToDate(dt_movto),
                    enquadramentoId = enquadramentoId,
                    fileComprovante = arquivo,
                    ContaReceberParcela = new ContaReceberParcelaViewModel()
                    {
                        bancoId = bancoId,
                        ind_forma_pagamento = Request["ContaReceberParcela.ind_forma_pagamento"],
                        dt_baixa = Funcoes.StringToDate(dt_pagamento),
                        cheque_banco = cheque_banco,
                        cheque_agencia = cheque_agencia,
                        cheque_numero = cheque_numero,
                        vr_desconto = vr_desconto != null && vr_desconto != "" ? decimal.Parse(vr_desconto) : 0
                    }
                };
                try
                {
                    Factory<ContaReceberViewModel, ApplicationContext> factory = new Factory<ContaReceberViewModel, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    value = factory.Execute(new ContaReceberLiquidarOperacaoBI(), value, operacaoId).FirstOrDefault();
                    if (factory.Mensagem.Code > 0)
                        throw new App_DominioException(factory.Mensagem);

                    Success(factory.Mensagem.Message);
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
                BindBreadCrumb("Liquidar Operação");
                return View("_Index", value);
            }
            else
            {
                Error("Acesso para esta funcionalidade negado");
                return View("_EditarOperacao");
            }
        }
        #endregion

        public override void BeforeCreate(ref ContaReceberViewModel value, FormCollection collection)
        {
            if (collection["vr_jurosMora.1"] != null && collection["vr_jurosMora.1"] != "" && collection["vr_jurosMora.1"] != "0,00")
                value.vr_jurosMora = decimal.Parse(collection["vr_jurosMora.1"]);

            if (collection["vr_multa.1"] != null && collection["vr_multa.1"] != "" && collection["vr_multa.1"] != "0,00")
                value.vr_multa = decimal.Parse(collection["vr_multa.1"]);

            if (collection["enquadramentoId"] != null && collection["enquadramentoId"] != "")
                value.enquadramentoId = int.Parse(collection["enquadramentoId"]);

            if (collection["centroCustoId"] != null && collection["centroCustoId"] != "")
                value.centroCustoId = int.Parse(collection["centroCustoId"]);

            value.recorrencia = !value.recorrencia_mensal ? "N" : "S";
            value.clienteId = int.Parse(collection["clienteId"]);
            value.historicoId = int.Parse(collection["historicoId"]);
            value.fileBoleto = collection["fileBoleto"];
            value.fileComprovante = collection["fileComprovante"];
            if (collection["enquadramento_amortizacaoId"] != null && collection["enquadramento_amortizacaoId"] != "")
                value.enquadramento_amortizacaoId = int.Parse(collection["enquadramento_amortizacaoId"]);
            value.ContaReceberParcela.vr_principal = decimal.Parse(collection["ContaReceberParcela.vr_principal.1"]);
            value.ContaReceberParcelas = new List<ContaReceberParcelaViewModel>();

            #region Conta Receber Parcela
            value.ContaReceberParcela.parcelaId = 1;
            value.ContaReceberParcela.vr_encargos = 0;
            if (collection["bancoId"] != "" && collection["bancoId"] != "")
                value.ContaReceberParcela.bancoId = int.Parse(collection["bancoId"]);
            if (collection["ContaReceberParcela.ContaReceberParcelaEvento.valor.1"] != null && collection["ContaReceberParcela.ContaReceberParcelaEvento.valor.1"] != "")
                value.ContaReceberParcela.vr_amortizacao = decimal.Parse(collection["ContaReceberParcela.ContaReceberParcelaEvento.valor.1"]);
            if (collection["vr_desconto.1"] != null && collection["vr_desconto.1"] != "" && collection["vr_desconto.1"] != "0,00")
                value.ContaReceberParcela.vr_desconto = decimal.Parse(collection["vr_desconto.1"]);
            if (collection["vr_juros.1"] != null && collection["vr_juros.1"] != "" && collection["vr_juros.1"] != "0,00")
                value.ContaReceberParcela.vr_encargos = decimal.Parse(collection["vr_juros.1"]);
            if (collection["vr_mora.1"] != null && collection["vr_mora.1"] != "" && collection["vr_mora.1"] != "0,00")
                value.ContaReceberParcela.vr_encargos += decimal.Parse(collection["vr_mora.1"]);
            value.ContaReceberParcela.vr_saldo_devedor = value.ContaReceberParcela.vr_principal - value.ContaReceberParcela.vr_amortizacao;
            if (value.ContaReceberParcela.vr_amortizacao > 0 && collection["dt_ocorrencia"] != "")
                value.ContaReceberParcela.dt_ultima_amortizacao = Funcoes.StringToDate(collection["dt_ocorrencia"]);
            if (value.ContaReceberParcela.vr_amortizacao.HasValue && value.ContaReceberParcela.vr_amortizacao.Value == value.ContaReceberParcela.vr_principal)
            {
                value.ContaReceberParcela.ind_baixa = "4"; // baixa por motivo de liquidção
                value.ContaReceberParcela.dt_baixa = value.ContaReceberParcela.dt_ultima_amortizacao;
                value.ContaReceberParcela.ContaReceberParcelaEvento.dt_movto = Funcoes.StringToDate(collection["dt_movto"]).Value;
                value.dt_movto = Funcoes.StringToDate(collection["dt_movto"]).Value;
            }
            else if (!value.ContaReceberParcela.vr_amortizacao.HasValue || value.ContaReceberParcela.vr_amortizacao.Value == 0)
            {
                value.ContaReceberParcela.ContaReceberParcelaEvento.dt_movto = Funcoes.StringToDate("01/01/0001").Value;
                value.dt_movto = null;
            }
            value.ContaReceberParcela.dt_vencimento = Funcoes.StringToDate(collection["dt_vencimento"]).Value;
            #endregion

            if (value.num_parcelas > 1)
            {
                IEnumerable<App_Dominio.Component.Repository> r = GeraParcelas(value.ContaReceberParcela.num_titulo, collection["dt_vencimento"], value.ContaReceberParcela.ind_forma_pagamento, int.Parse(collection["bancoId"]), collection["nome_banco"],
                                                                               value.ContaReceberParcela.cheque_banco.ToString(), value.ContaReceberParcela.cheque_agencia, value.ContaReceberParcela.cheque_numero, collection["ContaReceberParcela.vr_principal.1"],
                                                                               collection["ContaReceberParcela.ContaReceberParcelaEvento.valor.1"], collection["vr_juros.1"], collection["vr_mora.1"], collection["vr_desconto.1"],
                                                                               int.Parse(collection["num_parcelas"]));

                int index = 1;
                foreach (ContaReceberParcelaViewModel par in r)
                {
                    if (collection["bancoId"] != "" && collection["bancoId"] != "")
                        par.bancoId = int.Parse(collection["bancoId"]);

                    if (index == 1)
                    {
                        if (par.vr_amortizacao > 0 && collection["dt_ocorrencia"] != "")
                            par.dt_ultima_amortizacao = Funcoes.StringToDate(collection["dt_ocorrencia"]);

                        if (par.vr_amortizacao.HasValue && par.vr_amortizacao.Value == par.vr_principal)
                        {
                            par.ind_baixa = "4"; // baixa por motivo de liquidção
                            par.dt_baixa = par.dt_ultima_amortizacao;
                        }
                    }

                    ((List<ContaReceberParcelaViewModel>)value.ContaReceberParcelas).Add(par);
                    index++;
                }
            }
            else
            {
                ContaReceberParcelaViewModel cp = new ContaReceberParcelaViewModel()
                {
                    parcelaId = value.ContaReceberParcela.parcelaId,
                    bancoId = value.ContaReceberParcela.bancoId,
                    nome_banco = value.ContaReceberParcela.nome_banco,
                    num_titulo = value.ContaReceberParcela.num_titulo,
                    dt_vencimento = value.ContaReceberParcela.dt_vencimento,
                    vr_principal = value.ContaReceberParcela.vr_principal,
                    vr_amortizacao = value.ContaReceberParcela.vr_amortizacao,
                    vr_desconto = value.ContaReceberParcela.vr_desconto,
                    vr_encargos = value.ContaReceberParcela.vr_encargos,
                    vr_saldo_devedor = value.ContaReceberParcela.vr_saldo_devedor,
                    ind_forma_pagamento = value.ContaReceberParcela.ind_forma_pagamento,
                    codigo_barras = value.ContaReceberParcela.codigo_barras,
                    dt_ultima_amortizacao = value.ContaReceberParcela.dt_ultima_amortizacao,
                    ind_baixa = value.ContaReceberParcela.ind_baixa,
                    dt_baixa = value.ContaReceberParcela.dt_baixa,
                    cheque_banco = value.ContaReceberParcela.cheque_banco,
                    cheque_agencia = value.ContaReceberParcela.cheque_agencia,
                    cheque_numero = value.ContaReceberParcela.cheque_numero
                };

                ((List<ContaReceberParcelaViewModel>)value.ContaReceberParcelas).Add(cp);
            }
        }

        public override void OnCreateError(ref ContaReceberViewModel value, FormCollection collection)
        {
            if (collection["clienteId"] != null)
                value.clienteId = int.Parse(collection["clienteId"]);

            value.nome_cliente = collection["nome_cliente"];

            if (collection["historicoId"] != null && collection["historicoId"] != "")
                value.historicoId = int.Parse(collection["historicoId"]);

            value.descricao_historico = collection["descricao_historico"];

            if (collection["centroCustoId"] != null && collection["centroCustoId"] != "")
            {
                value.centroCustoId = int.Parse(collection["centroCustoId"]);
                value.descricao_centroCusto = collection["descricao_centroCusto"];
            }

            if (collection["enquadramentoId"] != null && collection["enquadramentoId"] != "")
            {
                value.enquadramentoId = int.Parse(collection["enquadramentoId"]);
                value.descricao_enquadramento = collection["descricao_enquadramento"];
            }

            if (collection["enquadramento_amortizacaoId"] != null && collection["enquadramento_amortizacaoId"] != "")
            {
                value.enquadramento_amortizacaoId = int.Parse(collection["enquadramento_amortizacaoId"]);
                value.descricao_enquadramentoAmortizacao = collection["descricao_enquadramento_amortizacao"];
            }

            value.ContaReceberParcela = new ContaReceberParcelaViewModel()
            {
                parcelaId = 1,
                dt_vencimento = collection["dt_vencimento"] != null && collection["dt_vencimento"] != "" ? DateTime.Parse(collection["dt_vencimento"]) : Funcoes.Brasilia().Date,
                ContaReceberParcelaEvento = new ContaReceberParcelaEventoViewModel()
                {
                    parcelaId = 1,
                    dt_ocorrencia = collection["dt_ocorrencia"] != null && collection["dt_ocorrencia"] != "" ? DateTime.Parse(collection["dt_ocorrencia"]) : Funcoes.Brasilia().Date,
                    dt_movto = collection["dt_movto"] != null && collection["dt_movto"] != "" ? DateTime.Parse(collection["dt_movto"]) : Funcoes.Brasilia().Date
                }
            };

            if (collection["bancoId"] != null && collection["bancoId"] != "")
            {
                value.ContaReceberParcela.bancoId = int.Parse(collection["bancoId"]);
                value.ContaReceberParcela.nome_banco = collection["nome_banco"];
            }

            if (collection["fileBoleto"] != null)
                value.fileBoleto = collection["fileBoleto"];

            if (collection["fileComprovante"] != null)
                value.fileComprovante = collection["fileComprovante"];

            if (collection["ContaReceberParcela.vr_principal.1"] != null && collection["ContaReceberParcela.vr_principal.1"] != "")
                value.ContaReceberParcela.vr_principal = Decimal.Parse(collection["ContaReceberParcela.vr_principal.1"]);

            if (collection["ContaReceberParcela.ContaReceberParcelaEvento.valor.1"] != null && collection["ContaReceberParcela.ContaReceberParcelaEvento.valor.1"] != "")
                value.ContaReceberParcela.ContaReceberParcelaEvento.valor = decimal.Parse(collection["ContaReceberParcela.ContaReceberParcelaEvento.valor.1"]);
            if (collection["vr_jurosMora.1"] != null && collection["vr_jurosMora.1"] != "")
                value.vr_jurosMora = decimal.Parse(collection["vr_jurosMora.1"]);
            if (collection["vr_multa.1"] != null && collection["vr_multa.1"] != "")
                value.vr_multa = decimal.Parse(collection["vr_multa.1"]);

            value.num_parcelas = int.Parse(collection["num_parcelas"]);

            if (collection["vr_desconto.1"] != null && collection["vr_desconto.1"] != "" && collection["vr_desconto.1"] != "0,00")
                value.ContaReceberParcela.vr_desconto = decimal.Parse(collection["vr_desconto.1"]);
            if (collection["vr_juros.1"] != null && collection["vr_juros.1"] != "" && collection["vr_juros.1"] != "0,00")
                value.ContaReceberParcela.vr_encargos = decimal.Parse(collection["vr_juros.1"]);
            if (collection["vr_mora.1"] != null && collection["vr_mora.1"] != "" && collection["vr_mora.1"] != "0,00")
                value.ContaReceberParcela.vr_encargos += decimal.Parse(collection["vr_mora.1"]);
        }
        #endregion

        #region Gerar Parcelas
        public ActionResult ListParcelas(string num_titulo, string dt_vencimento, string ind_forma_pagamento, int bancoId, string nome_banco, string cheque_banco, string cheque_agencia, string cheque_numero, string vr_principal, string vr_amortizacao,
                                            string vr_juros, string vr_mora, string vr_desconto, int num_parcelas)
        {
            return View(GeraParcelas(num_titulo, dt_vencimento, ind_forma_pagamento, bancoId, nome_banco, cheque_banco, cheque_agencia,
                                        cheque_numero, vr_principal, vr_amortizacao, vr_juros, vr_mora, vr_desconto, num_parcelas));
        }

        private IEnumerable<App_Dominio.Component.Repository> GeraParcelas(string num_titulo, string dt_vencimento, string ind_forma_pagamento,
                                                                           int bancoId, string nome_banco, string cheque_banco, string cheque_agencia, string cheque_numero,
                                                                           string vr_principal, string vr_amortizacao,
                                                                           string vr_juros, string vr_mora, string vr_desconto, int num_parcelas)
        {
            ViewBag.ValidateRequest = true;
            DateTime _dt_vencimento = Convert.ToDateTime(dt_vencimento.Substring(6, 4) + "-" + dt_vencimento.Substring(3, 2) + "-" + dt_vencimento.Substring(0, 2));
            int? _cheque_banco = null;
            if (cheque_banco != null && cheque_banco != "")
                _cheque_banco = int.Parse(cheque_banco);
            decimal _vr_principal = Convert.ToDecimal(vr_principal);
            decimal _vr_amortizacao = Convert.ToDecimal(vr_amortizacao);
            decimal _vr_juros = Convert.ToDecimal(vr_juros);
            decimal _vr_mora = Convert.ToDecimal(vr_mora);
            decimal _vr_desconto = Convert.ToDecimal(vr_desconto);

            ListViewContaReceberParcela list = new ListViewContaReceberParcela();

            return list.ListRepository(0, 50, num_titulo, _dt_vencimento, ind_forma_pagamento, nome_banco, _cheque_banco, cheque_agencia, cheque_numero, _vr_principal, _vr_amortizacao, _vr_juros, _vr_mora, _vr_desconto, num_parcelas, bancoId);
        }

        #endregion

        #region MovtoBancario
        public JsonResult getMovtoBancario(int movtoBancarioId)
        {
            Facade<MovtoBancarioViewModel, MovtoBancarioModel, ApplicationContext> facade = new Facade<MovtoBancarioViewModel, MovtoBancarioModel, ApplicationContext>();
            MovtoBancarioViewModel r = facade.getObject(new MovtoBancarioViewModel() { movtoBancarioId = movtoBancarioId });
            if (r.mensagem.Code == 0)
                return new JsonResult()
                {
                    Data = r,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            else
                return new JsonResult()
                {
                    Data = new MovtoBancarioViewModel(),
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
        }

        #endregion

        #region Contabilidade
        public JsonResult getContabilidade(int contabilidadeId)
        {
            Facade<ContabilidadeViewModel, ContabilidadeModel, ApplicationContext> facade = new Facade<ContabilidadeViewModel, ContabilidadeModel, ApplicationContext>();
            ContabilidadeViewModel r = facade.getObject(new ContabilidadeViewModel() { contabilidadeId = contabilidadeId });
            if (r.mensagem.Code == 0)
                return new JsonResult()
                {
                    Data = r.ContabilidadeItems.ToList(),
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            else
                return new JsonResult()
                {
                    Data = new ContabilidadeItemViewModel(),
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
        }

        #endregion

        public JsonResult getNames()
        {
            return JSonTypeahead(null, new ListViewContaReceber());
        }
    }
}