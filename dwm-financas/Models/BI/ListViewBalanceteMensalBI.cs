using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
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
    public class ListViewBalanceteMensalBI : DWMContext<ApplicationContext>, IProcess<BalanceteMensalViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewBalanceteMensalBI() { }

        public ListViewBalanceteMensalBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual BalanceteMensalViewModel Run(Repository value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BalanceteMensalViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 100, params object[] param)
        {
            int pageIndex = index ?? 0;

            #region Parâmetros
            int centroCustoId = (int)param[1];
            int exercicio = (int)param[2];
            int grauPC = (int)param[3];
            string RecDesp = (string)param[5];

            SqlParameter empresaIdParam = new SqlParameter("@pEmpresaId", SqlDbType.Int);
            SqlParameter centroCustoIdParam = new SqlParameter("@pCentroCustoID", SqlDbType.Int);
            SqlParameter exercicioIdParam = new SqlParameter("@pExercicio", SqlDbType.Int);
            SqlParameter grauPCParam = new SqlParameter("@pGrauPC", SqlDbType.Int);
            SqlParameter RecDespParam = new SqlParameter("@pRecDesp", SqlDbType.NChar, 1);
            SqlParameter totalCountParam = new SqlParameter("@totalCount", SqlDbType.Int);
            SqlParameter Cod_erroParam = new SqlParameter("@pCod_erro", SqlDbType.Int);
            SqlParameter Desc_erroParam = new SqlParameter("@pDesc_erro", SqlDbType.NVarChar, 400);

            empresaIdParam.Value = sessaoCorrente.empresaId;
            centroCustoIdParam.Value = centroCustoId;
            exercicioIdParam.Value = exercicio;
            grauPCParam.Value = grauPC;
            RecDespParam.Value = RecDesp;

            totalCountParam.Direction = ParameterDirection.Output;
            totalCountParam.Value = 0;

            Cod_erroParam.Direction = ParameterDirection.Output;
            Cod_erroParam.Value = 0;

            Desc_erroParam.Direction = ParameterDirection.Output;
            Desc_erroParam.Value = "";
            #endregion

            db.Database.CommandTimeout = 3600;
            IEnumerable<BalanceteMensalViewModel> bal = db.Database.SqlQuery<BalanceteMensalViewModel>("spr_balancete_mensal @pEmpresaId, @pCentroCustoID, @pExercicio, @pGrauPC, @pRecDesp, @totalCount out, @pCod_erro out, @pDesc_erro out",
                                                                                            empresaIdParam,
                                                                                            centroCustoIdParam,
                                                                                            exercicioIdParam,
                                                                                            grauPCParam,
                                                                                            RecDespParam,
                                                                                            totalCountParam,
                                                                                            Cod_erroParam,
                                                                                            Desc_erroParam);

            return new PagedList<BalanceteMensalViewModel>(bal.ToList(), 0, 1000, (int)totalCountParam.Value, "ListParam", null, "div-list-static");
        }
    }
}