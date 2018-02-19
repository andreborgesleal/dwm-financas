using System;
using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using System.Web.Mvc;
using App_Dominio.Security;
using System.Data.SqlClient;
using System.Data;

namespace DWM.Models.BI
{
    public class GerarLancamentosRecorrentesBI : DWMContext<ApplicationContext>, IProcess<ExercicioViewModel, ApplicationContext>
    {
        #region Constructor
        public GerarLancamentosRecorrentesBI() { }

        public GerarLancamentosRecorrentesBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public ExercicioViewModel Run(Repository value)
        {
            ExercicioViewModel e = (ExercicioViewModel)value;
            try
            {
                #region Parâmetros

                SqlParameter empresaIdParam = new SqlParameter("@pEmpresaId", SqlDbType.Int);
                SqlParameter AnoMesParam = new SqlParameter("@pAnoMes", SqlDbType.NVarChar, 6);
                SqlParameter Cod_erroParam = new SqlParameter("@pCod_erro", SqlDbType.Int);
                SqlParameter Desc_erroParam = new SqlParameter("@pDesc_erro", SqlDbType.NVarChar, 400);

                empresaIdParam.Value = sessaoCorrente.empresaId;
                AnoMesParam.Value = e.dt_lancamento_inicio.Value.ToString("yyyyMM");
                Cod_erroParam.Direction = ParameterDirection.Output;
                Cod_erroParam.Value = 0;
                Desc_erroParam.Direction = ParameterDirection.Output;
                Desc_erroParam.Value = "";
                #endregion

                int result = db.Database.ExecuteSqlCommand("spr_incluir_lancamentos_recorrentes @pEmpresaId, @pAnoMes, @pCod_erro OUT, @pDesc_erro OUT",
                                                               empresaIdParam,
                                                               AnoMesParam,
                                                               Cod_erroParam,
                                                               Desc_erroParam);

                e.mensagem = new Validate() { Code = (int)Cod_erroParam.Value, Message = Desc_erroParam.Value.ToString(), MessageBase = Desc_erroParam.Value.ToString() };
            }
            catch (App_DominioException ex)
            {
                e.mensagem = ex.Result;

                if (ex.InnerException != null)
                    e.mensagem.MessageBase = new App_DominioException(ex.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                else
                    e.mensagem.MessageBase = new App_DominioException(ex.Result.Message, GetType().FullName).Message;
            }
            catch (Exception ex)
            {
                e.mensagem = new Validate() { Code = 999, MessageBase = ex.Message, Message = "Erro de processamento da operação." };

                if (ex.InnerException != null)
                    e.mensagem.MessageBase = new App_DominioException(ex.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                else
                    e.mensagem.MessageBase = new App_DominioException(ex.Message, GetType().FullName).Message;
            }

            return e;
        }

        public IEnumerable<ExercicioViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }
}