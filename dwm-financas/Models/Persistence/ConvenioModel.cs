using System;
using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;

namespace DWM.Models.Persistence
{
    public class ConvenioModel : CrudModel<Convenio, ConvenioViewModel, ApplicationContext>
    {
        #region Métodos da classe CrudContext
        public override ConvenioViewModel BeforeInsert(ConvenioViewModel value)
        {
            value.empresaId = value.empresaId == 0 ? sessaoCorrente.empresaId : value.empresaId;
            return base.BeforeInsert(value);
        }

        public override Convenio MapToEntity(ConvenioViewModel value)
        {
            Convenio convenio = Find(value);

            if (convenio == null)
                convenio = new Convenio();

            convenio.BancoID = value.BancoID;
            convenio.ConvenioID = value.ConvenioID;
            convenio.empresaId = value.empresaId;
            convenio.NomeBanco = value.NomeBanco;
            convenio.AgenciaID = value.AgenciaID;
            convenio.AgenciaDV = value.AgenciaDV;
            convenio.ContaID = value.ContaID;
            convenio.ContaDV = value.ContaDV;
            convenio.CarteiraID = value.CarteiraID;
            convenio.Instrucao1 = value.Instrucao1;
            convenio.Instrucao2 = value.Instrucao2;
            convenio.LayoutArquivo = value.LayoutArquivo;
            convenio.NossoNumeroInicio = value.NossoNumeroInicio;
            convenio.NossoNumeroFim = value.NossoNumeroFim;
            convenio.NossoNumeroUltimo = value.NossoNumeroUltimo;

            return convenio;
        }

        public override ConvenioViewModel MapToRepository(Convenio entity)
        {
            return new ConvenioViewModel()
            {
                BancoID = entity.BancoID,
                ConvenioID = entity.ConvenioID,
                empresaId = entity.empresaId,
                NomeBanco = entity.NomeBanco,
                AgenciaID = entity.AgenciaID,
                AgenciaDV = entity.AgenciaDV,
                ContaID = entity.ContaID,
                ContaDV = entity.ContaDV,
                CarteiraID = entity.CarteiraID,
                Instrucao1 = entity.Instrucao1,
                Instrucao2 = entity.Instrucao2,
                LayoutArquivo = entity.LayoutArquivo,
                NossoNumeroInicio = entity.NossoNumeroInicio,
                NossoNumeroFim = entity.NossoNumeroFim,
                NossoNumeroUltimo = entity.NossoNumeroUltimo,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Convenio Find(ConvenioViewModel key)
        {
            return db.Convenios.Find(key.BancoID, key.ConvenioID);
        }

        public override Validate Validate(ConvenioViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };

            if (operation != Crud.INCLUIR)
            {
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
            }

            if (operation != Crud.EXCLUIR)
            {
                if (value.NomeBanco == null || value.NomeBanco.Trim().Length <= 2)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Nome Banco").ToString();
                    value.mensagem.MessageBase = "Campo Nome do Banco deve ser informado e deve possuir no mínimo 2 caracteres";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
                if (String.IsNullOrEmpty(value.AgenciaID))
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Agência").ToString();
                    value.mensagem.MessageBase = "Campo Agência deve ser informada";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
                if (String.IsNullOrEmpty(value.ContaID))
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Conta").ToString();
                    value.mensagem.MessageBase = "Campo Conta deve ser informada";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
            }

            return value.mensagem;
        }
        #endregion
    }

    public class ListViewModelConvenio : ListViewModel<ConvenioViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewModelConvenio() { }
        public ListViewModelConvenio(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe ListViewRepository
        public override IEnumerable<ConvenioViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _nome = param != null && param.Count() > 0 && param[0] != null ? param[0].ToString() : null;
            return (from conv in db.Convenios
                    where conv.empresaId == sessaoCorrente.empresaId &&
                          (_nome == null || String.IsNullOrEmpty(_nome) || conv.NomeBanco.Contains(_nome.Trim()) || conv.BancoID == _nome || conv.ConvenioID == _nome)
                    orderby conv.BancoID, conv.ConvenioID
                    select new ConvenioViewModel
                    {
                        BancoID = conv.BancoID,
                        ConvenioID = conv.ConvenioID,
                        empresaId = conv.empresaId,
                        NomeBanco = conv.NomeBanco,
                        AgenciaID = conv.AgenciaID,
                        AgenciaDV = conv.AgenciaDV,
                        ContaID = conv.ContaID,
                        ContaDV = conv.ContaDV,
                        CarteiraID = conv.CarteiraID,
                        Instrucao1 = conv.Instrucao1,
                        Instrucao2 = conv.Instrucao2,
                        LayoutArquivo = conv.LayoutArquivo,
                        NossoNumeroInicio = conv.NossoNumeroInicio,
                        NossoNumeroFim = conv.NossoNumeroFim,
                        NossoNumeroUltimo = conv.NossoNumeroUltimo,
                        PageSize = pageSize,
                        TotalCount = 0
                    }).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new ClienteModel().getObject((ClienteViewModel)id);
        }
        #endregion
    }

}