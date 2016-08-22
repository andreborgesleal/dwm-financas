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
    public class ModifyContaReceberBI : DWMContext<ApplicationContext>, IProcess<EditarContaReceberViewModel, ApplicationContext>
    {
        #region Constructor
        public ModifyContaReceberBI() { }

        public ModifyContaReceberBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual EditarContaReceberViewModel Run(Repository value)
        {
            EditarContaReceberViewModel r = (EditarContaReceberViewModel)value;
            try
            {
                #region Recupera os dados da parcela
                ContaReceberParcelaViewModel contaReceberParcela = new ContaReceberParcelaViewModel()
                {
                    operacaoId = r.operacaoId,
                    parcelaId = r.parcelaId
                };
                ContaReceberParcelaCrudModel model = new ContaReceberParcelaCrudModel();
                model.Create(this.db, this.seguranca_db);
                ContaReceberParcela entity = db.ContaReceberParcelas.SingleOrDefault(info => info.operacaoId == r.operacaoId && info.parcelaId == r.parcelaId);
                contaReceberParcela = model.MapToRepository(entity);
                #endregion

                #region Atualiza os dados da parcela
                contaReceberParcela.bancoId = r.bancoId;
                contaReceberParcela.cheque_banco = r.cheque_banco;
                contaReceberParcela.cheque_agencia = r.cheque_agencia;
                contaReceberParcela.cheque_numero = r.cheque_numero;
                contaReceberParcela.ind_forma_pagamento = r.ind_forma_pagamento;
                contaReceberParcela.num_titulo = r.num_titulo;
                contaReceberParcela.vr_principal = r.vr_principal;
                contaReceberParcela.dt_vencimento = r.dt_vencimento;
                contaReceberParcela.uri = r.uri;
                #endregion

                #region Validar a alteração
                value.mensagem = Validar(contaReceberParcela);
                if (value.mensagem.Code > 0)
                    throw new App_DominioException(r.mensagem);
                #endregion

                #region estonar o evento de inclusão de operação
                int i = 0;
                while (i <= contaReceberParcela.OperacaoParcelaEventos.Count() - 1)
                {
                    if (contaReceberParcela.OperacaoParcelaEventos.ElementAt(i).ind_estorno == "N")
                        contaReceberParcela.OperacaoParcelaEventos.ElementAt(i).ind_estorno = "S";
                    i++;
                }
                #endregion

                #region Incluir o evento "Alteração de título"

                Evento eve = db.Eventos.Where(info => info.codigo == 8 && info.empresaId == sessaoCorrente.empresaId).FirstOrDefault(); // 8-Alteração de título

                ContaReceberParcelaEventoViewModel contaReceberParcelaEvento = new ContaReceberParcelaEventoViewModel()
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

                contaReceberParcela.OperacaoParcelaEvento = contaReceberParcelaEvento;
                ((List<ContaReceberParcelaEventoViewModel>)contaReceberParcela.OperacaoParcelaEventos).Add(contaReceberParcelaEvento);

                #endregion

                #region Alterar
                contaReceberParcela = model.Update(contaReceberParcela);
                if (contaReceberParcela.mensagem.Code > 0)
                    throw new Exception(contaReceberParcela.mensagem.MessageBase);
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

        public IEnumerable<EditarContaReceberViewModel> List(params object[] param)
        {
            ContaReceberEditarBI bi = new ContaReceberEditarBI(this.db, this.seguranca_db);
            IList<EditarContaReceberViewModel> list = new List<EditarContaReceberViewModel>();
            list.Add(bi.Run(new EditarContaReceberViewModel() { operacaoId = (int)param[0], parcelaId = (int)param[1] }));
            return list;
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }

        #region Métodos customizados
        private Validate Validar(ContaReceberParcelaViewModel value)
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