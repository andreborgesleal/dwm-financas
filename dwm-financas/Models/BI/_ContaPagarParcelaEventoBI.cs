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
    public class ContaPagarParcelaEventoBI : DWMContext<ApplicationContext>, IProcess<EditarContaPagarViewModel, ApplicationContext>
    {
        #region Constructor
        public ContaPagarParcelaEventoBI() { }

        public ContaPagarParcelaEventoBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual EditarContaPagarViewModel Run(Repository value)
        {
            EditarContaPagarParcelaEventoViewModel r = (EditarContaPagarParcelaEventoViewModel)value;
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
                ContaPagarParcelaEventoViewModel contaPagarParcelaEvento = new ContaPagarParcelaEventoViewModel()
                {
                    operacaoId = r.operacaoId,
                    parcelaId = r.parcelaId,
                    dt_evento = Funcoes.Brasilia(),
                    eventoId = eve.eventoId,
                    dt_ocorrencia = dt_proximo_diaUtil,
                    valor = r.valor,
                    ind_operacao = eve.ind_operacao,
                    ind_estorno = "N",
                    ind_tipoEvento = eve.ind_tipoEvento,
                    arquivo = r.arquivo,
                    enquadramentoId = r.enquadramentoId,
                    uri = r.uri
                };

                ContaPagarParcelaEventoModel eveModel = new ContaPagarParcelaEventoModel(this.db, this.seguranca_db);
                
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
                        tipoMovto = "D"
                    };
                    contaPagarParcelaEvento.MovtoBancario = movtoViewModel;
                }
                #endregion

                contaPagarParcelaEvento = eveModel.Insert(contaPagarParcelaEvento);
                if (contaPagarParcelaEvento.mensagem.Code > 0)
                    throw new Exception(contaPagarParcelaEvento.mensagem.MessageBase);
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