using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using System;
using App_Dominio.Models;

namespace DWM.Models.Persistence
{
    public class _ContaPagarParcelaCrudModel : CrudModel<ContaPagarParcela, ContaPagarParcelaViewModel, ApplicationContext>
    {
        #region Constructor
        public _ContaPagarParcelaCrudModel() { }
        public _ContaPagarParcelaCrudModel(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe CrudModel
        public override ContaPagarParcela MapToEntity(ContaPagarParcelaViewModel value)
        {
            ContaPagarParcela p = db.ContaPagarParcelas.SingleOrDefault(info => info.operacaoId == value.operacaoId && info.parcelaId == value.parcelaId); // Find(value);

            #region ContaPagarParcela
            if (p == null)
            {
            	p = new ContaPagarParcela();
            	p.ContaPagarParcelaEventos = new List<ContaPagarParcelaEvento>();
            }
            else
            	p.ContaPagarParcelaEventos.Clear();

            p.operacaoId = value.operacaoId;
            p.parcelaId = value.parcelaId;
            p.bancoId = value.bancoId;
            p.num_titulo = value.num_titulo;
            p.dt_vencimento = value.dt_vencimento;
            p.vr_principal = value.vr_principal;
            p.vr_amortizacao = value.vr_amortizacao;
            p.vr_desconto = value.vr_desconto;
            p.vr_encargos = value.vr_encargos;
            p.vr_saldo_devedor = value.vr_saldo_devedor;
            p.ind_forma_pagamento = value.ind_forma_pagamento;
            p.codigo_barras = value.codigo_barras;
            p.dt_ultima_amortizacao = value.dt_ultima_amortizacao;
            p.ind_baixa = value.ind_baixa;
            p.dt_baixa = value.dt_baixa;
            p.cheque_banco = value.cheque_banco;
            p.cheque_agencia = value.cheque_agencia;
            p.cheque_numero = value.cheque_numero;
            #endregion

            #region Eventos da parcela
            ContaPagarParcelaEventoModel contaPagarParcelaEventoModel = new ContaPagarParcelaEventoModel(this.db, this.seguranca_db);
            foreach (ContaPagarParcelaEventoViewModel pev in value.ContaPagarParcelaEventos)
            	p.ContaPagarParcelaEventos.Add(contaPagarParcelaEventoModel.MapToEntity(pev));
            #endregion

            return p;
        }

        public override ContaPagarParcelaViewModel MapToRepository(ContaPagarParcela entity)
        {
            #region ContaPagarParcela
            ContaPagarParcelaViewModel x = new ContaPagarParcelaViewModel()
            {
                operacaoId = entity.operacaoId,
                parcelaId = entity.parcelaId,
                bancoId = entity.bancoId,
                nome_banco = entity.bancoId != null ? db.Bancos.Find(entity.bancoId).nome : "",
                num_titulo = entity.num_titulo,
                dt_vencimento = entity.dt_vencimento,
                vr_principal = entity.vr_principal,
                vr_encargos = entity.vr_encargos,
                vr_amortizacao = entity.vr_amortizacao,
                vr_total_pago = entity.vr_total_pago,
                dt_ultima_amortizacao = entity.dt_ultima_amortizacao,
                vr_saldo_devedor = entity.vr_saldo_devedor,
                ind_forma_pagamento = entity.ind_forma_pagamento,
                codigo_barras = entity.codigo_barras,
                ind_baixa = entity.ind_baixa,
                dt_baixa = entity.dt_baixa,
                cheque_banco = entity.cheque_banco,
                cheque_agencia = entity.cheque_agencia,
                cheque_numero = entity.cheque_numero,
                ContaPagarParcelaEventos = new List<ContaPagarParcelaEventoViewModel>()
            };
            #endregion

            #region ContaPagarParcelaEventos
            ContaPagarParcelaEventoModel contaPagarParcelaEventoModel = new ContaPagarParcelaEventoModel(this.db, this.seguranca_db);

            foreach (ContaPagarParcelaEvento e in entity.ContaPagarParcelaEventos)
            {
            	ContaPagarParcelaEventoViewModel pev = contaPagarParcelaEventoModel.MapToRepository(e);
                ((List<ContaPagarParcelaEventoViewModel>)x.ContaPagarParcelaEventos).Add(pev);
            }
            #endregion

            return x;
        }

        public override ContaPagarParcela Find(ContaPagarParcelaViewModel key)
        {
            if (key.operacaoId == 0 || key.parcelaId == 0)
                return null;

            return db.ContaPagarParcelas.SingleOrDefault(info => info.operacaoId == key.operacaoId && info.parcelaId == key.parcelaId); //db.ContaPagarParcelas.Find(new { key.operacaoId, key.parcelaId });
        }

        public override Validate Validate(ContaPagarParcelaViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };

            if (operation != Crud.INCLUIR && value.operacaoId <= 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Operação ID").ToString();
                value.mensagem.MessageBase = "Campo obrigatório: Operação ID";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.parcelaId <= 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Nº Parcela").ToString();
                value.mensagem.MessageBase = "Campo obrigatório: Número da parcela";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }


            if (operation != Crud.EXCLUIR)
            {
                if (value.dt_vencimento <= Convert.ToDateTime("1980-01-01"))
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Dt.Vencimento").ToString();
                    value.mensagem.MessageBase = "Campo obrigatório: Dt. Vencimento";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.vr_principal <= 0)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Vr.Principal").ToString();
                    value.mensagem.MessageBase = "Campo obrigatório: Valor Principal";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (!"1|2|3|4|5|6|9".Contains(value.ind_forma_pagamento))
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Vr.Principal").ToString();
                    value.mensagem.MessageBase = "Campo obrigatório: Forma de pagamento";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (!value.bancoId.HasValue && value.vr_amortizacao > 0)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Banco").ToString();
                    value.mensagem.MessageBase = "Campo obrigatório: Banco";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }


                if (value.ContaPagarParcelaEventos.Count() == 0)
                {
                    value.mensagem.Code = 46;
                    value.mensagem.MessageBase = MensagemPadrao.Message(46, "evento de contas a pagar").ToString();
                    value.mensagem.Message = "Deve ser incluído pelo menos um evento para a parcela.";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                // Validar eventos da parcela
                ContaPagarParcelaEventoModel pevModel = new ContaPagarParcelaEventoModel(this.db, this.seguranca_db);
                foreach (ContaPagarParcelaEventoViewModel pev in value.ContaPagarParcelaEventos)
                {
                    Validate validate = pevModel.Validate(pev, operation);
                    if (validate.Code > 0)
                        return validate;
                }

            }

            return value.mensagem;
        }

        public override ContaPagarParcelaViewModel CreateRepository(System.Web.HttpRequestBase Request = null)
        {
            ContaPagarParcelaViewModel r = new ContaPagarParcelaViewModel()
            {
                parcelaId = 1,
                dt_vencimento = Funcoes.Brasilia().Date,
                ContaPagarParcelaEvento = new ContaPagarParcelaEventoViewModel()
                {
                    parcelaId = 1,
                    dt_ocorrencia = Funcoes.Brasilia().Date,
                    dt_movto = Funcoes.Brasilia().Date
                }
            };

            return r;
        }
        #endregion

        #region Métodos Customizados

        #endregion
    }
}