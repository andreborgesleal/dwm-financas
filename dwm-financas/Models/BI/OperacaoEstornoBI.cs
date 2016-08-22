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
    public abstract class OperacaoEstornoBI<EORepo, EOPERepo, OPModel, OPEModel, OPRepo, OPERepo, OP, OPE, BI> : DWMContext<ApplicationContext>, IProcess<EORepo, ApplicationContext>
        where EORepo : EditarOperacaoViewModel<EOPERepo>
        where EOPERepo : EditarOperacaoParcelaEventoViewModel
        where OPModel : OperacaoParcelaCrudModel<OP, OPRepo, OPE, OPERepo, OPEModel>
        where OPEModel : OperacaoParcelaEventoModel<OPE, OPERepo>
        where OP : OperacaoParcela<OPE>
        where OPE : OperacaoParcelaEvento
        where OPRepo : OperacaoParcelaViewModel<OPERepo>
        where OPERepo : OperacaoParcelaEventoViewModel
        where BI : OperacaoEditarBI<EORepo,EOPERepo>
    {
        #region Constructor
        public OperacaoEstornoBI() { }

        public OperacaoEstornoBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        protected EOPERepo getEditarOperacaoParcelaEventoRepositoryInstance()
        {
            Type typeInstance = typeof(EOPERepo);
            return (EOPERepo)Activator.CreateInstance(typeInstance);
        }
        protected EORepo getEditarOperacaoRepositoryInstance()
        {
            Type typeInstance = typeof(EORepo);
            return (EORepo)Activator.CreateInstance(typeInstance);
        }
        protected OPEModel getOperacaoParcelaEventoModelInstance()
        {
            Type typeInstance = typeof(OPEModel);
            return (OPEModel)Activator.CreateInstance(typeInstance);
        }
        protected OPERepo getOperacaoParcelaEventoRepositoryInstance()
        {
            Type typeInstance = typeof(OPERepo);
            return (OPERepo)Activator.CreateInstance(typeInstance);
        }
        protected BI getOperacaoEditarBIInstance()
        {
            Type typeInstance = typeof(BI);
            return (BI)Activator.CreateInstance(typeInstance);
        }
        #endregion

        public virtual EORepo Run(Repository value)
        {
            EOPERepo r = (EOPERepo)value;

            try
            {
                OPEModel model = getOperacaoParcelaEventoModelInstance();
                model.Create(this.db, this.seguranca_db);

                MovtoBancarioModel movtoBancarioModel = new MovtoBancarioModel(this.db, this.seguranca_db);
                ContabilidadeModel contabilidadeModel = new ContabilidadeModel(this.db, this.seguranca_db);

                int? contabilidadeId = null;
                int? movtoBancarioId = null;

                OPERepo repository = getOperacaoParcelaEventoRepositoryInstance();
                repository.operacaoId = r.operacaoId;
                repository.parcelaId = r.parcelaId;
                repository.dt_evento = r.dt_evento;

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

            EORepo x = getEditarOperacaoRepositoryInstance();
            x.mensagem = r.mensagem;
            return x;
        }

        public IEnumerable<EORepo> List(params object[] param)
        {
            BI bi = getOperacaoEditarBIInstance();
            bi.Create(this.db, this.seguranca_db);
            IList<EORepo> list = new List<EORepo>();
            EORepo editarOperacaoViewModel = getEditarOperacaoRepositoryInstance();
            editarOperacaoViewModel.operacaoId = (int)param[0];
            editarOperacaoViewModel.parcelaId = (int)param[1];

            list.Add(bi.Run(editarOperacaoViewModel));
            return list;
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }

    }
}