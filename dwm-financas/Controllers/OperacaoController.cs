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
    public abstract class OperacaoController<EORepo, EOPERepo, ORepo, OModel, OPModel, OPEModel, OPRepo, OPERepo, O, OP, OPE, EditarBI, BaixarBI,
                                             EventoBI, EstornoBI, ModifyBI, CancelarBI, ExcluirBI, LiquidarBI> : DwmRootController<ORepo, OModel, ApplicationContext>
        where EORepo : EditarOperacaoViewModel<EOPERepo>
        where EOPERepo : EditarOperacaoParcelaEventoViewModel
        where ORepo : OperacaoViewModel<OPRepo,OPERepo>
        where OModel : OperacaoModel<O, ORepo, OP, OPRepo,OPE, OPERepo, OPModel, OPEModel>
        where OPModel : OperacaoParcelaCrudModel<OP, OPRepo, OPE, OPERepo, OPEModel>
        where OPEModel : OperacaoParcelaEventoModel<OPE, OPERepo>
        where OPRepo : OperacaoParcelaViewModel<OPERepo>
        where OPERepo : OperacaoParcelaEventoViewModel
        where O : Operacao<OP, OPE>
        where OP : OperacaoParcela<OPE>
        where OPE : OperacaoParcelaEvento
        where EditarBI : OperacaoEditarBI<EORepo, EOPERepo>
        where BaixarBI : OperacaoBaixarBI<EORepo, EOPERepo, ORepo,OPRepo,OPERepo,OPEModel,OPModel,OModel,OPE,OP,O,EditarBI>
        where EventoBI : OperacaoParcelaEventoBI<EORepo, EOPERepo,ORepo,OPRepo,OPERepo,OPEModel,OPE,EditarBI>
        where EstornoBI : OperacaoEstornoBI<EORepo,EOPERepo,OPModel,OPEModel,OPRepo,OPERepo,OP,OPE,EditarBI>
        where ModifyBI : OperacaoModifyBI<EORepo,EOPERepo,OPRepo,OPERepo,OPModel,OPEModel, OP, OPE, EditarBI>
        where CancelarBI : OperacaoCancelarBI<ORepo, OPRepo, OPERepo, OModel, OPModel, OPEModel, O, OP, OPE>
        where ExcluirBI : OperacaoExcluirBI<ORepo, OPRepo, OPERepo, OModel, OPModel, OPEModel, O, OP, OPE>
        where LiquidarBI : OperacaoLiquidarBI<ORepo, OPRepo, OPERepo, OModel, OPModel, OPEModel, O, OP, OPE>
    {
        #region Herança
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override bool mustListOnLoad()
        {
            return false;
        }
        #endregion

        #region constructor
        protected EORepo getEditarOperacaoRepositoryInstance()
        {
            Type typeInstance = typeof(EORepo);
            return (EORepo)Activator.CreateInstance(typeInstance);
        }
        protected EOPERepo getEditarOperacaoParcelaEventoRepositoryInstance()
        {
            Type typeInstance = typeof(EOPERepo);
            return (EOPERepo)Activator.CreateInstance(typeInstance);
        }
        protected ORepo getOperacaoRepositoryInstance()
        {
            Type typeInstance = typeof(ORepo);
            return (ORepo)Activator.CreateInstance(typeInstance);
        }
        protected OPRepo getOperacaoParcelaRepositoryInstance()
        {
            Type typeInstance = typeof(OPRepo);
            return (OPRepo)Activator.CreateInstance(typeInstance);
        }
        protected OPERepo getOperacaoParcelaEventoRepositoryInstance()
        {
            Type typeInstance = typeof(OPERepo);
            return (OPERepo)Activator.CreateInstance(typeInstance);
        }
        protected EditarBI getOperacaoEditarBIInstance()
        {
            Type typeInstance = typeof(EditarBI);
            return (EditarBI)Activator.CreateInstance(typeInstance);
        }
        protected BaixarBI getOperacaoBaixarBIInstance()
        {
            Type typeInstance = typeof(BaixarBI);
            return (BaixarBI)Activator.CreateInstance(typeInstance);
        }
        protected EventoBI getOperacaoParcelaEventoBIInstance()
        {
            Type typeInstance = typeof(EventoBI);
            return (EventoBI)Activator.CreateInstance(typeInstance);
        }
        protected EstornoBI getOperacaoEstornoBIInstance()
        {
            Type typeInstance = typeof(EstornoBI);
            return (EstornoBI)Activator.CreateInstance(typeInstance);
        }
        protected ModifyBI getOperacaoModifyBIInstance()
        {
            Type typeInstance = typeof(ModifyBI);
            return (ModifyBI)Activator.CreateInstance(typeInstance);
        }
        protected CancelarBI getOperacaoCancelarBIInstance()
        {
            Type typeInstance = typeof(CancelarBI);
            return (CancelarBI)Activator.CreateInstance(typeInstance);
        }
        protected ExcluirBI getOperacaoExcluirBIInstance()
        {
            Type typeInstance = typeof(ExcluirBI);
            return (ExcluirBI)Activator.CreateInstance(typeInstance);
        }
        protected LiquidarBI getOperacaoELiquidarBIInstance()
        {
            Type typeInstance = typeof(LiquidarBI);
            return (LiquidarBI)Activator.CreateInstance(typeInstance);
        }
        protected ListViewOperacaoParcela<OPRepo, OPERepo> getListViewOperacaoParcelaInstance()
        {
            Type typeInstance = typeof(ListViewOperacaoParcela<OPRepo, OPERepo>);
            return (ListViewOperacaoParcela<OPRepo, OPERepo>)Activator.CreateInstance(typeInstance);
        }
        #endregion

        #region Abstract Methods
        protected abstract string getTipoMovto();
        protected abstract IEnumerable<App_Dominio.Component.Repository> getParc(ref ORepo value, FormCollection collection);
        protected abstract void Add(ref ORepo value, OPRepo par);
        protected abstract void Modify(ref ORepo value, FormCollection collection);
        protected abstract IEnumerable<App_Dominio.Component.Repository> getParcelasl(string num_titulo, string dt_vencimento, string ind_forma_pagamento,
                                                                                       int bancoId, string nome_banco, string cheque_banco, string cheque_agencia, string cheque_numero,
                                                                                       string vr_principal, string vr_amortizacao,
                                                                                       string vr_juros, string vr_mora, string vr_desconto, int num_parcelas);
        #endregion

        #region Master
        #region edit
        [AuthorizeFilter]
        public ActionResult Edit(int operacaoId, int parcelaId)
        {
            BindBreadCrumb(getBreadCrumbText(null, null));
            EORepo value = getEditarOperacaoRepositoryInstance();
            try
            {
                value.operacaoId = operacaoId;
                value.parcelaId = parcelaId;
                Factory<EORepo, ApplicationContext> facade = new Factory<EORepo, ApplicationContext>();
                value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                value = facade.Execute(getOperacaoEditarBIInstance(), value);
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
                EORepo value = getEditarOperacaoRepositoryInstance();
                try
                {
                    int? _cheque_banco = null;
                    if (cheque_banco != null && cheque_banco.Trim() != "")
                        _cheque_banco = int.Parse(cheque_banco);
                    value.operacaoId = operacaoId;
                    value.parcelaId = parcelaId;
                    value.bancoId = bancoId;
                    value.dt_pagamento = Funcoes.StringToDate(dt_pagamento).Value;
                    value.dt_movto = Funcoes.StringToDate(dt_movto).Value;
                    value.ind_forma_pagamento = ind_forma_pagamento;
                    value.vr_encargos = vr_juros_mora != null && vr_juros_mora != "" ? decimal.Parse(vr_juros_mora) : 0;
                    value.vr_multa_atraso = vr_multa_atraso != null && vr_multa_atraso != "" ? decimal.Parse(vr_multa_atraso) : 0;
                    value.vr_desconto = vr_desconto != null && vr_desconto != "" ? decimal.Parse(vr_desconto) : 0;
                    value.vr_saldo_devedor = vr_saldo_devedor != null && vr_saldo_devedor != "" ? decimal.Parse(vr_saldo_devedor) : 0;
                    value.vr_baixa = vr_baixa != null && vr_baixa != "" ? decimal.Parse(vr_baixa) : 0;
                    value.cheque_banco = _cheque_banco;
                    value.cheque_agencia = cheque_agencia;
                    value.cheque_numero = cheque_numero;

                    value.OperacaoParcelaEvento = getOperacaoParcelaEventoRepositoryInstance();

                    value.OperacaoParcelaEvento.operacaoId = operacaoId;
                    value.OperacaoParcelaEvento.parcelaId = parcelaId;
                    value.OperacaoParcelaEvento.dt_evento = Funcoes.Brasilia();
                    value.OperacaoParcelaEvento.enquadramentoId = enquadramentoId;

                    value.OperacaoParcelaEvento.MovtoBancario = new MovtoBancarioViewModel()
                    {
                        bancoId = bancoId,
                        historicoId = historicoId,
                        complementoHist = complementoHist,
                        dt_movto = Funcoes.StringToDate(dt_movto).Value,
                        valor = vr_baixa != null && vr_baixa != "" ? decimal.Parse(vr_baixa) : 0,
                        tipoMovto = getTipoMovto()
                    };

                    value.OperacaoParcelaEvento.dt_ocorrencia = Funcoes.StringToDate(dt_pagamento).Value;
                    value.OperacaoParcelaEvento.dt_movto = Funcoes.StringToDate(dt_movto).Value;
                    value.OperacaoParcelaEvento.arquivo = fileupload;
                    value.OperacaoParcelaEvento.ind_operacao = "D";
                    value.OperacaoParcelaEvento.ind_estorno = "N";

                    Factory<EORepo, ApplicationContext> facade = new Factory<EORepo, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    value = facade.Execute(getOperacaoBaixarBIInstance(), value, operacaoId, parcelaId).FirstOrDefault();

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

        private void OnBaixaError(ref EORepo value, int bancoId, string dt_pagamento, string dt_movto,
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
            EORepo result = getEditarOperacaoRepositoryInstance();
            if (ViewBag.ValidateRequest)
            {
                try
                {
                    EOPERepo value = getEditarOperacaoParcelaEventoRepositoryInstance();
                    value.operacaoId = operacaoId;
                    value.parcelaId = parcelaId;
                    value.eventoId = eventoId.Value;
                    value.bancoId = banco2Id;
                    value.historicoId = historicoId;
                    value.complementoHist = complementoHist;
                    value.arquivo = fileuploadComprovante;
                    value.enquadramentoId = enquadramento_amortizacaoId;
                    value.dt_ocorrencia = Funcoes.StringToDate(dt_ocorrencia).Value;
                    value.valor = valor != null && valor != "" ? decimal.Parse(valor) : 0;

                    Factory<EORepo, ApplicationContext> facade = new Factory<EORepo, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    result = facade.Execute(getOperacaoParcelaEventoBIInstance(), value, operacaoId, parcelaId).FirstOrDefault();

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
                Factory<EORepo, ApplicationContext> facade = new Factory<EORepo, ApplicationContext>();
                result = facade.Execute(getOperacaoEditarBIInstance(), result);
                Error("Acesso para esta funcionalidade negado");
                return View("_Edit", result);
            }
        }

        private void OnEventoError(ref EORepo value, int operacaoId, int parcelaId, int? banco2Id, string dt_ocorrencia,
                                    string valor, string fileuploadComprovante, int historicoId, string complementoHist, int? eventoId,
                                    int? enquadramento_amortizacaoId)
        {
            value.editarOperacaoParcelaEventoViewModel.operacaoId = operacaoId;
            value.editarOperacaoParcelaEventoViewModel.parcelaId = parcelaId;
            value.editarOperacaoParcelaEventoViewModel.bancoId = banco2Id;
            value.editarOperacaoParcelaEventoViewModel.dt_ocorrencia = Funcoes.StringToDate(dt_ocorrencia).Value;
            value.editarOperacaoParcelaEventoViewModel.valor = valor != null && valor != "" ? decimal.Parse(valor) : 0;
            value.editarOperacaoParcelaEventoViewModel.historicoId = historicoId;
            value.editarOperacaoParcelaEventoViewModel.complementoHist = complementoHist;
            value.editarOperacaoParcelaEventoViewModel.eventoId = eventoId ?? 0;
            value.editarOperacaoParcelaEventoViewModel.enquadramentoId = enquadramento_amortizacaoId ?? 0;
        }

        #endregion

        #region Estornar
        [AuthorizeFilter(Order = 1020)]
        public ActionResult Estornar(int operacaoId, int parcelaId, string dt_evento)
        {
            EORepo result = getEditarOperacaoRepositoryInstance();
            if (ViewBag.ValidateRequest)
            {
                try
                {
                    EOPERepo value = getEditarOperacaoParcelaEventoRepositoryInstance();
                    value.operacaoId = operacaoId;
                    value.parcelaId = parcelaId;
                    value.dt_evento = Convert.ToDateTime(dt_evento);

                    Factory<EORepo, ApplicationContext> facade = new Factory<EORepo, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    result = facade.Execute(getOperacaoEstornoBIInstance(), value, operacaoId, parcelaId, dt_evento).FirstOrDefault();

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
                Factory<EORepo, ApplicationContext> facade = new Factory<EORepo, ApplicationContext>();
                result = facade.Execute(getOperacaoEditarBIInstance(), result);
                Error("Acesso para esta funcionalidade negado");
                return View("_Edit", result);
            }
        }

        private void OnEstornoError(ref EORepo value, int operacaoId, int parcelaId, string dt_evento)
        {
            value.editarOperacaoParcelaEventoViewModel.operacaoId = operacaoId;
            value.editarOperacaoParcelaEventoViewModel.parcelaId = parcelaId;
            value.editarOperacaoParcelaEventoViewModel.dt_evento = Convert.ToDateTime(dt_evento);
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
                EORepo value = getEditarOperacaoRepositoryInstance();
                try
                {
                    int? _cheque_banco = null;
                    if (cheque_banco != null && cheque_banco.Trim() != "")
                        _cheque_banco = int.Parse(cheque_banco);

                    value.operacaoId = operacaoId;
                    value.parcelaId = parcelaId;
                    value.bancoId = banco3Id;
                    value.dt_vencimento = Funcoes.StringToDate(dt_vencimento).Value;
                    value.num_titulo = num_titulo;
                    value.ind_forma_pagamento = ind_forma_pagamento;
                    value.vr_principal = vr_principal != null && vr_principal != "" ? decimal.Parse(vr_principal) : 0;
                    value.cheque_banco = _cheque_banco;
                    value.cheque_agencia = cheque_agencia;
                    value.cheque_numero = cheque_numero;

                    Factory<EORepo, ApplicationContext> facade = new Factory<EORepo, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    value = facade.Execute(getOperacaoModifyBIInstance(), value, operacaoId, parcelaId).FirstOrDefault();

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

        private void OnModifyError(ref EORepo value, int bancoId, string dt_vencimento, string num_titulo,
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
                ORepo value = getOperacaoRepositoryInstance();
                value.operacaoId = operacaoId;
                Facade<ORepo, OModel, ApplicationContext> facade = new Facade<ORepo, OModel, ApplicationContext>();
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
        public ActionResult Cancel(int operacaoId)
        {
            if (ViewBag.ValidateRequest)
            {
                ORepo value = getOperacaoRepositoryInstance();
                value.operacaoId = operacaoId;
                try
                {
                    Factory<ORepo, ApplicationContext> factory = new Factory<ORepo, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    value = factory.Execute(getOperacaoCancelarBIInstance(), value, operacaoId).FirstOrDefault();
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
                ORepo value = getOperacaoRepositoryInstance();
                value.operacaoId = operacaoId;
                try
                {
                    Factory<ORepo, ApplicationContext> factory = new Factory<ORepo, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    value = factory.Execute(getOperacaoExcluirBIInstance(), value, operacaoId).FirstOrDefault();
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
                ORepo value = getOperacaoRepositoryInstance();
                value.operacaoId = operacaoId;
                value.vr_jurosMora = vr_juros_mora != null && vr_juros_mora != "" ? decimal.Parse(vr_juros_mora) : 0;
                value.vr_multa = vr_multa_atraso != null && vr_multa_atraso != "" ? decimal.Parse(vr_multa_atraso) : 0;
                value.dt_movto = Funcoes.StringToDate(dt_movto);
                value.enquadramentoId = enquadramentoId;
                value.fileComprovante = arquivo;
                value.OperacaoParcela = getOperacaoParcelaRepositoryInstance();
                value.OperacaoParcela.bancoId = bancoId;
                value.OperacaoParcela.ind_forma_pagamento = Request["OperacaoParcela.ind_forma_pagamento"];
                value.OperacaoParcela.dt_baixa = Funcoes.StringToDate(dt_pagamento);
                value.OperacaoParcela.cheque_banco = cheque_banco;
                value.OperacaoParcela.cheque_agencia = cheque_agencia;
                value.OperacaoParcela.cheque_numero = cheque_numero;
                value.OperacaoParcela.vr_desconto = vr_desconto != null && vr_desconto != "" ? decimal.Parse(vr_desconto) : 0;

                try
                {
                    Factory<ORepo, ApplicationContext> factory = new Factory<ORepo, ApplicationContext>();
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                    value = factory.Execute(getOperacaoELiquidarBIInstance(), value, operacaoId).FirstOrDefault();
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

        public override void BeforeCreate(ref ORepo value, FormCollection collection)
        {
            if (collection["OperacaoParcela_OperacaoParcelaEvento_valor"] != "")
                value.OperacaoParcela.vr_amortizacao = decimal.Parse(collection["OperacaoParcela_OperacaoParcelaEvento_valor"]);

            if (collection["vr_jurosMora"] != null && collection["vr_jurosMora"] != "" && collection["vr_jurosMora"] != "0,00")
                value.vr_jurosMora = decimal.Parse(collection["vr_jurosMora"]);

            if (collection["vr_multa"] != null && collection["vr_multa"] != "" && collection["vr_multa"] != "0,00")
                value.vr_multa = decimal.Parse(collection["vr_multa"]);

            if (collection["enquadramentoId"] != null && collection["enquadramentoId"] != "")
                value.enquadramentoId = int.Parse(collection["enquadramentoId"]);

            if (collection["centroCustoId"] != null && collection["centroCustoId"] != "")
                value.centroCustoId = int.Parse(collection["centroCustoId"]);

            value.recorrencia = !value.recorrencia_mensal ? "N" : "S";
            value.historicoId = int.Parse(collection["historicoId"]);
            value.fileBoleto = collection["fileBoleto"];
            value.fileComprovante = collection["fileComprovante"];
            if (collection["enquadramento_amortizacaoId"] != null && collection["enquadramento_amortizacaoId"] != "")
                value.enquadramento_amortizacaoId = int.Parse(collection["enquadramento_amortizacaoId"]);
            value.OperacaoParcela.vr_principal = decimal.Parse(collection["OperacaoParcela_vr_principal"]);
            value.OperacaoParcelas = new List<OPRepo>();

            #region Parcela
            value.OperacaoParcela.parcelaId = 1;
            value.OperacaoParcela.vr_encargos = 0;
            if (collection["bancoId"] != "" && collection["bancoId"] != "")
                value.OperacaoParcela.bancoId = int.Parse(collection["bancoId"]);
            if (collection["vr_desconto"] != null && collection["vr_desconto"] != "" && collection["vr_desconto"] != "0,00")
                value.OperacaoParcela.vr_desconto = decimal.Parse(collection["vr_desconto"]);
            if (collection["vr_juros"] != null && collection["vr_juros"] != "" && collection["vr_juros"] != "0,00")
                value.OperacaoParcela.vr_encargos = decimal.Parse(collection["vr_juros"]);
            if (collection["vr_mora"] != null && collection["vr_mora"] != "" && collection["vr_mora"] != "0,00")
                value.OperacaoParcela.vr_encargos += decimal.Parse(collection["vr_mora"]);
            value.OperacaoParcela.vr_saldo_devedor = value.OperacaoParcela.vr_principal - value.OperacaoParcela.vr_amortizacao;
            if (value.OperacaoParcela.vr_amortizacao > 0 && collection["dt_ocorrencia"] != "")
                value.OperacaoParcela.dt_ultima_amortizacao = Funcoes.StringToDate(collection["dt_ocorrencia"]);
            if (value.OperacaoParcela.vr_amortizacao.HasValue && value.OperacaoParcela.vr_amortizacao.Value == value.OperacaoParcela.vr_principal)
            {
                value.OperacaoParcela.ind_baixa = "4"; // baixa por motivo de liquidção
                value.OperacaoParcela.dt_baixa = value.OperacaoParcela.dt_ultima_amortizacao;
                value.OperacaoParcela.OperacaoParcelaEvento.dt_movto = Funcoes.StringToDate(collection["dt_movto"]).Value;
                value.dt_movto = Funcoes.StringToDate(collection["dt_movto"]).Value;
            }
            else if (!value.OperacaoParcela.vr_amortizacao.HasValue || value.OperacaoParcela.vr_amortizacao.Value == 0)
            {
                value.OperacaoParcela.OperacaoParcelaEvento.dt_movto = Funcoes.StringToDate("01/01/0001").Value;
                value.dt_movto = null;
            }
            value.OperacaoParcela.dt_vencimento = Funcoes.StringToDate(collection["dt_vencimento"]).Value;

            #endregion

            if (value.num_parcelas > 1)
            {
                IEnumerable<App_Dominio.Component.Repository> r = getParc(ref value, collection);

                int index = 1;
                foreach (OPRepo par in r)
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

                    Add(ref value, par);

                    index++;
                }
            }
            else
            {
                OPRepo par = getOperacaoParcelaRepositoryInstance();

                par.parcelaId = value.OperacaoParcela.parcelaId;
                par.bancoId = value.OperacaoParcela.bancoId;
                par.nome_banco = value.OperacaoParcela.nome_banco;
                par.num_titulo = value.OperacaoParcela.num_titulo;
                par.dt_vencimento = value.OperacaoParcela.dt_vencimento;
                par.vr_principal = value.OperacaoParcela.vr_principal;
                par.vr_amortizacao = value.OperacaoParcela.vr_amortizacao;
                par.vr_desconto = value.OperacaoParcela.vr_desconto;
                par.vr_encargos = value.OperacaoParcela.vr_encargos;
                par.vr_saldo_devedor = value.OperacaoParcela.vr_saldo_devedor;
                par.ind_forma_pagamento = value.OperacaoParcela.ind_forma_pagamento;
                par.codigo_barras = value.OperacaoParcela.codigo_barras;
                par.dt_ultima_amortizacao = value.OperacaoParcela.dt_ultima_amortizacao;
                par.ind_baixa = value.OperacaoParcela.ind_baixa;
                par.dt_baixa = value.OperacaoParcela.dt_baixa;
                par.cheque_banco = value.OperacaoParcela.cheque_banco;
                par.cheque_agencia = value.OperacaoParcela.cheque_agencia;
                par.cheque_numero = value.OperacaoParcela.cheque_numero;

                Add(ref value, par);
            }
        }

        public override void OnCreateError(ref ORepo value, FormCollection collection)
        {
            Modify(ref value, collection); // popula credorId/clienteId e nome_credor/nome_cliente

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

            value.OperacaoParcela = getOperacaoParcelaRepositoryInstance();
            value.OperacaoParcela.parcelaId = 1;
            value.OperacaoParcela.dt_vencimento = collection["dt_vencimento"] != null && collection["dt_vencimento"] != "" ? DateTime.Parse(collection["dt_vencimento"]) : Funcoes.Brasilia().Date;
            value.OperacaoParcela.OperacaoParcelaEvento = getOperacaoParcelaEventoRepositoryInstance();
            value.OperacaoParcela.OperacaoParcelaEvento.parcelaId = 1;
            value.OperacaoParcela.OperacaoParcelaEvento.dt_ocorrencia = collection["dt_ocorrencia"] != null && collection["dt_ocorrencia"] != "" ? DateTime.Parse(collection["dt_ocorrencia"]) : Funcoes.Brasilia().Date;
            value.OperacaoParcela.OperacaoParcelaEvento.dt_movto = collection["dt_movto"] != null && collection["dt_movto"] != "" ? DateTime.Parse(collection["dt_movto"]) : Funcoes.Brasilia().Date;

            if (collection["bancoId"] != null && collection["bancoId"] != "")
            {
                value.OperacaoParcela.bancoId = int.Parse(collection["bancoId"]);
                value.OperacaoParcela.nome_banco = collection["nome_banco"];
            }

            if (collection["fileBoleto"] != null)
                value.fileBoleto = collection["fileBoleto"];

            if (collection["fileComprovante"] != null)
                value.fileComprovante = collection["fileComprovante"];

            if (collection["OperacaoParcela_vr_principal"] != null && collection["OperacaoParcela_vr_principal"] != "")
                value.OperacaoParcela.vr_principal = Decimal.Parse(collection["OperacaoParcela_vr_principal"]);

            if (collection["OperacaoParcela_OperacaoParcelaEvento_valor"] != null && collection["OperacaoParcela_OperacaoParcelaEvento_valor"] != "")
                value.OperacaoParcela.OperacaoParcelaEvento.valor = decimal.Parse(collection["OperacaoParcela_OperacaoParcelaEvento_valor"]);
            if (collection["vr_jurosMora"] != null && collection["vr_jurosMora"] != "")
                value.vr_jurosMora = decimal.Parse(collection["vr_jurosMora"]);
            if (collection["vr_multa"] != null && collection["vr_multa"] != "")
                value.vr_multa = decimal.Parse(collection["vr_multa"]);

            value.num_parcelas = int.Parse(collection["num_parcelas"]);

            if (collection["vr_desconto"] != null && collection["vr_desconto"] != "" && collection["vr_desconto"] != "0,00")
                value.OperacaoParcela.vr_desconto = decimal.Parse(collection["vr_desconto"]);
            if (collection["vr_juros"] != null && collection["vr_juros"] != "" && collection["vr_juros"] != "0,00")
                value.OperacaoParcela.vr_encargos = decimal.Parse(collection["vr_juros"]);
            if (collection["vr_mora"] != null && collection["vr_mora"] != "" && collection["vr_mora"] != "0,00")
                value.OperacaoParcela.vr_encargos += decimal.Parse(collection["vr_mora"]);
        }
        #endregion

        #region Gerar Parcelas
        public ActionResult ListParcelas(string num_titulo, string dt_vencimento, string ind_forma_pagamento, int bancoId, string nome_banco, string cheque_banco, string cheque_agencia, string cheque_numero, string vr_principal, string vr_amortizacao,
                                            string vr_juros, string vr_mora, string vr_desconto, int num_parcelas)
        {
            return View(GeraParcelas(num_titulo, dt_vencimento, ind_forma_pagamento, bancoId, nome_banco, cheque_banco, cheque_agencia,
                                        cheque_numero, vr_principal, vr_amortizacao, vr_juros, vr_mora, vr_desconto, num_parcelas));
        }

        #region Deve ser especializado
        protected IEnumerable<App_Dominio.Component.Repository> GeraParcelas(string num_titulo, string dt_vencimento, string ind_forma_pagamento,
                                                                           int bancoId, string nome_banco, string cheque_banco, string cheque_agencia, string cheque_numero,
                                                                           string vr_principal, string vr_amortizacao,
                                                                           string vr_juros, string vr_mora, string vr_desconto, int num_parcelas)
        {
            ViewBag.ValidateRequest = true;
            //DateTime _dt_vencimento = Convert.ToDateTime(dt_vencimento.Substring(6, 4) + "-" + dt_vencimento.Substring(3, 2) + "-" + dt_vencimento.Substring(0, 2));
            //int? _cheque_banco = null;
            //if (cheque_banco != null && cheque_banco != "")
            //    _cheque_banco = int.Parse(cheque_banco);
            //decimal _vr_principal = Convert.ToDecimal(vr_principal);
            //decimal _vr_amortizacao = Convert.ToDecimal(vr_amortizacao);
            //decimal _vr_juros = Convert.ToDecimal(vr_juros);
            //decimal _vr_mora = Convert.ToDecimal(vr_mora);
            //decimal _vr_desconto = Convert.ToDecimal(vr_desconto);

            return getParcelasl(num_titulo, dt_vencimento, ind_forma_pagamento, bancoId, nome_banco, cheque_banco, cheque_agencia, cheque_numero,
                                vr_principal, vr_amortizacao, vr_juros, vr_mora, vr_desconto, num_parcelas);
        }
        #endregion

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
            return JSonTypeahead(null, new ListViewContaPagar());
        }

    }
}