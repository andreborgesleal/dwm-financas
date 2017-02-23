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
    public class HistoricoModel : CrudContext<Historico, HistoricoViewModel, ApplicationContext>
    {
        #region Métodos da classe CrudContext
        public override Historico MapToEntity(HistoricoViewModel value)
        {
            return new Historico()
            {
                historicoId = value.historicoId,
                descricao = value.descricao != null ? value.descricao.Replace("&","e") : null,
                empresaId = value.empresaId,
                ind_tipoHistorico = value.ind_tipoHistorico
            };
        }

        public override HistoricoViewModel MapToRepository(Historico entity)
        {
            return new HistoricoViewModel()
            {
                historicoId = entity.historicoId,
                descricao = entity.descricao,
                ind_tipoHistorico = entity.ind_tipoHistorico,
                empresaId = entity.empresaId,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Historico Find(HistoricoViewModel key)
        {
            return db.Historicos.Find(key.historicoId);
        }

        public override Validate Validate(HistoricoViewModel value, Crud operation)
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
                value.mensagem.Message = MensagemPadrao.Message(5, "Descricao do histórico").ToString();
                value.mensagem.MessageBase = "Descrição do histórico deve ser preenchida";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.ind_tipoHistorico == null || !"CPR".Contains(value.ind_tipoHistorico.Trim().Substring(0,1)))
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Tipo do histórico").ToString();
                value.mensagem.MessageBase = "Tipo do histórico deve ser preenchida";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (operation == Crud.INCLUIR || operation == Crud.ALTERAR)
            {
                int nomeHistorico = (from c in db.Historicos
                                     where c.empresaId.Equals(value.empresaId)
                                           && c.historicoId != value.historicoId
                                           && c.descricao.Equals(value.descricao)
                                     select c.descricao).Count();
                if (nomeHistorico > 0)
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

    public class ListViewHistorico : ListViewRepository<HistoricoViewModel, ApplicationContext>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<HistoricoViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _descricao = param != null && param.Count() > 0 && param[0] != null ? param[0].ToString() : null;
            string _ind_tipoHistorico = null;
            if (param != null && param.Count() > 1)
                _ind_tipoHistorico = param[1].ToString();

            return (from c in db.Historicos
                    where c.empresaId.Equals(sessaoCorrente.empresaId) &&
                          (_descricao == null || String.IsNullOrEmpty(_descricao) || c.descricao.Contains(_descricao.Trim())) &&
                          (_ind_tipoHistorico == null || c.ind_tipoHistorico == _ind_tipoHistorico)
                    orderby c.descricao
                    select new HistoricoViewModel
                    {
                        empresaId = sessaoCorrente.empresaId,
                        historicoId = c.historicoId,
                        descricao = c.descricao,
                        ind_tipoHistorico = c.ind_tipoHistorico,
                        PageSize = pageSize,
                        TotalCount = (from c1 in db.Historicos
                                      where c1.empresaId.Equals(sessaoCorrente.empresaId) &&
                                            (_descricao == null || String.IsNullOrEmpty(_descricao) || c1.descricao.Contains(_descricao.Trim())) &&
                                            (_ind_tipoHistorico == null || c1.ind_tipoHistorico == _ind_tipoHistorico)
                                      select c1).Count()
                    }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new HistoricoModel().getObject((HistoricoViewModel)id);
        }
        #endregion
    }

    #region Formulário Modal
    public class LookupHistoricoContabilidadeModel : ListViewHistorico
    {
        public override string action()
        {
            return "../Historicos/ListHistoricoContabilidadeModal";
        }

        public override string DivId()
        {
            return "div-his";
        }
    }

    public class LookupHistoricoContaReceberModel : ListViewHistorico
    {
        public override string action()
        {
            return "../Historicos/ListHistoricoContaReceberModal";
        }

        public override string DivId()
        {
            return "div-his";
        }
    }

    public class LookupHistoricoContaPagarModel : ListViewHistorico
    {
        public override string action()
        {
            return "../Historicos/ListHistoricoContaPagarModal";
        }

        public override string DivId()
        {
            return "div-his";
        }
    }
    #endregion

    #region Filtros
    public class LookupHistoricoContabilidadeFiltroModel : ListViewHistorico
    {
        public override string action()
        {
            return "../Historicos/_ListHistoricoContabilidadeModal";
        }

        public override string DivId()
        {
            return "div-his";
        }
    }

    public class LookupHistoricoContaPagarFiltroModel : ListViewHistorico
    {
        public override string action()
        {
            return "../Historicos/_ListHistoricoContaPagarModal";
        }

        public override string DivId()
        {
            return "div-his";
        }
    }

    public class LookupHistoricoContaReceberFiltroModel : ListViewHistorico
    {
        public override string action()
        {
            return "../Historicos/_ListHistoricoContaReceberModal";
        }

        public override string DivId()
        {
            return "div-his";
        }
    }
    #endregion

}