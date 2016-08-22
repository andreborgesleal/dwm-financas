using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Models;
using App_Dominio.Security;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace DWM.Models.BI
{
    public abstract class OperacaoCancelarBI<ORepo, OPRepo, OPERepo, OModel, OPModel, OPEModel, O, OP, OPE > : DWMContext<ApplicationContext>, IProcess<ORepo, ApplicationContext>
        where ORepo : OperacaoViewModel<OPRepo, OPERepo>
        where OPRepo : OperacaoParcelaViewModel<OPERepo>
        where OPERepo : OperacaoParcelaEventoViewModel
        where OModel : OperacaoModel<O, ORepo, OP, OPRepo, OPE, OPERepo, OPModel, OPEModel>
        where OPModel : OperacaoParcelaCrudModel<OP, OPRepo, OPE, OPERepo, OPEModel>
        where OPEModel : OperacaoParcelaEventoModel<OPE, OPERepo>
        where OPE : OperacaoParcelaEvento
        where OP : OperacaoParcela<OPE>
        where O : Operacao<OP, OPE>
    {
        #region Constructor
        public OperacaoCancelarBI() { }

        public OperacaoCancelarBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        protected ORepo getOperacaoRepositoryInstance()
        {
            Type typeInstance = typeof(ORepo);
            return (ORepo)Activator.CreateInstance(typeInstance);
        }
        protected OPERepo getOperacaoParcelaEventoRepositoryInstance()
        {
            Type typeInstance = typeof(OPERepo);
            return (OPERepo)Activator.CreateInstance(typeInstance);
        }

        protected OModel getOperacaoModelInstance()
        {
            Type typeInstance = typeof(OModel);
            return (OModel)Activator.CreateInstance(typeInstance);
        }
        protected OPEModel getOperacaoParcelaEventoModelInstance()
        {
            Type typeInstance = typeof(OPEModel);
            return (OPEModel)Activator.CreateInstance(typeInstance);
        }
        #endregion

        public virtual ORepo Run(Repository value)
        {
            ORepo r = (ORepo)value;
            ORepo repository = getOperacaoRepositoryInstance();
            repository.operacaoId = r.operacaoId;
            repository.mensagem = new Validate() { Code = 0, Message = "Registro cancelado com sucesso" };

            try
            {
                OModel model = getOperacaoModelInstance();
                model.Create(this.db, this.seguranca_db);

                OPEModel operacaoParcelaEventoModel = getOperacaoParcelaEventoModelInstance();
                operacaoParcelaEventoModel.Create(this.db, this.seguranca_db);

                repository = model.getObject(repository);

                int seconds = 0;
                int i = 0;
                Evento eve = db.Eventos.Where(info => info.codigo == 5 && info.empresaId == sessaoCorrente.empresaId).FirstOrDefault();

                foreach (OPRepo par in repository.OperacaoParcelas)
                {
                    if (!"4|5".Contains(par.ind_baixa ?? " "))
                    {
                        OPERepo operacaoParcelaEvento = getOperacaoParcelaEventoRepositoryInstance();
                        operacaoParcelaEvento.operacaoId = repository.operacaoId;
                        operacaoParcelaEvento.parcelaId = par.parcelaId;
                        operacaoParcelaEvento.dt_evento = Funcoes.Brasilia().AddSeconds(seconds);
                        operacaoParcelaEvento.eventoId = eve.eventoId;
                        operacaoParcelaEvento.dt_ocorrencia = Funcoes.Brasilia().Date;
                        operacaoParcelaEvento.valor = par.vr_saldo_devedor.Value;
                        operacaoParcelaEvento.ind_operacao = eve.ind_operacao;
                        operacaoParcelaEvento.ind_estorno = "N";
                        operacaoParcelaEvento.ind_tipoEvento = eve.ind_tipoEvento;
                        operacaoParcelaEvento.uri = r.uri;

                        operacaoParcelaEvento = operacaoParcelaEventoModel.Insert(operacaoParcelaEvento);
                        if (operacaoParcelaEvento.mensagem.Code > 0)
                            throw new Exception(operacaoParcelaEvento.mensagem.MessageBase);

                        seconds += 2;
                    }

                    i++;
                }

                if (seconds == 0)
                    repository.mensagem = new Validate() { Code = 20, Message = "Operação já havia sido cancelada" };
                else
                    repository.mensagem = new Validate() { Code = 0, Message = "Operação cancelada com sucesso !!!" };
            }
            catch (App_DominioException ex)
            {
                repository.mensagem = ex.Result;

                if (ex.InnerException != null)
                    repository.mensagem.MessageBase = new App_DominioException(ex.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                else
                    repository.mensagem.MessageBase = new App_DominioException(ex.Result.Message, GetType().FullName).Message;
            }
            catch (Exception ex)
            {
                repository.mensagem = new Validate() { Code = 999, MessageBase = ex.Message, Message = "Erro de processamento da operação." };

                if (ex.InnerException != null)
                    repository.mensagem.MessageBase = new App_DominioException(ex.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                else
                    repository.mensagem.MessageBase = new App_DominioException(ex.Message, GetType().FullName).Message;
            }

            return repository;
        }

        public IEnumerable<ORepo> List(params object[] param)
        {
            ORepo repository = getOperacaoRepositoryInstance();
            repository.operacaoId = (int)param[0];
            repository.mensagem = new Validate() { Code = 0, Message = "Registro cancelado com sucesso" };

            OModel model = getOperacaoModelInstance();
            model.Create(this.db, this.seguranca_db);

            repository = model.getObject(repository);

            int i = 0;

            while (i <= repository.OperacaoParcelas.Count() - 1)
                repository.OperacaoParcelas.ElementAt(i++).vr_saldo_devedor = 0;

            repository.OperacaoParcela.vr_saldo_devedor = 0;

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