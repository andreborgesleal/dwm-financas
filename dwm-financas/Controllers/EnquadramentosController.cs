using App_Dominio.Contratos;
using App_Dominio.Controllers;
using App_Dominio.Security;
using DWM.Models.Entidades;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Web.Mvc;

namespace DWM.Controllers
{
    public class EnquadramentosController : DwmRootItemController<EnquadramentoViewModel, EnquadramentoModel, EnquadramentoItemViewModel, EnquadramentoItemModel, ApplicationContext>
    {
        #region Master
        #region Herança
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override string getListName()
        {
            return "Listar Enquadramentos";
        }
        #endregion

        #region List
        [AuthorizeFilter]
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                ListViewEnquadramento l = new ListViewEnquadramento();
                return this._List(index, pageSize, "Browse", (IListRepository)l, descricao);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult _ListEnquadramentoModal(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                LookupEnquadramentoFiltroModel l = new LookupEnquadramentoFiltroModel();
                return this.ListModal(index, pageSize, (IListRepository)l, "Descrição", descricao);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult _ListEnquadramentoOperacaoModal(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                LookupEnquadramentoOperacaoFiltroModel l = new LookupEnquadramentoOperacaoFiltroModel();
                return this.ListModal(index, pageSize, (IListRepository)l, "Descrição", descricao);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult _ListEnquadramentoOperacaoAmortizacaoModal(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                LookupEnquadramentoOperacaoAmortizacaoFiltroModel l = new LookupEnquadramentoOperacaoAmortizacaoFiltroModel();
                return this.ListModal(index, pageSize, (IListRepository)l, "Descrição", descricao);
            }
            else
                return View();
        }

        #endregion

        #region edit
        [AuthorizeFilter]
        public ActionResult Edit(int enquadramentoId)
        {
            return _Edit(new EnquadramentoViewModel() { enquadramentoId = enquadramentoId });
        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult Delete(int enquadramentoId)
        {
            return Edit(enquadramentoId);
        }
        #endregion

        public JsonResult getNames()
        {
            return JSonTypeahead(null, (IListRepository)new ListViewEnquadramento());
        }

        #endregion

        #region Items
        #region implementação métodos abstratos
        public override EnquadramentoViewModel setRepositoryAfterError(EnquadramentoViewModel value, FormCollection collection)
        {
            #region recupera conteúdo do textbox dos atributos de Lookup
            if (collection["centroCustoId"] != "")
            {
                value.EnquadramentoItem.centroCustoId = int.Parse(collection["centroCustoId"]);
                value.EnquadramentoItem.descricao_centroCusto = collection["descricao_centroCusto"];
            }

            if (collection["planoContaId"] != "")
            {
                value.EnquadramentoItem.planoContaId = int.Parse(collection["planoContaId"]);
                value.EnquadramentoItem.codigoPleno = collection["codigoPleno"];
                value.EnquadramentoItem.descricao_planoConta = collection["descricao_planoConta"];
            }

            if (collection["´historicoId"] != "")
            {
                value.EnquadramentoItem.historicoId = int.Parse(collection["historicoId"]);
                value.EnquadramentoItem.descricao_historico = collection["descricao_historico"];
            }

            return value;
            #endregion
        }
        #endregion

        #region Create Item
        public ActionResult CreateItem(int? sequencial,
                                int? centroCustoId,
                                string descricao_centroCusto,
                                int planoContaId,
                                string codigoPleno,
                                string descricao_planoConta,
                                int historicoId,
                                string descricao_historico,
                                string complementoHist,
                                string tipoLancamento,
                                string valor,
                                string operacao)
        {
            EnquadramentoItemViewModel item = new EnquadramentoItemViewModel()
            {
                sequencial = sequencial.Value,
                centroCustoId = centroCustoId,
                descricao_centroCusto = descricao_centroCusto,
                planoContaId = planoContaId,
                codigoPleno = codigoPleno,
                descricao_planoConta = descricao_planoConta,
                historicoId = historicoId,
                descricao_historico = descricao_historico,
                complementoHist = complementoHist,
                tipoLancamento = tipoLancamento,
                valor = !String.IsNullOrEmpty(valor) ? decimal.Parse(valor) : new Nullable<decimal>(),
                operacao = operacao
            };
            TempData["operacao"] = "I";
            return UpdateItem(item, new EnquadramentoViewModel(), operacao);
        }
        #endregion

        #region Edit Item
        public ActionResult EditItem(int sequencial)
        {
            if (TempData["master"] == null)
            {
                TempData["operacao"] = "A";
                return GetItem(info => info.sequencial == sequencial, new EnquadramentoViewModel());
            }
            else
                return View((EnquadramentoViewModel)TempData["master"]);
        }
        #endregion

        #region Delete Item
        public ActionResult DeleteItem(int sequencial)
        {
            if (TempData["master"] == null)
            {
                TempData["operacao"] = "E";
                return GetItem(info => info.sequencial == sequencial, new EnquadramentoViewModel());
            }
            else
                return View((EnquadramentoViewModel)TempData["master"]);
        }
        #endregion

        public ActionResult NewItem()
        {
            return _NewItem(new EnquadramentoViewModel());
        }

        public override string getName()
        {
            return "Enquadramento";
        }
        #endregion
    }
}