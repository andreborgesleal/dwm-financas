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

namespace DWM.Models.BI
{
    public class ContaPagarEstornoBI : DWMContext<ApplicationContext>, IProcess<EditarContaPagarViewModel, ApplicationContext>
    {
        #region Constructor
        public ContaPagarEstornoBI() { }

        public ContaPagarEstornoBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual EditarContaPagarViewModel Run(Repository value)
        {
            EditarContaPagarParcelaEventoViewModel r = (EditarContaPagarParcelaEventoViewModel)value;
            try
            {
                ContaPagarParcelaEventoModel model = new ContaPagarParcelaEventoModel(this.db, this.seguranca_db);
                MovtoBancarioModel movtoBancarioModel = new MovtoBancarioModel(this.db, this.seguranca_db);
                ContabilidadeModel contabilidadeModel = new ContabilidadeModel(this.db, this.seguranca_db);

                int? contabilidadeId = null;
                int? movtoBancarioId = null;

                ContaPagarParcelaEventoViewModel repository = new ContaPagarParcelaEventoViewModel()
                {
                    operacaoId = r.operacaoId,
                    parcelaId = r.parcelaId,
                    dt_evento = r.dt_evento
                };

                repository = model.getObject(repository); 

                movtoBancarioId = repository.movtoBancarioId;
                contabilidadeId = repository.contabilidadeId;

                repository.movtoBancarioId = null;
                repository.MovtoBancario = null;
                repository.contabilidadeId = null;
                repository.Contabilidade = null;
                repository.ind_estorno = "S";
                repository.uri = r.uri;

                repository = model.Update(repository);
                if (repository.mensagem.Code > 0)
                    throw new Exception(repository.mensagem.MessageBase);

                #region Exclui o movimento bancário
                if (movtoBancarioId.HasValue)
                {
                    MovtoBancarioViewModel movtoBancarioViewModel = new MovtoBancarioViewModel()
                    {
                        movtoBancarioId = movtoBancarioId.Value
                    };

                    movtoBancarioViewModel = movtoBancarioModel.getObject(movtoBancarioViewModel);
                    movtoBancarioViewModel.uri = r.uri;
                    movtoBancarioViewModel = movtoBancarioModel.Delete(movtoBancarioViewModel);
                    if (movtoBancarioViewModel.mensagem.Code > 0)
                        throw new Exception(movtoBancarioViewModel.mensagem.MessageBase);
                }
                #endregion

                #region Exclui a contabilidade
                if (contabilidadeId.HasValue)
                {
                    ContabilidadeViewModel contabilidadeViewModel = new ContabilidadeViewModel()
                    {
                        contabilidadeId = contabilidadeId.Value
                    };

                    contabilidadeViewModel = contabilidadeModel.getObject(contabilidadeViewModel);
                    contabilidadeViewModel.uri = r.uri;
                    contabilidadeViewModel = contabilidadeModel.Delete(contabilidadeViewModel);
                    if (contabilidadeViewModel.mensagem.Code > 0)
                        throw new Exception(contabilidadeViewModel.mensagem.MessageBase);
                }
                #endregion

                r.mensagem = new Validate() { Code = 0, Message = "Registro estornado com sucesso" };
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

            return new EditarContaPagarViewModel()
            {
                mensagem = r.mensagem
            };
        }

        public IEnumerable<EditarContaPagarViewModel> List(params object[] param)
        {
            EditarContaPagarBI bi = new EditarContaPagarBI(this.db, this.seguranca_db);
            IList<EditarContaPagarViewModel> list = new List<EditarContaPagarViewModel>();
            list.Add(bi.Run(new EditarContaPagarViewModel() { operacaoId = (int)param[0], parcelaId = (int)param[1] }));
            return list;
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }
}