using App_Dominio.Component;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DWM.Models.Report
{
    public class RazaoReport : ReportViewModel<RazaoViewModel>
    {
        #region Métodos da classe ReportRepository
        public override IEnumerable<RazaoViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            index = index ?? 0;
            DateTime dt1 = Convert.ToDateTime(param[0].ToString());
            DateTime dt2 = Convert.ToDateTime(param[1].ToString());
            int? centroCustoId = (int?)param[2];
            int? planoContaId = (int?)param[3];
            totalizaColuna1 = param[4].ToString();
            totalizaColuna2 = param[5].ToString();

            //int _exercicio = int.Parse(sessaoCorrente.value1);
            int _exercicio = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EXERCICIO_CONTABIL, sessaoCorrente.empresaId).valor);

            SqlParameter centroCustoIdParam = new SqlParameter("@centroCustoId", SqlDbType.Int);
            SqlParameter pageSizeParam = new SqlParameter("@pageSize", SqlDbType.Int);

            centroCustoIdParam.Value = DBNull.Value;
            centroCustoIdParam.Value = centroCustoId == 0 || centroCustoId == null ? centroCustoIdParam.Value : centroCustoId;

            IEnumerable<PlanoContaViewModel> listPC = (from pc in db.PlanoContas where pc.empresaId == sessaoCorrente.empresaId
                                                       select new PlanoContaViewModel()
                                                       {
                                                           planoContaId = pc.planoContaId,
                                                           planoContaId_pai = pc.planoContaId_pai,
                                                           codigoPleno = pc.codigoPleno,
                                                           tipoConta = pc.tipoConta
                                                       }).ToList();

            string codigoPleno = getContasAnaliticas(listPC, planoContaId.Value);

            pageSizeParam.Value = pageSize;

            #region stored procedure
            IEnumerable<RazaoViewModel> raz = db.Database.SqlQuery<RazaoViewModel>("spr_razao @data1, @data2, @centroCustoId, @codigoPlenoList, @exercicio, @empresaId, @pageSize, @pageNumber",
                                                                                      new SqlParameter("@data1", dt1),
                                                                                      new SqlParameter("@data2", dt2),
                                                                                      centroCustoIdParam,
                                                                                      new SqlParameter("@codigoPlenoList", codigoPleno),
                                                                                      new SqlParameter("@exercicio", _exercicio),
                                                                                      new SqlParameter("@empresaId", sessaoCorrente.empresaId),
                                                                                      pageSizeParam,
                                                                                      new SqlParameter("@pageNumber", ++index));
            #endregion

            return raz.ToList();
        }

        public override Repository getRepository(Object id)
        {
            throw new NotImplementedException();
        }

        public override string action()
        {
            return "ListParam";
        }

        public override string DivId()
        {
            return "div-list-static2";
        }

        public override IEnumerable<RazaoViewModel> BindReport(params object[] param)
        {
            DateTime dt1 = Convert.ToDateTime(param[0].ToString());
            DateTime dt2 = Convert.ToDateTime(param[1].ToString());
            int? centroCustoId = (int?)param[2];
            int? planoContaId = (int?)param[3];
            totalizaColuna1 = param[4].ToString();
            totalizaColuna2 = param[5].ToString();

            //int _exercicio = int.Parse(sessaoCorrente.value1);
            int _exercicio = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EXERCICIO_CONTABIL, sessaoCorrente.empresaId).valor);

            SqlParameter centroCustoIdParam = new SqlParameter("@centroCustoId", SqlDbType.Int);
            SqlParameter pageSizeParam = new SqlParameter("@pageSize", SqlDbType.Int);

            centroCustoIdParam.Value = DBNull.Value;
            centroCustoIdParam.Value = centroCustoId == 0 || centroCustoId == null ? centroCustoIdParam.Value : centroCustoId;

            string codigoPleno = db.PlanoContas.Find(planoContaId).codigoPleno;

            pageSizeParam.Value = 0;

            #region stored procedure
            IEnumerable<RazaoViewModel> raz = db.Database.SqlQuery<RazaoViewModel>("spr_razao @data1, @data2, @centroCustoId, @codigoPlenoList, @exercicio, @empresaId, @pageSize, @pageNumber",
                                                                                      new SqlParameter("@data1", dt1),
                                                                                      new SqlParameter("@data2", dt2),
                                                                                      centroCustoIdParam,
                                                                                      new SqlParameter("@codigoPlenoList", codigoPleno),
                                                                                      new SqlParameter("@exercicio", _exercicio),
                                                                                      new SqlParameter("@empresaId", sessaoCorrente.empresaId),
                                                                                      pageSizeParam,
                                                                                      new SqlParameter("@pageNumber", 1));
            #endregion

            //int? centroCustoId = null;

            //string periodo = param[0].ToString();
            //string data1 = param[1].ToString();
            //string data2 = param[2].ToString();
            //string codigoPleno1 = param[3].ToString();
            //string codigoPleno2 = param[4].ToString();

            //totalizaColuna1 = param[6].ToString();
            //totalizaColuna2 = param[7].ToString();

            //centroCustoId = param[5].ToString() != "" && param[5].ToString() != "0" ? int.Parse(param[5].ToString()) : centroCustoId;

            //#region Preenche os parâmetros com Filtros (se não for informado nenhum parâmetro)
            //if (periodo == "")
            //{
            //    periodo = getFiltros().Where(info => info.atributo == "Periodo").Select(info => info.valor).First();
            //    data1 = getFiltros().Where(info => info.atributo == "data1").Select(info => info.valor).First();
            //    data2 = getFiltros().Where(info => info.atributo == "data2").Select(info => info.valor).First();
            //    centroCustoId = getFiltros().Where(info => info.atributo == "centroCustoId").Select(info => info.valor).First() != "" ? int.Parse(getFiltros().Where(info => info.atributo == "centroCustoId").Select(info => info.valor).First()) : centroCustoId;
            //    codigoPleno1 = getFiltros().Where(info => info.atributo == "codigoPleno1").Select(info => info.valor).First();
            //    codigoPleno2 = getFiltros().Where(info => info.atributo == "codigoPleno2").Select(info => info.valor).First();
            //    totalizaColuna1 = getFiltros().Where(info => info.atributo == "totalizaConta").Select(info => info.valor).First();
            //    totalizaColuna2 = getFiltros().Where(info => info.atributo == "totalizaDia").Select(info => info.valor).First();
            //}
            //#endregion

            //DateTime[] datas = Funcoes.CalculaPeriodo(periodo, data1, data2);

            //DateTime dt1 = datas[0];
            //DateTime dt2 = datas[1];

            //SqlParameter centroCustoIdParam = new SqlParameter("@centroCustoId", SqlDbType.Int);
            //SqlParameter pageSizeParam = new SqlParameter("@pageSize", SqlDbType.Int);

            //centroCustoIdParam.Value = DBNull.Value;
            //centroCustoIdParam.Value = centroCustoId == 0 || centroCustoId == null ? centroCustoIdParam.Value : centroCustoId;

            //pageSizeParam.Value = 0; 

            //#region stored procedure
            //IEnumerable<RazaoViewModel> raz = db.Database.SqlQuery<RazaoViewModel>("spr_razao @data1, @data2, @centroCustoId, @codigoPleno1, @codigoPleno2, @exercicio, @empresaId, @pageSize, @pageNumber",
            //                                                                          new SqlParameter("@data1", dt1),
            //                                                                          new SqlParameter("@data2", dt2),
            //                                                                          centroCustoIdParam,
            //                                                                          new SqlParameter("@codigoPleno1", codigoPleno1),
            //                                                                          new SqlParameter("@codigoPleno2", codigoPleno2),
            //                                                                          new SqlParameter("@exercicio", sessaoCorrente.exercicio.Value),
            //                                                                          new SqlParameter("@empresaId", sessaoCorrente.empresaId),
            //                                                                          pageSizeParam,
            //                                                                          new SqlParameter("@pageNumber", 1));
            //#endregion

            return raz.ToList();
        }
        #endregion

        #region Métodos customizados
        public string getContasAnaliticas(IEnumerable<PlanoContaViewModel> listPC, int planoContaId)
        {
            string codigoPleno = "";
            if (listPC.Where(info => info.planoContaId == planoContaId).FirstOrDefault().tipoConta == "A")
                codigoPleno = listPC.Where(info => info.planoContaId == planoContaId).FirstOrDefault().codigoPleno + ", ";
                //codigoPleno = db.PlanoContas.Find(planoContaId).codigoPleno + ", ";
            else
            {
                IEnumerable<PlanoContaViewModel> placon = (from pc in listPC //db.PlanoContas
                                                          where pc.planoContaId_pai == planoContaId
                                                          select new PlanoContaViewModel()
                                                          {
                                                              codigoPleno = pc.codigoPleno,
                                                              planoContaId = pc.planoContaId,
                                                              planoContaId_pai = pc.planoContaId_pai
                                                          }).ToList();
                foreach (PlanoContaViewModel pc in placon)
                    codigoPleno += getContasAnaliticas(listPC, pc.planoContaId);
            }
            return codigoPleno;
        }
        #endregion
    }

    public class LookupRazaoModel : RazaoReport
    {
        public override string action()
        {
            return "../Razao/ListRazaoModal";
        }
    }

    public class LookupRazaoFiltroModel : RazaoReport
    {
        public override string action()
        {
            return "../Razao/_ListRazaoModal";
        }

        public override string DivId()
        {
            return "div-raz";
        }
    }
}