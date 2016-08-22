using App_Dominio.Controllers;
using DWM.Models.Repositories;
using System;
using System.Web.Mvc;
//using Microsoft.Reporting.WebForms;
using DWM.Models.Enumeracoes;
using DWM.Models.Report;
using App_Dominio.Security;

namespace DWM.Controllers
{
    public class RazaoController : ReportController<RazaoViewModel>
    {
        #region Herança
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }

        public override bool mustListOnLoad()
        {
            return Request["planoContaid"] != null;
        }

        public override string getListName()
        {
            return "Razão";
        }

        public override bool ClearBreadCrumbOnBrowse()
        {
            return Request["id"] == null;
        }
        #endregion    

        #region List
        public override ActionResult List(int? index, int? PageSize, string descricao = null)
        {
            return ListParam(index, PageSize);
        }

        public ActionResult _ListRazaoModal(int? index, int? pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                LookupRazaoFiltroModel l = new LookupRazaoFiltroModel();
                return this.ListModal(index, pageSize, l, "Descrição", descricao);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult ListParam(int? index, int? pageSize = 50, string data1 = "", string data2 = "", 
                                        int? centroCustoId = null, string descricao_centroCusto = "",
                                        int? planoContaId = null, string descricao_conta = "", string codigoPleno = "",
                                        string totalizaConta = "", string totalizaDia = "")
        {
            ViewBag.ValidateRequest = true;
            if (ViewBag.ValidateRequest)
            {
                if (Request ["planoContaId"] != "")
                {
                    data1 = Request["data1"];
                    data2 = Request["data2"];
                    if (Request["centroCustoId"] != "")
                        centroCustoId = int.Parse(Request["centroCustoid"]);

                    planoContaId = int.Parse(Request["planoContaId"]);
                    totalizaConta = Request["totalizaConta"];
                    totalizaDia = Request["totalizaDia"];
                }

                if (!planoContaId.HasValue)
                {
                    Attention("É preciso informar a conta para a emissão do Razão");
                    return View();
                }   

                if (data1 == "")
                {
                    data1 = DateTime.Today.ToString("yyyy-MM-") + "01";
                    data2 = Convert.ToDateTime(DateTime.Today.AddMonths(1).ToString("yyyy-MM-") + "01").AddDays(-1).ToString("yyyy-MM-dd");
                }

                RazaoReport raz = new RazaoReport();
                return this._List(index, pageSize, "Browse", raz, data1, data2, centroCustoId, planoContaId, totalizaConta, totalizaDia);
            }
            else
                return View();
        }
        #endregion

        public FileResult PDF(string export, string data1 = "", string data2 = "", int? centroCustoId = null, int? planoContaId = null, string descricao_centroCusto = "", string descricao_conta = "", string totalizaConta = "S", string totalizaDia = "N")
        {
            throw new NotImplementedException();
            //ReportParameter[] p = new ReportParameter[5];
            //// o parâmetro p[0] fica reservado para ser preenchido automaticamente com o nome da empresa
            //p[1] = new ReportParameter("periodo", "Período: " + data1 + " à " + data2, false);
            //p[2] = new ReportParameter("centroCusto", "C.Custo: " + (descricao_centroCusto == "" ? "Todos" : descricao_centroCusto), false);
            //p[3] = new ReportParameter("totalizaConta", totalizaConta, false);
            //p[4] = new ReportParameter("totalizaDia", totalizaDia, false);

            //if (!planoContaId.HasValue)
            //{
            //    Attention("É preciso informar a conta para a emissão do Razão");
            //    throw new Exception("É preciso informar a conta para a emissão do Razão");
            //}

            //if (data1 == "")
            //{
            //    data1 = DateTime.Today.ToString("yyyy-MM-") + "01";
            //    data2 = DateTime.Today.ToString("yyyy-MM-dd");
            //}

            //return _PDF(export, "Razao", new RazaoReport(), p, null, null, data1, data2, centroCustoId, planoContaId, "N", "N");
        }

        #region Formulário Modal
        public ActionResult ListRazaoModal(int? index, int? pageSize = 50, string data1 = "", string data2 = "", int? centroCustoId = null, int? planoContaId = null, string totalizaConta = "", string totalizaDia = "")
        {
            LookupRazaoModel l = new LookupRazaoModel();
            return this.ListModal(index, pageSize, l, "Razão", data1, data2, centroCustoId, planoContaId, totalizaConta, totalizaDia);
        }

        public ActionResult _ListRazaoModal(int? index, int? pageSize = 50, string data1 = "", string data2 = "", int? centroCustoId = null, int? planoContaId = null, string totalizaConta = "", string totalizaDia = "")
        {
            LookupRazaoFiltroModel l = new LookupRazaoFiltroModel();
            return this.ListModal(index, pageSize, l, "Razão", data1, data2, centroCustoId, planoContaId, totalizaConta, totalizaDia);
        }

        #endregion
    }


}