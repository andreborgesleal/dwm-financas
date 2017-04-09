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

namespace DWM.Models.Persistence
{
    public class EventoModel : CrudContext<Evento, EventoViewModel, ApplicationContext>
    {
        #region Métodos da classe CrudContext
        public override Evento MapToEntity(EventoViewModel value)
        {
            Evento entity = Find(value);

            if (entity == null)
            {
                entity = new Evento();
                entity.ind_eventoFixo = "N";
            }
            else
                entity.ind_eventoFixo = value.ind_eventoFixo;

            entity.eventoId = value.eventoId;
            entity.descricao = value.descricao;
            entity.empresaId = value.empresaId;
            entity.ind_tipoEvento = value.ind_tipoEvento;
            entity.ind_operacao = value.ind_operacao;
            entity.ind_modalidade = value.ind_modalidade;

            return entity;
        }

        public override EventoViewModel MapToRepository(Evento entity)
        {
            return new EventoViewModel()
            {
                eventoId = entity.eventoId,
                descricao = entity.descricao,
                codigo = entity.codigo,
                ind_tipoEvento = entity.ind_tipoEvento,
                empresaId = entity.empresaId,
                ind_eventoFixo = entity.ind_eventoFixo,
                ind_operacao = entity.ind_operacao,
                ind_modalidade = entity.ind_modalidade,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Evento Find(EventoViewModel key)
        {
            return db.Eventos.Find(key.eventoId);
        }

        public override Validate Validate(EventoViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };

            if (value.empresaId == 0)
            {
                value.mensagem.Code = 35;
                value.mensagem.Message = MensagemPadrao.Message(35).ToString();
                value.mensagem.MessageBase = "Sua sessão expirou. Faça um novo login no sistema";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.descricao.Trim().Length == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Descricao do evento").ToString();
                value.mensagem.MessageBase = "Descrição do evento deve ser informada";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.ind_tipoEvento == null || !"0|1|2|3|4|5|6|7|8|9".Contains(value.ind_tipoEvento.Trim().Substring(0, 1)))
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Tipo do Evento").ToString();
                value.mensagem.MessageBase = "Tipo do Evento deve ser informado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.ind_operacao == null || !"D|C".Contains(value.ind_operacao.Trim().Substring(0, 1)))
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Operação").ToString();
                value.mensagem.MessageBase = "Operação deve ser informada";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.ind_modalidade == null || !"P|R|C".Contains(value.ind_modalidade.Trim().Substring(0, 1)))
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Modalidade").ToString();
                value.mensagem.MessageBase = "Modalidade deve ser informada";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (operation == Crud.INCLUIR || operation == Crud.ALTERAR)
            {
                int nomeEvento = (from c in db.Eventos
                                     where c.empresaId.Equals(value.empresaId)
                                           && c.eventoId != value.eventoId
                                           && c.descricao.Equals(value.descricao)
                                     select c.descricao).Count();
                if (nomeEvento > 0)
                {
                    value.mensagem.Code = 19;
                    value.mensagem.Message = MensagemPadrao.Message(19).ToString();
                    value.mensagem.MessageBase = "Descrição já existente";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
            }
            return value.mensagem;
        }

        #endregion
    }

    public class ListViewEvento : ListViewRepository<EventoViewModel, ApplicationContext>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<EventoViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _descricao = param != null && param.Count() > 0 && param[0] != null ? param[0].ToString() : null;

            return (from e in db.Eventos
                    where e.empresaId.Equals(sessaoCorrente.empresaId) &&
                          (_descricao == null || String.IsNullOrEmpty(_descricao) || e.descricao.Contains(_descricao.Trim()))
                    orderby e.descricao
                    select new EventoViewModel
                    {
                        empresaId = sessaoCorrente.empresaId,
                        eventoId = e.eventoId,
                        descricao = e.descricao,
                        codigo = e.codigo,
                        ind_tipoEvento = e.ind_tipoEvento,
                        ind_eventoFixo = e.ind_eventoFixo,
                        ind_operacao = e.ind_operacao,
                        ind_modalidade = e.ind_modalidade,
                        PageSize = pageSize,
                        TotalCount = (from e1 in db.Eventos
                                      where e1.empresaId.Equals(sessaoCorrente.empresaId) &&
                                            (_descricao == null || String.IsNullOrEmpty(_descricao) || e1.descricao.Contains(_descricao.Trim()))
                                      select e1).Count()
                    }).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new EventoModel().getObject((EventoViewModel)id);
        }
        #endregion
    }

    #region Formulário Modal
    public class LookupEventoModel : ListViewEvento
    {
        public override string action()
        {
            return "../Eventos/ListEventoModal";
        }

        public override string DivId()
        {
            return "div-eve";
        }
    }

    public class LookupEventoFiltroModel : ListViewEvento
    {
        public override string action()
        {
            return "../Eventos/_ListEventoModal";
        }

        public override string DivId()
        {
            return "div-eve";
        }
    }

    #endregion

}