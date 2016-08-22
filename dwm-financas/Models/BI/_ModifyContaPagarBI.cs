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
    public class ModifyContaPagarBI : DWMContext<ApplicationContext>, IProcess<EditarContaPagarViewModel, ApplicationContext>
    {
        #region Constructor
        public ModifyContaPagarBI() { }

        public ModifyContaPagarBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual EditarContaPagarViewModel Run(Repository value)
        {
            EditarContaPagarViewModel r = (EditarContaPagarViewModel)value;
            try
            {
                #region Recupera os dados da parcela
                ContaPagarParcelaViewModel contaPagarParcela = new ContaPagarParcelaViewModel()
                {
                    operacaoId = r.operacaoId,
                    parcelaId = r.parcelaId
                };
                ContaPagarParcelaCrudModel model = new ContaPagarParcelaCrudModel(this.db, this.seguranca_db);
                ContaPagarParcela entity = db.ContaPagarParcelas.SingleOrDefault(info => info.operacaoId == r.operacaoId && info.parcelaId == r.parcelaId); 
                contaPagarParcela = model.MapToRepository(entity);
                #endregion

                #region Atualiza os dados da parcela
                contaPagarParcela.bancoId = r.bancoId;
                contaPagarParcela.cheque_banco = r.cheque_banco;
                contaPagarParcela.cheque_agencia = r.cheque_agencia;
                contaPagarParcela.cheque_numero = r.cheque_numero;
                contaPagarParcela.ind_forma_pagamento = r.ind_forma_pagamento;
                contaPagarParcela.num_titulo = r.num_titulo;
                contaPagarParcela.vr_principal = r.vr_principal;
                contaPagarParcela.dt_vencimento = r.dt_vencimento;
                contaPagarParcela.uri = r.uri;
                #endregion

                #region Validar a alteração
                value.mensagem = Validar(contaPagarParcela);
                if (value.mensagem.Code > 0)
                    throw new App_DominioException(r.mensagem);
                #endregion

                #region estonar o evento de inclusão de operação
                int i = 0;
                while (i <= contaPagarParcela.ContaPagarParcelaEventos.Count() - 1)
                {
                    if (contaPagarParcela.ContaPagarParcelaEventos.ElementAt(i).ind_estorno == "N")
                        contaPagarParcela.ContaPagarParcelaEventos.ElementAt(i).ind_estorno = "S";
                    i++;
                }
                #endregion

                #region Incluir o evento "Alteração de título"

                Evento eve = db.Eventos.Where(info => info.codigo == 8 && info.empresaId == sessaoCorrente.empresaId).FirstOrDefault(); // 8-Alteração de título

                ContaPagarParcelaEventoViewModel contaPagarParcelaEvento = new ContaPagarParcelaEventoViewModel()
                {
                    operacaoId = r.operacaoId,
                    parcelaId = r.parcelaId,
                    dt_evento = Funcoes.Brasilia(),
                    eventoId = eve.eventoId,
                    dt_ocorrencia = Funcoes.Brasilia().Date,
                    valor = r.vr_principal,
                    ind_operacao = eve.ind_operacao,
                    ind_estorno = "N",
                    ind_tipoEvento = eve.ind_tipoEvento
                };

                contaPagarParcela.ContaPagarParcelaEvento = contaPagarParcelaEvento;
                ((List<ContaPagarParcelaEventoViewModel>)contaPagarParcela.ContaPagarParcelaEventos).Add(contaPagarParcelaEvento);
                
                #endregion

                #region Alterar
                contaPagarParcela = model.Update(contaPagarParcela);
                if (contaPagarParcela.mensagem.Code > 0)
                    throw new Exception(contaPagarParcela.mensagem.MessageBase);
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

        #region Métodos customizados
        private Validate Validar(ContaPagarParcelaViewModel value)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };

            // Verifica se a parcela está em aberto e se está na situação "Inclusão de operação"
            if (value.ContaPagarParcelaEventos.Where(info => info.ind_estorno != "S").Count() > 1)
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