using App_Dominio.Controllers;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using Microsoft.Reporting.WebForms;
using DWM.Models.Enumeracoes;
using DWM.Models.Report;
using App_Dominio.Repositories;
using App_Dominio.Security;
using App_Dominio.Pattern;
using DWM.Models.Entidades;
using DWM.Models.BI;

namespace DWM.Controllers
{
    public class DiarioController : ReportController<DiarioViewModel>
    {
        #region Herança
        public override int _sistema_id() { return (int)Sistema.DWMFINANCAS; }

        public override string getListName()
        {
            return "Diário geral da Contabilidade";
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
            //return Request["data1"] != null && Request["data1"] != "";
            return base.mustListOnLoad();
        }
        #endregion

        public override ActionResult List(int? index, int? PageSize, string descricao = null)
        {
            if (Request["data1"] != null && Request["data1"] != "")
                return ListParam(index, PageSize, Request["data1"], Request["data2"]);
            else
                return ListParam(index, PageSize);
        }

        [AuthorizeFilter]
        public ActionResult ListParam(int? index, int? pageSize = 50, string data1 = "", string data2 = "", int? centroCustoId = null, string totalizaDia = "", string totalizaId = "")
        {
            if (ViewBag.ValidateRequest)
            {
                if (data1 == "")
                {
                    data1 = DateTime.Today.ToString("yyyy-MM-") + "01";
                    data2 = Convert.ToDateTime(DateTime.Today.AddMonths(1).ToString("yyyy-MM-") + "01").AddDays(-1).ToString("yyyy-MM-dd");
                }

                DiarioReport d = new DiarioReport();
                return this._List(index, pageSize, "Browse", d, data1, data2, centroCustoId, totalizaDia, totalizaId);
            }
            else
                return View();

        }

        //public ActionResult SaveParam(int? index, int? pageSize = 50, string report = "_default",
        //                                string periodo = "", string data1 = "", string data2 = "",
        //                                string centroCustoId = "", string descricao_centroCusto = "",
        //                                string totalizaDia = "S", string totalizaId = "S")
        //{
        //    #region Salvar
        //    IEnumerable<FiltroRepository> Filtros = new List<FiltroRepository>()
        //    {   
        //        new FiltroRepository() { controller = "Diario", action = "Browse", atributo = "Periodo", valor = periodo },
        //        new FiltroRepository() { controller = "Diario", action = "Browse", atributo = "data1", valor = data1 },
        //        new FiltroRepository() { controller = "Diario", action = "Browse", atributo = "data2", valor = data2 },
        //        new FiltroRepository() { controller = "Diario", action = "Browse", atributo = "centroCustoId ", valor = centroCustoId },
        //        new FiltroRepository() { controller = "Diario", action = "Browse", atributo = "descricao_centroCusto", valor = descricao_centroCusto },
        //        new FiltroRepository() { controller = "Diario", action = "Browse", atributo = "totalizaDia", valor = totalizaDia },
        //        new FiltroRepository() { controller = "Diario", action = "Browse", atributo = "totalizaId", valor = totalizaId }                
        //    };

        //    SetEditParam(Filtros);

        //    return View("ListParam", this.PagedList(index, pageSize, report, "Browse", new DiarioReport(), periodo, data1, data2, centroCustoId, totalizaDia, totalizaId));

        //    #endregion
        //}

        public FileResult PDF(string export, string data1 = "", string data2 = "", int? centroCustoId = null, string descricao_centroCusto = "", string totalizaDia = "S", string totalizaId = "N")
        {
            throw new NotImplementedException();
            //ReportParameter[] p = new ReportParameter[5];
            //// o parâmetro p[0] fica reservado para ser preenchido automaticamente com o nome da empresa
            //p[1] = new ReportParameter("periodo", "Período: " + data1 + " à " + data2, false);
            //p[2] = new ReportParameter("centroCusto", "C.Custo: " + (descricao_centroCusto == "" ? "Todos" : descricao_centroCusto), false);
            //p[3] = new ReportParameter("totalizaDia", totalizaDia, false);
            //p[4] = new ReportParameter("totalizaId", totalizaId, false);

            //return _PDF(export, "Diario", new DiarioReport(), p, null, null, data1, data2, centroCustoId, "N", "N");
        }
    }
}