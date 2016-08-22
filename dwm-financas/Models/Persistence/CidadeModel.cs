using System;
using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using App_Dominio.Enumeracoes;
using App_Dominio.Models;
using DWM.Models.Entidades;
using DWM.Models.Repositories;
using DWM.Models.Entidades;

namespace DWM.Models.Persistence
{
    public class CidadeModel: CrudContext<Cidade, CidadeViewModel, ApplicationContext>
    {
        #region Métodos da classe CrudContext
        public override Cidade MapToEntity(CidadeViewModel value)
        {
            return new Cidade()
            {
                cidadeId = value.cidadeId,
                nome = value.nome
            };
        }

        public override CidadeViewModel MapToRepository(Cidade entity)
        {
            return new CidadeViewModel()
            {
                cidadeId = entity.cidadeId,
                nome = entity.nome,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Cidade Find(CidadeViewModel key)
        {
            return db.Cidades.Find(key.cidadeId);
        }

        public override Validate Validate(CidadeViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };

            if (value.nome.Trim().Length == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Descrição do Centro de Custo").ToString();
                value.mensagem.MessageBase = "Descrição do centro de Custo deve ser preenchida";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (operation == Crud.INCLUIR || operation == Crud.ALTERAR)
            {
                int nomeCidade = (from c in db.Cidades
                                       where c.cidadeId != value.cidadeId
                                             && c.nome.Equals(value.nome)
                                       select c.nome).Count();
                if (nomeCidade > 0)
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

    public class ListViewCidade : ListViewRepository<CidadeViewModel, ApplicationContext>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<CidadeViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _nome = param != null && param.Count() > 0 && param[0] != null ? param[0].ToString() : null;
            return (from c in db.Cidades
                    where (_nome == null || String.IsNullOrEmpty(_nome) || c.nome.StartsWith(_nome.Trim()))
                    orderby c.nome
                    select new CidadeViewModel
                    {
                        empresaId = sessaoCorrente.empresaId,
                        cidadeId = c.cidadeId,
                        nome = c.nome,
                        PageSize = pageSize,
                        TotalCount = (from c1 in db.Cidades
                                      where (_nome == null || String.IsNullOrEmpty(_nome) || c1.nome.StartsWith(_nome.Trim()))
                                      select c1).Count()
                    }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new CidadeModel().getObject((CidadeViewModel)id);
        }
        #endregion
    }
}