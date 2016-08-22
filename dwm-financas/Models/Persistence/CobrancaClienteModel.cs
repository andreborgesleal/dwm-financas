using System;
using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Models;
using App_Dominio.Security;

namespace DWM.Models.Persistence
{
    public class CobrancaClienteModel : CrudModel<CobrancaCliente, CobrancaClienteViewModel, ApplicationContext>
    {
        #region Constructor
        public CobrancaClienteModel() { }

        public CobrancaClienteModel(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe CrudModel
        public override CobrancaCliente MapToEntity(CobrancaClienteViewModel value)
        {
            CobrancaCliente c = Find(value);

            if (c == null)
            {
                c = new CobrancaCliente();
                c.cobrancaId = value.cobrancaId;
                c.clienteId = value.clienteId;
            }
            c.dia_vencimento = value.dia_vencimento;
            c.mes_dia = value.mes_dia;
            c.valor = value.valor;
            c.dt_desativacao = value.dt_desativacao;
            
            return c;
        }

        public override CobrancaClienteViewModel MapToRepository(CobrancaCliente entity)
        {
            CobrancaClienteViewModel CobrancaClienteViewModel = new CobrancaClienteViewModel()
            {
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS },
                cobrancaId = entity.cobrancaId,
                clienteId = entity.clienteId,
                nome_cliente = entity.clienteId > 0 ? db.Clientes.Find(entity.clienteId).nome : "",
                dia_vencimento = entity.dia_vencimento,
                mes_dia = entity.mes_dia,
                valor = entity.valor,
                dt_desativacao = entity.dt_desativacao
            };

            return CobrancaClienteViewModel;
        }

        public override CobrancaCliente Find(CobrancaClienteViewModel key)
        {
            CobrancaCliente entity = db.CobrancaClientes.Find(key.cobrancaId, key.clienteId);
            return entity;
        }

        public override Validate Validate(CobrancaClienteViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };

            if (operation != Crud.INCLUIR && value.cobrancaId <= 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Cobrança ID").ToString();
                value.mensagem.MessageBase = "Campo obrigatório: Cobrança ID";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.clienteId <= 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.MessageBase = MensagemPadrao.Message(5, "Cliente").ToString();
                value.mensagem.Message = "Campo obrigatório: Cliente";
                value.mensagem.MessageType = MsgType.WARNING;

                return value.mensagem;
            }

            if (operation == Crud.INCLUIR)
            {
                CobrancaCliente c = Find(value);
                if (c != null)
                {
                    value.mensagem.Code = 19;
                    value.mensagem.Message = MensagemPadrao.Message(19).ToString();
                    value.mensagem.MessageBase = "Cliente já cadastrado na lista de cobrança";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
            }

            if (operation != Crud.EXCLUIR)
            {
                if (value.dia_vencimento < 0 || value.dia_vencimento > 31)
                {
                    value.mensagem.Code = 48;
                    value.mensagem.Message = MensagemPadrao.Message(48, "Dia Vencimento", "01", "31").ToString();
                    value.mensagem.MessageBase = "Período de vencimento inválido";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.valor < 0)
                {
                    value.mensagem.Code = 4;
                    value.mensagem.Message = MensagemPadrao.Message(4, "Valor", value.valor.Value.ToString("###,###,##0.00")).ToString();
                    value.mensagem.MessageBase = "Campo obrigatório: Valor";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
            }

            return value.mensagem;
        }
        #endregion
    }
}