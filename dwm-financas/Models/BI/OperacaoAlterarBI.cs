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

namespace DWM.Models.BI
{
    public abstract class OperacaoAlterarBI<ORepo, OPRepo, OPERepo, OModel, OPModel, OPEModel, O, OP, OPE> : DWMContext<ApplicationContext>, IProcess<ORepo, ApplicationContext>
        where ORepo : OperacaoViewModel<OPRepo, OPERepo>
        where OPRepo : OperacaoParcelaViewModel<OPERepo>
        where OPERepo : OperacaoParcelaEventoViewModel
        where OModel : OperacaoModel<O, ORepo, OP, OPRepo, OPE, OPERepo, OPModel, OPEModel>
        where OPModel : OperacaoParcelaCrudModel<OP, OPRepo, OPE, OPERepo, OPEModel>
        where OPEModel : OperacaoParcelaEventoModel<OPE, OPERepo>
        where O : Operacao<OP, OPE>
        where OP : OperacaoParcela<OPE>
        where OPE : OperacaoParcelaEvento
    {
        #region Constructor
        public OperacaoAlterarBI() { }

        public OperacaoAlterarBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        protected ORepo getOperacaoRepositoryInstance()
        {
            Type typeInstance = typeof(ORepo);
            return (ORepo)Activator.CreateInstance(typeInstance);
        }
        protected OModel getOperacaoModelInstance()
        {
            Type typeInstance = typeof(OModel);
            return (OModel)Activator.CreateInstance(typeInstance);
        }
        #endregion

        #region Abstract Methods
        protected abstract string Operacao_Table();
        protected abstract int Cliente_Credor_ID(Repository value);
        protected abstract string Cliente_Credor_Atributo();
        #endregion

        public virtual ORepo Run(Repository value)
        {
            ORepo r = (ORepo)value;

            try
            {
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

        public IEnumerable<ORepo> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }


    }
}