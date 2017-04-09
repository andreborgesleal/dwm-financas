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
    public class CentroCustoModel : CrudContext<CentroCusto, CentroCustoViewModel, ApplicationContext>
    {
        #region Métodos da classe CrudContext
        public override CentroCusto MapToEntity(CentroCustoViewModel value)
        {
            return new CentroCusto()
            {
                centroCustoId = value.centroCustoId,
                descricao = value.descricao != null ? value.descricao.Replace("&","e") : null,
                empresaId = value.empresaId
            };
        }

        public override CentroCustoViewModel MapToRepository(CentroCusto entity)
        {
            return new CentroCustoViewModel()
            {
                centroCustoId = entity.centroCustoId,
                descricao = entity.descricao,
                empresaId = entity.empresaId,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override CentroCusto Find(CentroCustoViewModel key)
        {
            return db.CentroCustos.Find(key.centroCustoId);
        }

        public override Validate Validate(CentroCustoViewModel value, Crud operation)
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
                value.mensagem.Message = MensagemPadrao.Message(5, "Descrição do Centro de Custo").ToString();
                value.mensagem.MessageBase = "Descrição do centro de Custo deve ser preenchida";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (operation == Crud.INCLUIR || operation == Crud.ALTERAR)
            {
                int nomeCentroCusto = (from c in db.CentroCustos
                                  where c.empresaId.Equals(value.empresaId)
                                        && c.centroCustoId != value.centroCustoId
                                        && c.descricao.Equals(value.descricao)
                                  select c.descricao).Count();
                if (nomeCentroCusto > 0)
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

    public class ListViewCentroCusto : ListViewRepository<CentroCustoViewModel, ApplicationContext>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<CentroCustoViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _descricao = param != null && param.Count() > 0 && param[0] != null ? param[0].ToString() : null;
            return (from c in db.CentroCustos
                    where c.empresaId.Equals(sessaoCorrente.empresaId)
                          && (_descricao == null || String.IsNullOrEmpty(_descricao) || c.descricao.StartsWith(_descricao.Trim()))
                    orderby c.descricao
                    select new CentroCustoViewModel
                    {
                        empresaId = sessaoCorrente.empresaId,
                        centroCustoId = c.centroCustoId,
                        descricao = c.descricao,
                        PageSize = pageSize,
                        TotalCount = 0
                        //TotalCount = (from c1 in db.CentroCustos
                        //              where c1.empresaId.Equals(sessaoCorrente.empresaId)
                        //                    && (_descricao == null || String.IsNullOrEmpty(_descricao) || c1.descricao.StartsWith(_descricao.Trim()))
                        //              select c1).Count()
                    }).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new CentroCustoModel().getObject((CentroCustoViewModel)id);
        }
        #endregion
    }

    public class LookupCentroCustoModel : ListViewCentroCusto
    {
        public override string action()
        {
            return "../CentroCustos/ListCentroCustoModal";
        }

        public override string DivId()
        {
            return "div-ccu";
        }
    }

    public class LookupCentroCustoFiltroModel : ListViewCentroCusto
    {
        public override string action()
        {
            return "../CentroCustos/_ListCentroCustoModal";
        }

        public override string DivId()
        {
            return "div-ccu";
        }
    }
}