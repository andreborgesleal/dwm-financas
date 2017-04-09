using System;
using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using DWM.Models.Entidades;
using DWM.Models.Repositories;

namespace DWM.Models.Persistence
{
    public class GrupoClienteModel : CrudContext<GrupoCliente, GrupoClienteViewModel, ApplicationContext>
    {
        #region Métodos da classe CrudContext
        public override GrupoCliente MapToEntity(GrupoClienteViewModel value)
        {
            return new GrupoCliente()
            {
                grupoClienteId = value.grupoClienteId == 0 ? getId() : value.grupoClienteId,
                empresaId = value.empresaId,
                nome = value.nome
            };
        }

        public override GrupoClienteViewModel MapToRepository(GrupoCliente entity)
        {
            return new GrupoClienteViewModel()
            {
                grupoClienteId = entity.grupoClienteId,
                nome = entity.nome,
                empresaId = entity.empresaId,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override GrupoCliente Find(GrupoClienteViewModel key)
        {
            return db.GrupoClientes.Find(key.grupoClienteId);
        }

        public override Validate Validate(GrupoClienteViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };

            if (value.nome.Trim().Length == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Descrição").ToString();
                value.mensagem.MessageBase = "Campo Nome do Grupo deve ser informado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            else if (operation == Crud.INCLUIR)
            {
                // Verifica se o grupo já foi cadastrado com o mesmo nome
                if (db.GrupoClientes.Where(info => info.nome == value.nome && info.empresaId == value.empresaId).Count() > 0)
                {
                    value.mensagem.Code = 19;
                    value.mensagem.Message = MensagemPadrao.Message(19).ToString();
                    value.mensagem.MessageBase = "Campo Nome do Grupo já existe";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
            }
            else if (operation == Crud.ALTERAR)
                // Verifica se o grupo já foi cadastrado com o mesmo nome
                if (db.GrupoClientes.Where(info => info.nome == value.nome && info.empresaId == value.empresaId && info.grupoClienteId != value.grupoClienteId).Count() > 0)
                {
                    value.mensagem.Code = 19;
                    value.mensagem.Message = MensagemPadrao.Message(19).ToString();
                    value.mensagem.MessageBase = "Campo Nome do Grupo já existe";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
            return value.mensagem;
        }

        private int getId()
        {
            int value = 1;
            if (db.GrupoClientes.Count() > 0)
                value = db.GrupoClientes.Max(info => info.grupoClienteId) + 1;

            return value;
        }

        #endregion
    }

    public class ListViewGrupoCliente : ListViewRepository<GrupoClienteViewModel, ApplicationContext>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<GrupoClienteViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _nome = param != null && param.Count() > 0 && param[0] != null ? param[0].ToString() : null;
            return (from gru in db.GrupoClientes
                    where (gru.empresaId == sessaoCorrente.empresaId && (_nome == null || String.IsNullOrEmpty(_nome) || gru.nome.Contains(_nome.Trim())))
                    orderby gru.nome
                    select new GrupoClienteViewModel
                    {
                        grupoClienteId = gru.grupoClienteId,
                        empresaId = gru.empresaId,
                        nome = gru.nome,
                        PageSize = pageSize,
                        TotalCount = (from gru1 in db.GrupoClientes
                                      where (gru1.empresaId == sessaoCorrente.empresaId && (_nome == null || String.IsNullOrEmpty(_nome) || gru1.nome.Contains(_nome.Trim())))
                                      select gru1).Count()
                    }).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new GrupoClienteModel().getObject((GrupoClienteViewModel)id);
        }
        #endregion
    }

    public class LookupGrupoClienteModel : ListViewGrupoCliente
    {
        public override string action()
        {
            return "../GrupoClientes/ListGrupoClienteModal";
        }

        public override string DivId()
        {
            return "div-gcli";
        }
    }

    public class LookupGrupoClienteFiltroModel : ListViewGrupoCliente
    {
        public override string action()
        {
            return "../GrupoClientes/_ListGrupoClienteModal";
        }

        public override string DivId()
        {
            return "div-gcli";
        }
    }

}