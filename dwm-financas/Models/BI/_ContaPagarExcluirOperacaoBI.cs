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
    public class ContaPagarExcluirOperacaoBI : DWMContext<ApplicationContext>, IProcess<ContaPagarViewModel, ApplicationContext>
    {
        #region Constructor
        public ContaPagarExcluirOperacaoBI() { }

        public ContaPagarExcluirOperacaoBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual ContaPagarViewModel Run(Repository value)
        {
            ContaPagarViewModel r = (ContaPagarViewModel)value;
            ContaPagarViewModel repository = new ContaPagarViewModel()
            {
                operacaoId = r.operacaoId,
                mensagem = new Validate() { Code = 0, Message = "Registro excluído com sucesso" }
            };

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
                int result = db.Database.ExecuteSqlCommand("spr_excluir_operacao_contas_pagar @pOperacaoId, @pERROR_CODE, @pERROR_DESC",
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

            return new ContaPagarViewModel()
            {
                mensagem = r.mensagem
            };
        }

        public IEnumerable<ContaPagarViewModel> List(params object[] param)
        {
            ContaPagarViewModel repository = new ContaPagarViewModel()
            {
                operacaoId = (int)param[0],
                mensagem = new Validate() { Code = 0, Message = "Registro excluído com sucesso" }
            };

            ContaPagarModel model = new ContaPagarModel(this.db, this.seguranca_db);
            repository = model.CreateRepository();

            IList<ContaPagarViewModel> list = new List<ContaPagarViewModel>();
            list.Add(repository);

            return list.ToList();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }
}