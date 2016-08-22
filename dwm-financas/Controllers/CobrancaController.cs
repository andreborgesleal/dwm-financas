using DWM.Models.Repositories;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.BI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App_Dominio.Security;
using App_Dominio.Models;
using App_Dominio.Pattern;

namespace DWM.Controllers
{
    public class CobrancaController : OperacaoController<EditarContaReceberViewModel,
                                                         EditarContaReceberParcelaEventoViewModel,
                                                         ContaReceberViewModel,
                                                         ContaReceberModel,
                                                         ContaReceberParcelaCrudModel,
                                                         ContaReceberParcelaEventoModel,
                                                         ContaReceberParcelaViewModel,
                                                         ContaReceberParcelaEventoViewModel,
                                                         ContaReceber,
                                                         ContaReceberParcela,
                                                         ContaReceberParcelaEvento,
                                                         ContaReceberEditarBI,
                                                         ContaReceberBaixarBI,
                                                         ContaReceberParcelaEventoBI,
                                                         ContaReceberEstornoBI,
                                                         ContaReceberModifyBI,
                                                         ContaReceberCancelarOperacaoBI,
                                                         ContaReceberExcluirOperacaoBI,
                                                         ContaReceberLiquidarOperacaoBI>
    {
        public override string getListName()
        {
            return "Listar Cobrança";
        }

        protected override string getTipoMovto()
        {
            return "C"; // crédito
        }

        protected override IEnumerable<App_Dominio.Component.Repository> getParc(ref ContaReceberViewModel value, FormCollection collection)
        {
            IEnumerable<App_Dominio.Component.Repository> r = GeraParcelas(value.OperacaoParcela.num_titulo, 
                                                                           collection["dt_vencimento"], 
                                                                           value.OperacaoParcela.ind_forma_pagamento, 
                                                                           int.Parse(collection["bancoId"]), 
                                                                           collection["nome_banco"],
                                                                           value.OperacaoParcela.cheque_banco.ToString(), 
                                                                           value.OperacaoParcela.cheque_agencia, 
                                                                           value.OperacaoParcela.cheque_numero, 
                                                                           collection["OperacaoParcela.vr_principal.1"],
                                                                           collection["OperacaoParcela.OperacaoParcelaEvento.valor.1"], 
                                                                           collection["vr_juros.1"], 
                                                                           collection["vr_mora.1"], 
                                                                           collection["vr_desconto.1"],
                                                                           int.Parse(collection["num_parcelas"]));
            return r;

        }

        protected override void Add(ref ContaReceberViewModel value, ContaReceberParcelaViewModel par)
        {
            ((List<ContaReceberParcelaViewModel>)value.OperacaoParcelas).Add(par);
        }

        protected override void Modify(ref ContaReceberViewModel value, FormCollection collection)
        {
            if (collection["clienteId"] != null)
                value.clienteId = int.Parse(collection["clienteId"]);

            value.nome_cliente = collection["nome_cliente"];

        }

        protected override IEnumerable<App_Dominio.Component.Repository> getParcelasl(string num_titulo, string dt_vencimento, string ind_forma_pagamento,
                                                                               int bancoId, string nome_banco, string cheque_banco, string cheque_agencia, string cheque_numero,
                                                                               string vr_principal, string vr_amortizacao,
                                                                               string vr_juros, string vr_mora, string vr_desconto, int num_parcelas)
        {
            DateTime _dt_vencimento = Convert.ToDateTime(dt_vencimento.Substring(6, 4) + "-" + dt_vencimento.Substring(3, 2) + "-" + dt_vencimento.Substring(0, 2));
            int? _cheque_banco = null;
            if (cheque_banco != null && cheque_banco != "")
                _cheque_banco = int.Parse(cheque_banco);
            decimal _vr_principal = Convert.ToDecimal(vr_principal);
            decimal _vr_amortizacao = Convert.ToDecimal(vr_amortizacao);
            decimal _vr_juros = Convert.ToDecimal(vr_juros);
            decimal _vr_mora = Convert.ToDecimal(vr_mora);
            decimal _vr_desconto = Convert.ToDecimal(vr_desconto);

            ListViewContaReceberParcela2 list = new ListViewContaReceberParcela2();

            return list.ListRepository(0, 50, num_titulo, _dt_vencimento, ind_forma_pagamento, nome_banco, _cheque_banco, cheque_agencia, cheque_numero, _vr_principal, _vr_amortizacao, _vr_juros, _vr_mora, _vr_desconto, num_parcelas, bancoId);
        }


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
                ListViewContaReceber list = new ListViewContaReceber();
                return this._List(index, pageSize, "Browse", list, _titulos_vencidos_atraso, _dt_vencidos_atraso1, _dt_vencidos_atraso2, _titulos_a_vencer, 
                                    _dt_vencimento1, _dt_vencimento2, _titulos_amortizados, _titulos_nao_pagos, _baixa_liquidacao,
                                    _baixa_cancelamento, _dt_baixa1, _dt_baixa2, _clienteId, _dt_emissao1, _dt_emissao2, _centroCustoId, _grupoId, _bancoId);
            }
            else
                return View();
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
        #endregion
    }
}