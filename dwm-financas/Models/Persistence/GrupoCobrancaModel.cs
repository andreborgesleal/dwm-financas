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
    public class GrupoCobrancaModel : CrudModel<GrupoCobranca, GrupoCobrancaViewModel, ApplicationContext>
    {
        #region Constructor
        public GrupoCobrancaModel() { }
        public GrupoCobrancaModel(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe CrudContext
        public override GrupoCobranca MapToEntity(GrupoCobrancaViewModel value)
        {
            GrupoCobranca entity = Find(value);

            if (entity == null)
                entity = new GrupoCobranca();

            entity.grupoCobrancaId = value.grupoCobrancaId;
            entity.empresaId = value.empresaId;
            entity.descricao = value.descricao;

            return entity;
        }

        public override GrupoCobrancaViewModel MapToRepository(GrupoCobranca entity)
        {
            return new GrupoCobrancaViewModel()
            {
                grupoCobrancaId = entity.grupoCobrancaId,
                empresaId = entity.empresaId,
                descricao = entity.descricao,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override GrupoCobranca Find(GrupoCobrancaViewModel key)
        {
            return db.GrupoCobrancas.Find(key.grupoCobrancaId);
        }

        public override Validate Validate(GrupoCobrancaViewModel value, Crud operation)
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
                value.mensagem.Message = MensagemPadrao.Message(5, "Descrição do Grupo de Cobrança").ToString();
                value.mensagem.MessageBase = "Descrição do Grupo de Cobrança deve ser informado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (operation == Crud.INCLUIR || operation == Crud.ALTERAR)
            {
                int descricaoGrupoCobranca = (from c in db.GrupoCobrancas
                                 where c.empresaId.Equals(value.empresaId)
                                       && c.grupoCobrancaId != value.grupoCobrancaId
                                       && c.descricao.Equals(value.descricao)
                                 select c.descricao).Count();
                if (descricaoGrupoCobranca > 0)
                {
                    value.mensagem.Code = 19;
                    value.mensagem.Message = MensagemPadrao.Message(19).ToString();
                    value.mensagem.MessageBase = "Descrição do Grupo de Cobrança já existente";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
            }
            return value.mensagem;
        }

        #endregion
    }

    public class ListViewGrupoCobranca : ListViewModel<GrupoCobrancaViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewGrupoCobranca() { }
        public ListViewGrupoCobranca(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe ListViewRepository
        public override IEnumerable<GrupoCobrancaViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _descricao = param != null && param.Count() > 0 && param[0] != null ? param[0].ToString() : null;

            return (from b in db.GrupoCobrancas
                    where b.empresaId.Equals(sessaoCorrente.empresaId) &&
                          (_descricao == null || String.IsNullOrEmpty(_descricao) || b.descricao.Contains(_descricao.Trim()))
                    orderby b.descricao
                    select new GrupoCobrancaViewModel
                    {
                        empresaId = sessaoCorrente.empresaId,
                        grupoCobrancaId = b.grupoCobrancaId,
                        descricao = b.descricao,
                        PageSize = pageSize,
                        TotalCount = (from b1 in db.GrupoCobrancas
                                      where b1.empresaId.Equals(sessaoCorrente.empresaId) &&
                                            (_descricao == null || String.IsNullOrEmpty(_descricao) || b1.descricao.Contains(_descricao.Trim()))
                                      select b1).Count()
                    }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new GrupoCobrancaModel().getObject((GrupoCobrancaViewModel)id);
        }
        #endregion
    }

}