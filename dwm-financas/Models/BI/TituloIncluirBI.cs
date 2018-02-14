using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Security;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Data.SqlClient;
using System.Data;

namespace DWM.Models.BI
{
    public class TituloIncluirBI : DWMContext<ApplicationContext>, IProcess<TituloViewModel, ApplicationContext>
    {
        #region Constructor
        public TituloIncluirBI() { }

        public TituloIncluirBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual TituloViewModel Run(Repository value)
        {
            TituloViewModel r = (TituloViewModel)value;
            try
            {
                TituloModel model = new TituloModel();
                model.Create(this.db, this.seguranca_db);
                r = model.BeforeInsert(r);
                Validate v = model.Validate(r, App_Dominio.Enumeracoes.Crud.INCLUIR);
                if (v.Code > 0)
                {
                    r.mensagem = v;
                    return r;
                }

                #region Parâmetros
                SqlParameter operacaoIdParam = new SqlParameter("@pOperacaoId", SqlDbType.Int);
                SqlParameter parcelaIdParam = new SqlParameter("@pParcelaId", SqlDbType.Int);
                SqlParameter SequenciaIDParam = new SqlParameter("@pSequenciaID", SqlDbType.Int);
                SqlParameter BancoIDParam = new SqlParameter("@pBancoID", SqlDbType.NChar, 3);
                SqlParameter ConvenioIDParam = new SqlParameter("@pConvenioID", SqlDbType.NVarChar, 15);
                SqlParameter empresaIdParam = new SqlParameter("@pEmpresaId", SqlDbType.Int);
                SqlParameter TituloIDParam = new SqlParameter("@pTituloID", SqlDbType.NVarChar, 25);
                SqlParameter OcorrenciaIDParam = new SqlParameter("@pOcorrenciaID", SqlDbType.NChar, 2);
                SqlParameter NossoNumeroParam = new SqlParameter("@pNossoNumero", SqlDbType.NChar, 8);
                SqlParameter NossoNumeroDVParam = new SqlParameter("@pNossoNumeroDV", SqlDbType.NChar, 1);
                SqlParameter SeuNumeroParam = new SqlParameter("@pSeuNumero", SqlDbType.NVarChar, 10);
                SqlParameter DataVencimentoParam = new SqlParameter("@pDataVencimento", SqlDbType.SmallDateTime);
                SqlParameter ValorPrincipalParam = new SqlParameter("@pValorPrincipal", SqlDbType.Decimal);


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

                int result = db.Database.ExecuteSqlCommand("spr_titulo_incluir @pEmpresaId, @pAnoMes, @pCod_erro out, @pDesc_erro out",
                                                               empresaIdParam,
                                                               AnoMesParam,
                                                               Cod_erroParam,
                                                               Desc_erroParam);

                r.mensagem = new Validate() { Code = (int)Cod_erroParam.Value, Message = Desc_erroParam.Value.ToString() };


                #region SQLCommand => "Update"
                string command = "update " + Operacao_Table() + " " +
                                 "set " + Cliente_Credor_Atributo() + " = @cliente_credor_ID, " +
                                 "    dt_emissao = @dt_emissao, " +
                                 "    historicoId = @historicoId, " +
                                 "    complementoHist = @complementoHist, " +
                                 "    centroCustoId = " + (r.centroCustoId.HasValue ? r.centroCustoId.Value.ToString() : "null") + ", " +
                                 "    enquadramentoId = " + (r.enquadramentoId.HasValue ? r.enquadramentoId.Value.ToString() : "null") + ", " +
                                 "    documento = @documento, " +
                                 "    vr_jurosMora = @vr_jurosMora, " +
                                 "    vr_multa = @vr_multa, " +
                                 "    recorrencia = @recorrencia " +
                                 "where operacaoId = @operacaoId";

                int result = db.Database.ExecuteSqlCommand(command, new SqlParameter("@cliente_credor_ID", Cliente_Credor_ID(value)),
                                                                    new SqlParameter("@dt_emissao", r.dt_emissao),
                                                                    new SqlParameter("@historicoId", r.historicoId),
                                                                    new SqlParameter("@complementoHist", r.complementoHist ?? ""),
                                                                    new SqlParameter("@documento", r.documento ?? ""),
                                                                    new SqlParameter("@vr_jurosMora", r.vr_jurosMora ?? 0),
                                                                    new SqlParameter("@vr_multa", r.vr_multa ?? 0),
                                                                    new SqlParameter("@recorrencia", r.recorrencia),
                                                                    new SqlParameter("@operacaoId", r.operacaoId));
                #endregion
                r.mensagem = new Validate() { Code = 0, Message = "Registro alterado com sucesso" };
            }
            catch (App_DominioException ex)
            {
                r.mensagem = ex.Result;

                if (ex.InnerException != null)
                    r.mensagem.MessageBase = new App_DominioException(ex.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                else
                    r.mensagem.MessageBase = new App_DominioException(ex.Result.Message, GetType().FullName).Message;
            }
            catch (Exception ex)
            {
                r.mensagem = new Validate() { Code = 999, MessageBase = ex.Message, Message = "Erro de processamento da operação." };

                if (ex.InnerException != null)
                    r.mensagem.MessageBase = new App_DominioException(ex.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                else
                    r.mensagem.MessageBase = new App_DominioException(ex.Message, GetType().FullName).Message;
            }

            return r;
        }


        public IEnumerable<TituloViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }
}