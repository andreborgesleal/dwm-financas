using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Models;
using App_Dominio.Security;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DWM.Models.BI
{
    public abstract class OperacaoModifyBI<EORepo, EOPERepo, OPRepo, OPERepo, OPModel, OPEModel, OP, OPE, BI> : DWMContext<ApplicationContext>, IProcess<EORepo, ApplicationContext>
        where EORepo : EditarOperacaoViewModel<EOPERepo>
        where EOPERepo : EditarOperacaoParcelaEventoViewModel
        where OPRepo : OperacaoParcelaViewModel<OPERepo>
        where OPERepo : OperacaoParcelaEventoViewModel
        where OPModel : OperacaoParcelaCrudModel<OP,OPRepo,OPE,OPERepo,OPEModel>
        where OPEModel : OperacaoParcelaEventoModel<OPE, OPERepo>
        where OP : OperacaoParcela<OPE>
        where OPE : OperacaoParcelaEvento
        where BI : OperacaoEditarBI<EORepo, EOPERepo>
    {
        #region Constructor
        public OperacaoModifyBI() { }

        public OperacaoModifyBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        protected OPRepo getOperacaoParcelaRepositoryInstance()
        {
            Type typeInstance = typeof(OPRepo);
            return (OPRepo)Activator.CreateInstance(typeInstance);
        }
        protected OPERepo getOperacaoParcelaEventoRepositoryInstance()
        {
            Type typeInstance = typeof(OPERepo);
            return (OPERepo)Activator.CreateInstance(typeInstance);
        }
        protected OPModel getOperacaoParcelaModelInstance()
        {
            Type typeInstance = typeof(OPModel);
            return (OPModel)Activator.CreateInstance(typeInstance);
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

        public virtual EORepo Run(Repository value)
        {
            EORepo r = (EORepo)value;
            try
            {
                #region Recupera os dados da parcela
                OPRepo operacaoParcela = getOperacaoParcelaRepositoryInstance();
                operacaoParcela.operacaoId = r.operacaoId;
                operacaoParcela.parcelaId = r.parcelaId;

                OPModel model = getOperacaoParcelaModelInstance();
                model.Create(this.db, this.seguranca_db);

                OP entity = model.Find(operacaoParcela);
                operacaoParcela = model.MapToRepository(entity);
                #endregion

                #region Atualiza os dados da parcela
                operacaoParcela.bancoId = r.bancoId;
                operacaoParcela.cheque_banco = r.cheque_banco;
                operacaoParcela.cheque_agencia = r.cheque_agencia;
                operacaoParcela.cheque_numero = r.cheque_numero;
                operacaoParcela.ind_forma_pagamento = r.ind_forma_pagamento;
                operacaoParcela.num_titulo = r.num_titulo;
                operacaoParcela.vr_principal = r.vr_principal;
                operacaoParcela.dt_vencimento = r.dt_vencimento;
                operacaoParcela.uri = r.uri;
                #endregion

                #region Validar a alteração
                value.mensagem = Validar(operacaoParcela);
                if (value.mensagem.Code > 0)
                    throw new App_DominioException(r.mensagem);
                #endregion

                #region estonar o evento de inclusão de operação
                int i = 0;
                while (i <= operacaoParcela.OperacaoParcelaEventos.Count() - 1)
                {
                    if (operacaoParcela.OperacaoParcelaEventos.ElementAt(i).ind_estorno == "N")
                        operacaoParcela.OperacaoParcelaEventos.ElementAt(i).ind_estorno = "S";
                    i++;
                }
                #endregion

                #region Incluir o evento "Alteração de título"

                Evento eve = db.Eventos.Where(info => info.codigo == 8 && info.empresaId == sessaoCorrente.empresaId).FirstOrDefault(); // 8-Alteração de título

                OPERepo operacaoParcelaEvento = getOperacaoParcelaEventoRepositoryInstance();
                operacaoParcelaEvento.operacaoId = r.operacaoId;
                operacaoParcelaEvento.parcelaId = r.parcelaId;
                operacaoParcelaEvento.dt_evento = Funcoes.Brasilia();
                operacaoParcelaEvento.eventoId = eve.eventoId;
                operacaoParcelaEvento.dt_ocorrencia = Funcoes.Brasilia().Date;
                operacaoParcelaEvento.valor = r.vr_principal;
                operacaoParcelaEvento.ind_operacao = eve.ind_operacao;
                operacaoParcelaEvento.ind_estorno = "N";
                operacaoParcelaEvento.ind_tipoEvento = eve.ind_tipoEvento;

                operacaoParcela.OperacaoParcelaEvento = operacaoParcelaEvento;
                ((List<OPERepo>)operacaoParcela.OperacaoParcelaEventos).Add(operacaoParcelaEvento);

                #endregion

                #region Alterar
                operacaoParcela = model.Update(operacaoParcela);
                if (operacaoParcela.mensagem.Code > 0)
                    throw new Exception(operacaoParcela.mensagem.MessageBase);
                #endregion

                r.mensagem = new Validate() { Code = 0, Message = "Baixa realizada com sucesso" };
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

        #region Métodos customizados
        private Validate Validar(OPRepo value)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };

            // Verifica se a parcela está em aberto e se está na situação "Inclusão de operação"
            if (value.OperacaoParcelaEventos.Where(info => info.ind_estorno != "S").Count() > 1)
            {
                value.mensagem.Code = 58;
                value.mensagem.Message = MensagemPadrao.Message(58).text;
                value.mensagem.MessageBase = "Este título não pode ser editado.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            return value.mensagem;
        }
        #endregion
    }
}