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
            value.empresaId = sessaoCorrente.empresaId;
            value.DataEmissao = Funcoes.Brasilia();
            value.IndAtivo = 1; // sim

            #region SequenciaID
            int sequencial = 0;
            if (db.Titulos.Where(info => info.operacaoId == value.operacaoId && info.parcelaId == value.parcelaId).Count() > 0)
            {
                sequencial = db.Titulos.Where(info => info.operacaoId == value.operacaoId && info.parcelaId == value.parcelaId).Select(m => m.SequenciaID).Max() + 1;
            }
            else
                value.OcorrenciaID = "01";

            value.SequenciaID = sequencial;
            #endregion

            #region NossoNumero
            if (db.Convenios.Find(value.BancoID, value.ConvenioID).NossoNumeroInicio.Trim() != "")
            {
                int nn = 1;
                if (db.Convenios.Find(value.BancoID, value.ConvenioID).NossoNumeroUltimo.Trim() != "")
                    nn = int.Parse(db.Convenios.Find(value.BancoID, value.ConvenioID).NossoNumeroUltimo) + 1;
                value.NossoNumero = nn.ToString().PadLeft(8, '0');
                value.NossoNumeroDV = Funcoes.DigitoM11(nn).ToString();
            }
            #endregion

            return base.BeforeInsert(value);
        }

        public override Titulo MapToEntity(TituloViewModel value)
        {
            Titulo titulo = Find(value);

            if (titulo == null)
                titulo = new Titulo();

            titulo.operacaoId = value.operacaoId;
            titulo.parcelaId = value.parcelaId;
            titulo.SequenciaID = value.SequenciaID;
            titulo.BancoID = value.BancoID;
            titulo.ConvenioID = value.ConvenioID;
            titulo.TituloID = value.TituloID;
            titulo.OcorrenciaID = value.OcorrenciaID;
            titulo.NossoNumero = value.NossoNumero;
            titulo.NossoNumeroDV = value.NossoNumeroDV;
            titulo.SeuNumero = value.SeuNumero;
            titulo.DataVencimento = value.DataVencimento;
            titulo.ValorPrincipal = value.ValorPrincipal;
            titulo.Especie = value.Especie;
            titulo.Aceite = value.Aceite;
            titulo.DataEmissao = value.DataEmissao;
            titulo.DataJuros = value.DataJuros;
            titulo.DataDesconto1 = value.DataDesconto1;
            titulo.ValorDesconto1 = value.ValorDesconto1;
            titulo.DataDesconto2 = value.DataDesconto2;
            titulo.ValorDesconto2 = value.ValorDesconto2;
            titulo.DataDesconto3 = value.DataDesconto3;
            titulo.ValorDesconto3 = value.ValorDesconto3;
            titulo.ValorIOF = value.ValorIOF;
            titulo.ValorAbatimento = value.ValorAbatimento;
            titulo.NumDiasDevolucao = value.NumDiasDevolucao;
            titulo.MultaID = value.MultaID;
            titulo.DataMulta = value.DataMulta;
            titulo.ValorMulta = value.ValorMulta;
            titulo.InstrucaoRodape = value.InstrucaoRodape;
            titulo.InstrucaoPagamento1 = value.InstrucaoPagamento1;
            titulo.InstrucaoPagamento2 = value.InstrucaoPagamento2;
            titulo.InstrucaoPagamento3 = value.InstrucaoPagamento3;
            titulo.InstrucaoPagamento4 = value.InstrucaoPagamento4;
            titulo.IndAtivo = value.IndAtivo;

            return titulo;
        }

        public override TituloViewModel MapToRepository(Titulo entity)
        {
            return new TituloViewModel()
            {
                operacaoId = entity.operacaoId,
                parcelaId = entity.parcelaId,
                SequenciaID = entity.SequenciaID,
                BancoID = entity.BancoID,
                ConvenioID = entity.ConvenioID,
                OcorrenciaID = entity.OcorrenciaID,
                NossoNumero = entity.NossoNumero,
                NossoNumeroDV = entity.NossoNumeroDV,
                SeuNumero = entity.SeuNumero,
                DataVencimento = entity.DataVencimento,
                ValorPrincipal = entity.ValorPrincipal,
                Especie = entity.Especie,
                Aceite = entity.Aceite,
                DataEmissao = entity.DataEmissao,
                DataJuros = entity.DataJuros,
                DataDesconto1 = entity.DataDesconto1,
                ValorDesconto1 = entity.ValorDesconto1,
                DataDesconto2 = entity.DataDesconto2,
                ValorDesconto2 = entity.ValorDesconto2,
                DataDesconto3 = entity.DataDesconto3,
                ValorDesconto3 = entity.ValorDesconto3,
                ValorIOF = entity.ValorIOF,
                ValorAbatimento = entity.ValorAbatimento,
                NumDiasDevolucao = entity.NumDiasDevolucao,
                MultaID = entity.MultaID,
                DataMulta = entity.DataMulta,
                ValorMulta = entity.ValorMulta,
                InstrucaoRodape = entity.InstrucaoRodape,
                InstrucaoPagamento1 = entity.InstrucaoPagamento1,
                InstrucaoPagamento2 = entity.InstrucaoPagamento2,
                InstrucaoPagamento3 = entity.InstrucaoPagamento3,
                InstrucaoPagamento4 = entity.InstrucaoPagamento4,
                IndAtivo = entity.IndAtivo,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Titulo Find(TituloViewModel key)
        {
            return db.Titulos.Find(key.operacaoId, key.parcelaId, key.SequenciaID);
        }

        public override Validate Validate(TituloViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };
            if (value.operacaoId == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Identificador da Operação (Contas a Receber)").ToString();
                value.mensagem.MessageBase = "Campo Identificador da Operação (Contas a Receber) deve ser informado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.parcelaId == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Nº Parcela").ToString();
                value.mensagem.MessageBase = "Campo Nº Parcela deve ser informado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.SequenciaID < 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Sequencial").ToString();
                value.mensagem.MessageBase = "Campo Sequencial de inclusão deve ser informado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

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
            if (value.clienteId==0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Pagador").ToString();
                value.mensagem.MessageBase = "Campo Pagador deve ser informado";
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
            if (String.IsNullOrEmpty(value.OcorrenciaID))
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Ocorrência").ToString();
                value.mensagem.MessageBase = "Campo Ocorrência deve ser informado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            if (value.DataVencimento < Funcoes.Brasilia().Date)
            {
                value.mensagem.Code = 8;
                value.mensagem.Message = MensagemPadrao.Message(8, "Data de Vencimento").ToString();
                value.mensagem.MessageBase = "Campo Data de Vencimento deve ser maior ou igual à data atual";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            if (value.ValorPrincipal < 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Valor Principal").ToString();
                value.mensagem.MessageBase = "Campo Valor Principal deve ser informado e ser maior que zero.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            if (String.IsNullOrEmpty(value.Especie) || value.Especie.Trim().Length == 1)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Espécie do Documento").ToString();
                value.mensagem.MessageBase = "Campo Espécie do documento deve ser informado.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            if (String.IsNullOrEmpty(value.Aceite) || !value.Aceite.Contains("A|N")) 
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Aceite").ToString();
                value.mensagem.MessageBase = "Campo Aceite deve ser informado.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            if (String.IsNullOrEmpty(value.MultaID) || !value.MultaID.Contains("0|1|2"))
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Tipo de Multa").ToString();
                value.mensagem.MessageBase = "Campo Tipos de Multa deve ser informado.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            if (value.MultaID != "0" && (!value.DataMulta.HasValue || value.DataMulta.Value < value.DataVencimento))
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Data da Multa").ToString();
                value.mensagem.MessageBase = "Campo Data da Multa deve ser informado e deve ser maior ou igual a data de vencimento.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            if (value.MultaID != "0" && (!value.ValorMulta.HasValue || value.ValorMulta.Value <= 0))
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Valor da Multa").ToString();
                value.mensagem.MessageBase = "Campo Valor da Multa deve ser informado.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            if (value.IndAtivo != 0 && value.IndAtivo != 1)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Título Ativo (S/N)").ToString();
                value.mensagem.MessageBase = "Campo Valor Título Ativo (S/N) deve ser informado.";
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