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

namespace DWM.Models.BI
{
    public abstract class OperacaoParcelaEventoBI<EORepo, EOPERepo, ORepo, OPRepo, OPERepo, OPEModel, OPE, BI> : DWMContext<ApplicationContext>, IProcess<EORepo, ApplicationContext>
        where EORepo : EditarOperacaoViewModel<EOPERepo>
        where EOPERepo : EditarOperacaoParcelaEventoViewModel
        where ORepo : OperacaoViewModel<OPRepo, OPERepo>
        where OPRepo : OperacaoParcelaViewModel<OPERepo>
        where OPERepo : OperacaoParcelaEventoViewModel
        where OPEModel : OperacaoParcelaEventoModel<OPE,OPERepo>
        where OPE : OperacaoParcelaEvento
        where BI : OperacaoEditarBI<EORepo, EOPERepo>
    {
        #region Constructor
        public OperacaoParcelaEventoBI() { }

        public OperacaoParcelaEventoBI(ApplicationContext _db, SecurityContext _seguranca_db)
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
        protected OPEModel getOperacaoParcelaEventoModelInstance()
        {
            Type typeInstance = typeof(OPEModel);
            return (OPEModel)Activator.CreateInstance(typeInstance);
        }
        protected EORepo getEditarOperacaoRepositoryInstance()
        {
            Type typeInstance = typeof(EORepo);
            return (EORepo)Activator.CreateInstance(typeInstance);
        }
        protected BI getOperacaoEditarBIInstance()
        {
            Type typeInstance = typeof(BI);
            return (BI)Activator.CreateInstance(typeInstance);
        }
        #endregion

        #region Abstract Methods
        protected abstract string getTipoMovto();
        #endregion
        public virtual EORepo Run(Repository value)
        {
            EOPERepo r = (EOPERepo)value;
            try
            {
                #region Calcula o próximo dia útil em relação à data de referência
                DateTime? dt_referencia = r.dt_ocorrencia;
                ObterProximoDiaUtil obterProximoDiaUtil = new ObterProximoDiaUtil(this.db, this.seguranca_db, dt_referencia);
                obterProximoDiaUtil.Run(new FeriadoViewModel());
                DateTime dt_proximo_diaUtil = obterProximoDiaUtil.dt_referencia;
                #endregion

                Evento eve = null;

                #region Novo evento
                eve = db.Eventos.Find(r.eventoId);
                OPERepo operacaoParcelaEvento = getOperacaoParcelaEventoRepositoryInstance();
                operacaoParcelaEvento.operacaoId = r.operacaoId;
                operacaoParcelaEvento.parcelaId = r.parcelaId;
                operacaoParcelaEvento.dt_evento = Funcoes.Brasilia();
                operacaoParcelaEvento.eventoId = eve.eventoId;
                operacaoParcelaEvento.dt_ocorrencia = dt_proximo_diaUtil;
                operacaoParcelaEvento.valor = r.valor;
                operacaoParcelaEvento.ind_operacao = eve.ind_operacao;
                operacaoParcelaEvento.ind_estorno = "N";
                operacaoParcelaEvento.ind_tipoEvento = eve.ind_tipoEvento;
                operacaoParcelaEvento.arquivo = r.arquivo;
                operacaoParcelaEvento.enquadramentoId = r.enquadramentoId;
                operacaoParcelaEvento.uri = r.uri;

                OPEModel eveModel = getOperacaoParcelaEventoModelInstance();
                eveModel.Create(this.db, this.seguranca_db);

                #region Gerar Movimentação Bancária
                if (r.bancoId.HasValue && r.bancoId > 0)
                {
                    MovtoBancarioViewModel movtoViewModel = new MovtoBancarioViewModel()
                    {
                        empresaId = sessaoCorrente.empresaId,
                        bancoId = r.bancoId.Value,
                        historicoId = r.historicoId,
                        complementoHist = r.complementoHist,
                        dt_movto = dt_proximo_diaUtil,
                        valor = r.valor,
                        tipoMovto = getTipoMovto()
                    };
                    operacaoParcelaEvento.MovtoBancario = movtoViewModel;
                }
                #endregion

                operacaoParcelaEvento = eveModel.Insert(operacaoParcelaEvento);
                if (operacaoParcelaEvento.mensagem.Code > 0)
                    throw new Exception(operacaoParcelaEvento.mensagem.MessageBase);
                #endregion

                r.mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso" };
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