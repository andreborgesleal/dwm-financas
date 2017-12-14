using System;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Models;

namespace DWM.Models.Persistence
{
    public class RemessaModel : CrudModel<Remessa, RemessaViewModel, ApplicationContext>
    {
        #region Métodos da classe CrudContext
        public override RemessaViewModel BeforeInsert(RemessaViewModel value)
        {
            value.empresaId = value.empresaId == 0 ? sessaoCorrente.empresaId : value.empresaId;
            value.DataGeracao = null;
            return base.BeforeInsert(value);
        }

        public override Remessa MapToEntity(RemessaViewModel value)
        {
            Remessa remessa = Find(value);

            if (remessa == null)
                remessa = new Remessa();

            remessa.BancoID = value.BancoID;
            remessa.ConvenioID = value.ConvenioID;
            remessa.RemessaID = value.RemessaID;
            remessa.DataGeracao = value.DataGeracao;
            remessa.LayoutArquivo = value.LayoutArquivo;

            return remessa;
        }

        public override RemessaViewModel MapToRepository(Remessa entity)
        {
            return new RemessaViewModel()
            {
                BancoID = entity.BancoID,
                ConvenioID = entity.ConvenioID,
                RemessaID = entity.RemessaID,
                empresaId = sessaoCorrente.empresaId,
                DataGeracao = entity.DataGeracao,
                LayoutArquivo = entity.LayoutArquivo,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Remessa Find(RemessaViewModel key)
        {
            return db.Remessas.Find(key.BancoID, key.ConvenioID, key.RemessaID);
        }

        public override Validate Validate(RemessaViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };

            if (String.IsNullOrEmpty(value.BancoID))
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Nº Banco").ToString();
                value.mensagem.MessageBase = "Campo Nº Banco deve ser informado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            if (String.IsNullOrEmpty(value.ConvenioID))
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Convênio").ToString();
                value.mensagem.MessageBase = "Campo Convênio deve ser informado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            if (String.IsNullOrEmpty(value.RemessaID))
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Remessa").ToString();
                value.mensagem.MessageBase = "Campo Nº Remessa deve ser informado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (operation != Crud.EXCLUIR)
            {
                if (value.DataGeracao != null && value.DataGeracao < Funcoes.Brasilia().Date)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Data da Geração do arquivo").ToString();
                    value.mensagem.MessageBase = "Data da geração do arquivo não pode ser menor que a data atual";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
                if (String.IsNullOrEmpty(value.LayoutArquivo))
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Layout").ToString();
                    value.mensagem.MessageBase = "Campo Layout do arquivo deve ser informado";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
            }

            return value.mensagem;
        }
        #endregion

    }
}