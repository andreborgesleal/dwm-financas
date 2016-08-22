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
    public abstract class OperacaoExcluirBI<ORepo, OPRepo, OPERepo, OModel, OPModel, OPEModel, O, OP, OPE> : DWMContext<ApplicationContext>, IProcess<ORepo, ApplicationContext>
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
        public OperacaoExcluirBI() { }

        public OperacaoExcluirBI(ApplicationContext _db, SecurityContext _seguranca_db)
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
        protected abstract string spr_excluir_operacao();
        #endregion

        public virtual ORepo Run(Repository value)
        {
            ORepo r = (ORepo)value;
            ORepo repository = getOperacaoRepositoryInstance();
            repository.operacaoId = r.operacaoId;
            repository.mensagem = new Validate() { Code = 0, Message = "Registro excluído com sucesso" };

            try
            {
                var _error_code = new SqlParameter
                {
                    ParameterName = "pERROR_CODE",
                    DbType = System.Data.DbType.Int16,
                    Direction = System.Data.ParameterDirection.Output
                };

                var _error_desc = new SqlParameter
                {
                    ParameterName = "pERROR_DESC",
                    DbType = System.Data.DbType.String,
                    Size = 1000,
                    Direction = System.Data.ParameterDirection.Output
                };

                #region stored procedure
                int result = db.Database.ExecuteSqlCommand(spr_excluir_operacao() + " @pOperacaoId, @pERROR_CODE, @pERROR_DESC",
                                                           new SqlParameter("@pOperacaoId", r.operacaoId),
                                                           _error_code,
                                                           _error_desc);
                #endregion
                r.mensagem = new Validate() { Code = 0, Message = "Registro excluído com sucesso" };
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

            ORepo x = getOperacaoRepositoryInstance();
            x.mensagem = r.mensagem;
            return x;
        }

        public IEnumerable<ORepo> List(params object[] param)
        {
            ORepo repository = getOperacaoRepositoryInstance();
            repository.operacaoId = (int)param[0];
            repository.mensagem = new Validate() { Code = 0, Message = "Registro excluído com sucesso" };

            OModel model = getOperacaoModelInstance();
            model.Create(this.db, this.seguranca_db);

            repository = model.CreateRepository();

            IList<ORepo> list = new List<ORepo>();
            list.Add(repository);

            return list.ToList();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }
}