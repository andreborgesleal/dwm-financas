using App_Dominio.Component;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DWM.Models.Report
{
    public class ExtratoReport : ReportViewModel<ExtratoViewModel>
    {
        #region Métodos da classe ReportRepository
        public override IEnumerable<ExtratoViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            index = index ?? 0;
            DateTime dt1 = Convert.ToDateTime(param[0].ToString());
            DateTime dt2 = Convert.ToDateTime(param[1].ToString());
            int bancoId = (int)param[2];

            SqlParameter pData1 = new SqlParameter("@pData1", dt1);
            SqlParameter pData2 = new SqlParameter("@pData2", dt2);
            SqlParameter pBancoId = new SqlParameter("@pBancoId", SqlDbType.Int);
            SqlParameter pEmpresaId = new SqlParameter("@pEmpresaId", SqlDbType.Int);
            SqlParameter pPageSize = new SqlParameter("@pPageSize", SqlDbType.Int);
            SqlParameter pPageNumber = new SqlParameter("@pPageNumber", SqlDbType.Int);

            pBancoId.Value = bancoId;
            pEmpresaId.Value = sessaoCorrente.empresaId;
            pPageSize.Value = pageSize;
            pPageNumber.Value = ++index;

            #region stored procedure
            IEnumerable<ExtratoViewModel> ext = db.Database.SqlQuery<ExtratoViewModel>("spr_extrato @pData1, @pData2, @pBancoId, @pEmpresaId, @pPageSize, @pPageNumber ",
                                                                                        pData1,
                                                                                        pData2,
                                                                                        pBancoId,
                                                                                        pEmpresaId,
                                                                                        pPageSize,
                                                                                        pPageNumber);
            #endregion
            return ext.ToList();
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
            return "div-list-extrato";
        }

        public override IEnumerable<ExtratoViewModel> BindReport(params object[] param)
        {
            DateTime dt1 = Convert.ToDateTime(param[0].ToString());
            DateTime dt2 = Convert.ToDateTime(param[1].ToString());
            int? bancoId = (int?)param[2];

            SqlParameter pData1 = new SqlParameter("@pData1", dt1);
            SqlParameter pData2 = new SqlParameter("@pData2", dt2);
            SqlParameter pBancoId = new SqlParameter("@pBancoId", bancoId.Value);
            SqlParameter pEmpresaId = new SqlParameter("@pEmpresaId", sessaoCorrente.empresaId);
            SqlParameter pPageSize = new SqlParameter("@pPageSize", 0);
            SqlParameter pPageNumber = new SqlParameter("@pPageNumber", 1);

            #region stored procedure
            IEnumerable<ExtratoViewModel> ext = db.Database.SqlQuery<ExtratoViewModel>("spr_extrato @pData1, @pData2, @pBancoId, @pEmpresaId, @pPageSize, @pPageNumber ",
                                                                                        pData1,
                                                                                        pData2,
                                                                                        pBancoId,
                                                                                        pEmpresaId,
                                                                                        pPageSize,
                                                                                        pPageNumber);
            #endregion

            
            return ext.ToList();
        }
        #endregion

    }
}