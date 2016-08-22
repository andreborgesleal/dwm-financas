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
    public class ExtratoController : ReportController<ExtratoViewModel>
    {
        #region Herança
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }

        public override bool mustListOnLoad()
        {
            if (Request["bancoId"] != null)
                return true;
            else
                return false;
        }

        public override string getListName()
        {
            return "Extrato Bancário";
        }
        #endregion    

        #region List
        public override ActionResult List(int? index, int? PageSize, string descricao = null)
        {
            if (Request ["bancoId"] == null)
                return ListParam(index, PageSize);
            else
                return ListParam(index, PageSize, Request["data1"], Request["data2"], int.Parse(Request["bancoId"]));
        }

        [AuthorizeFilter]
        public ActionResult ListParam(int? index, int? pageSize = 50, string data1 = "", string data2 = "",
                                        int? bancoId = null)
        {
            if (ViewBag.ValidateRequest)
            {
                if (!bancoId.HasValue)
                {
                    Attention("É preciso informar o banco para a emissão do Extrato");
                    return View();
                }

                if (data1 == "")
                {
                    data1 = DateTime.Today.ToString("yyyy-MM-") + "01";
                    data2 = Convert.ToDateTime(DateTime.Today.AddMonths(1).ToString("yyyy-MM-") + "01").AddDays(-1).ToString("yyyy-MM-dd");
                }

                ExtratoReport ext = new ExtratoReport();
                return this._List(index, pageSize, "Browse", ext, data1, data2, bancoId);
            }
            else
                return View();
        }
        #endregion
    }
}