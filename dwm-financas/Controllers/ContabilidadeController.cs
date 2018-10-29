using App_Dominio.Controllers;
using App_Dominio.Enumeracoes;
using App_Dominio.Security;
using DWM.Models.Entidades;
using DWM.Models.Enumeracoes;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Web.Mvc;
using App_Dominio.Contratos;
using App_Dominio.Pattern;
using DWM.Models.BI;
using System.Data.Entity;

namespace DWM.Controllers
{
    public class ContabilidadeController : DwmRootItemController<ContabilidadeViewModel, ContabilidadeModel, ContabilidadeItemViewModel, ContabilidadeItemModel, ApplicationContext>
    {
        #region Master
        #region Herança
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }
        public override string getListName()
        {
            return "Listar Lançamentos Contábeis";
        }
        public override bool mustListOnLoad()
        {
            Factory<ExercicioViewModel, ApplicationContext> facade = new Factory<ExercicioViewModel, ApplicationContext>();
            ExercicioViewModel e = facade.Execute(new ExercicioBI(), new ExercicioViewModel());

            if (e.mensagem.Code == 0 && e.dt_lancamento_inicio.HasValue)
            {
                ViewData["dt_lancamento_inicio"] = e.dt_lancamento_inicio.Value;
                ViewData["dt_lancamento_fim"] = e.dt_lancamento_fim.Value;
            }
            else
            {
                ViewData["dt_lancamento_inicio"] = Convert.ToDateTime(DateTime.Today.ToString("yyyy-MM-") + "01");
                ViewData["dt_lancamento_fim"] = Convert.ToDateTime(DateTime.Today.AddMonths(1).ToString("yyyy-MM-") + "01").AddDays(-1);
            }

            return base.mustListOnLoad();
        }
        #endregion

        #region List
        [AuthorizeFilter]
        public override ActionResult List(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                if (descricao != null)
                {
                    return ListParam(index, PageSize, descricao);
                }
                else
                    return ListParam(index, PageSize);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult ListParam(int? index, int? pageSize = 50, string data1 = "", string data2 = "", 
                                        int? planoContaId = null, int? centroCustoId = null, int? historicoId = null, 
                                        int? contabilidadeId = null)
        {
            if (ViewBag.ValidateRequest)
            {
                if (data1 == "")
                {
                    data1 = DateTime.Today.ToString("yyyy-MM-") + "01";
                    data2 = Convert.ToDateTime(DateTime.Today.AddMonths(1).ToString("yyyy-MM-") + "01").AddDays(-1).ToString("yyyy-MM-dd");
                }

                ListViewContabilidade l = new ListViewContabilidade();
                return this._List(index, pageSize, "Browse", l, data1, data2, planoContaId, centroCustoId, historicoId, contabilidadeId);
            }
            else
                return View();
        }
        #endregion

        #region edit
        [AuthorizeFilter]
        public ActionResult Edit(int contabilidadeId)
        {
            return _Edit(new ContabilidadeViewModel() { contabilidadeId = contabilidadeId });
        }
        #endregion

        #region saveAs
        [AuthorizeFilter]
        public ActionResult SaveAs(int contabilidadeId)
        {
            return _Edit(new ContabilidadeViewModel() { contabilidadeId = contabilidadeId },"Replicar");
        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult Delete(int contabilidadeId)
        {
            return Edit(contabilidadeId);
        }
        #endregion
        #endregion

        #region Items
        #region implementação métodos abstratos
        public override ContabilidadeViewModel setRepositoryAfterError(ContabilidadeViewModel value, FormCollection collection)
        {
            #region recupera conteúdo do textbox dos atributos de Lookup
            if (collection["centroCustoId"] != "")
            {
                value.ContabilidadeItem.centroCustoId = int.Parse(collection["centroCustoId"]);
                value.ContabilidadeItem.descricao_centroCusto = collection["descricao_centroCusto"];
            }

            if (collection["planoContaId"] != "")
            {
                value.ContabilidadeItem.planoContaId = int.Parse(collection["planoContaId"]);
                value.ContabilidadeItem.codigoPleno = collection["codigoPleno"];
                value.ContabilidadeItem.descricao_planoConta = collection["descricao_planoConta"];
            }

            if (collection["historicoId"] != "")
            {
                value.ContabilidadeItem.historicoId = int.Parse(collection["historicoId"]);
                value.ContabilidadeItem.descricao_historico = collection["descricao_historico"];
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
            ContabilidadeItemViewModel item = new ContabilidadeItemViewModel()
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
                valor = !String.IsNullOrEmpty(valor) ? decimal.Parse(valor) : new decimal(0),
                operacao = operacao
            };
            TempData["operacao"] = "I";
            return UpdateItem(item, new ContabilidadeViewModel(), operacao);
        }
        #endregion

        #region Edit Item
        public ActionResult EditItem(int sequencial)
        {
            if (TempData["master"] == null)
            {
                TempData["operacao"] = "A";
                return GetItem(info => info.sequencial == sequencial, new ContabilidadeViewModel());
            }
            else
                return View((ContabilidadeViewModel)TempData["master"]);
        }
        #endregion

        #region Delete Item
        public ActionResult DeleteItem(int sequencial)
        {
            if (TempData["master"] == null)
            {
                TempData["operacao"] = "E";
                return GetItem(info => info.sequencial == sequencial, new ContabilidadeViewModel());
            }
            else
                return View((ContabilidadeViewModel)TempData["master"]);
        }
        #endregion

        public ActionResult NewItem()
        {
            return _NewItem(new ContabilidadeViewModel());
        }

        [AuthorizeFilter]
        public ActionResult Enquadramento(int enquadramentoId)
        {
            if (ViewBag.ValidateRequest)
            {
                try
                {
                    Factory<ContabilidadeViewModel, ApplicationContext> factory = new Factory<ContabilidadeViewModel, ApplicationContext>();
                    EnquadramentoViewModel enq = new EnquadramentoViewModel() { enquadramentoId = enquadramentoId };
                    ContabilidadeViewModel cont = factory.Execute((IProcess<ContabilidadeViewModel, ApplicationContext>)new ContabilidadeModel(), enq); 

                    TempData["operacao"] = "I";
                    TempData.Remove(getName());
                    TempData.Add(getName(), cont);
                    cont.CreateItem(); // cria uma instância do ItemRepository para ser exibida na tela para uma nova inclusão
                    return View("CreateItem", cont);
                }
                catch (Exception ex)
                {
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Information(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }

                return View();
            }
            else
                return null;
        }

        public override string getName()
        {
            return "Contabilidade";
        }
        #endregion
    }
}