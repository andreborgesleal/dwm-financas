using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Models;
using DWM.Models.Entidades;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace DWM.Models.BI
{
    public class ListViewBalanceteBI : DWMContext<ApplicationContext>, IProcess<BalanceteViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewBalanceteBI() { }

        public ListViewBalanceteBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual BalanceteViewModel Run(Repository value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BalanceteViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 100, params object[] param)
        {
            int pageIndex = index ?? 0;

            #region Parâmetros
            int centroCustoId = (int)param[0];
            int exercicio = (int)param[1];
            int grauPC = (int)param[2];
            DateTime dt1 = Convert.ToDateTime(param[3].ToString());
            DateTime dt2 = Convert.ToDateTime(param[4].ToString());

            SqlParameter empresaIdParam = new SqlParameter("@pEmpresaId", SqlDbType.Int);
            SqlParameter centroCustoIdParam = new SqlParameter("@pCentroCustoID", SqlDbType.Int);
            SqlParameter exercicioIdParam = new SqlParameter("@pExercicio", SqlDbType.Int);
            SqlParameter grauPCParam = new SqlParameter("@pGrauPC", SqlDbType.Int);
            SqlParameter pageSizeParam = new SqlParameter("@pageSize", SqlDbType.Int);
            SqlParameter totalCountParam = new SqlParameter("@totalCount", SqlDbType.Int);
            SqlParameter Cod_erroParam = new SqlParameter("@pCod_erro", SqlDbType.Int);
            SqlParameter Desc_erroParam = new SqlParameter("@pDesc_erro", SqlDbType.NVarChar, 400);

            empresaIdParam.Value = sessaoCorrente.empresaId;
            centroCustoIdParam.Value = centroCustoId;
            exercicioIdParam.Value = exercicio;
            grauPCParam.Value = grauPC;
            pageSizeParam.Value = pageSize;

            totalCountParam.Direction = ParameterDirection.Output;
            totalCountParam.Value = 0;

            Cod_erroParam.Direction = ParameterDirection.Output;
            Cod_erroParam.Value = 0;

            Desc_erroParam.Direction = ParameterDirection.Output;
            Desc_erroParam.Value = "";
            #endregion


            IEnumerable<BalanceteViewModel> bal = db.Database.SqlQuery<BalanceteViewModel>("spr_balancete @pEmpresaId, @pCentroCustoID, @pExercicio, @pGrauPC, @pData1, @pData2, @pageSize, @pageNumber, @totalCount out, @pCod_erro out, @pDesc_erro out",
                                                                                            empresaIdParam,
                                                                                            centroCustoIdParam,
                                                                                            exercicioIdParam,
                                                                                            grauPCParam,
                                                                                            new SqlParameter("@pData1", dt1),
                                                                                            new SqlParameter("@pData2", dt2),
                                                                                            pageSizeParam,
                                                                                            new SqlParameter("@pageNumber", ++index),
                                                                                            totalCountParam,
                                                                                            Cod_erroParam,
                                                                                            Desc_erroParam);

            return new PagedList<BalanceteViewModel>(bal.ToList(), pageIndex, pageSize, (int)totalCountParam.Value, "ListParam", null, "div-list-static");
        }

    }
}