using System;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Models;
using System.Linq;

namespace DWM.Models.Persistence
{
    public class TituloModel : CrudModel<Titulo, TituloViewModel, ApplicationContext>
    {
        #region Métodos da classe CrudContext
        public override TituloViewModel BeforeInsert(TituloViewModel value)
        {
            value.empresaId = value.empresaId == 0 ? sessaoCorrente.empresaId : value.empresaId;
            value.DataEmissao = Funcoes.Brasilia();

            #region Remessa
            if (db.Remessas.Where(info => info.BancoID == value.BancoID && info.ConvenioID == value.ConvenioID && !info.DataGeracao.HasValue).Count() > 0)
                value.RemessaID = db.Remessas.Where(info => info.BancoID == value.BancoID && info.ConvenioID == value.ConvenioID && !info.DataGeracao.HasValue).FirstOrDefault().RemessaID;
            else
            {
                int RemessaID = int.Parse(db.Remessas.Where(info => info.BancoID == value.BancoID && info.ConvenioID == value.ConvenioID).Select(m => m.RemessaID).Max()) + 1;
                value.RemessaID = RemessaID.ToString().PadLeft(6,'0');
            }
            #endregion

            value.OcorrenciaID = "01";

            #region NossoNumero
            if (db.Convenios.Find(value.BancoID, value.ConvenioID).NossoNumeroInicio != "")
            {
                int nn = 1;
                if (db.Convenios.Find(value.BancoID, value.ConvenioID).NossoNumeroUltimo != "")
                    nn = int.Parse(db.Convenios.Find(value.BancoID, value.ConvenioID).NossoNumeroUltimo) + 1;

            }

            #endregion

            return base.BeforeInsert(value);
        }

        public override Titulo MapToEntity(TituloViewModel value)
        {
            Titulo titulo = Find(value);

            if (titulo == null)
                titulo = new Titulo();

            titulo.BancoID = value.BancoID;
            titulo.ConvenioID = value.ConvenioID;
            titulo.TituloID = value.TituloID;
            //titulo.DataGeracao = value.DataGeracao;
            //titulo.LayoutArquivo = value.LayoutArquivo;

            return titulo;
        }

        public override TituloViewModel MapToRepository(Titulo entity)
        {
            return new TituloViewModel()
            {
                BancoID = entity.BancoID,
                ConvenioID = entity.ConvenioID,
                //TituloID = entity.TituloID,
                //empresaId = sessaoCorrente.empresaId,
                //DataGeracao = entity.DataGeracao,
                //LayoutArquivo = entity.LayoutArquivo,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Titulo Find(TituloViewModel key)
        {
            return db.Titulos.Find(key.BancoID, key.ConvenioID, key.TituloID);
        }

        public override Validate Validate(TituloViewModel value, Crud operation)
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
            if (String.IsNullOrEmpty(value.TituloID))
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Titulo").ToString();
                value.mensagem.MessageBase = "Campo Nº Titulo deve ser informado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (operation != Crud.EXCLUIR)
            {
                //if (value.DataGeracao != null && value.DataGeracao < Funcoes.Brasilia().Date)
                //{
                //    value.mensagem.Code = 5;
                //    value.mensagem.Message = MensagemPadrao.Message(5, "Data da Geração do arquivo").ToString();
                //    value.mensagem.MessageBase = "Data da geração do arquivo não pode ser menor que a data atual";
                //    value.mensagem.MessageType = MsgType.WARNING;
                //    return value.mensagem;
                //}
                //if (String.IsNullOrEmpty(value.LayoutArquivo))
                //{
                //    value.mensagem.Code = 5;
                //    value.mensagem.Message = MensagemPadrao.Message(5, "Layout").ToString();
                //    value.mensagem.MessageBase = "Campo Layout do arquivo deve ser informado";
                //    value.mensagem.MessageType = MsgType.WARNING;
                //    return value.mensagem;
                //}
            }

            return value.mensagem;
        }
        #endregion
    }
}