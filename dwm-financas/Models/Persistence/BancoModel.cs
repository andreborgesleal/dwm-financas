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
    public class BancoModel : CrudContext<Banco, BancoViewModel, ApplicationContext>
    {
        #region Métodos da classe CrudContext
        public override Banco MapToEntity(BancoViewModel value)
        {
            Banco entity = Find(value);

            if (entity == null)
                entity = new Banco();

            entity.bancoId = value.bancoId;
            entity.empresaId = value.empresaId;
            entity.nome = value.nome;
            entity.classificacao = value.classificacao;
            entity.sigla = value.sigla;
            entity.numero = value.numero;

            return entity;

        }

        public override BancoViewModel MapToRepository(Banco entity)
        {
            return new BancoViewModel()
            {
                bancoId = entity.bancoId,
                empresaId = entity.empresaId,
                nome = entity.nome,
                classificacao = entity.classificacao,
                sigla = entity.sigla,
                numero = entity.numero,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Banco Find(BancoViewModel key)
        {
            return db.Bancos.Find(key.bancoId);
        }

        public override Validate Validate(BancoViewModel value, Crud operation)
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

            if (value.nome.Trim().Length == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Nome do banco").ToString();
                value.mensagem.MessageBase = "Nome do banco deve ser preenchida";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.classificacao == null || !"C/C|INV|POU".Contains(value.classificacao.Trim().Substring(0, 3)))
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Classificação").ToString();
                value.mensagem.MessageBase = "Classificação deve ser preenchida";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.sigla.Trim().Length == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Sigla do banco").ToString();
                value.mensagem.MessageBase = "Sigla do banco deve ser preenchida";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (operation == Crud.INCLUIR || operation == Crud.ALTERAR)
            {
                int nomeBanco = (from c in db.Bancos
                                     where c.empresaId.Equals(value.empresaId)
                                           && c.bancoId != value.bancoId
                                           && c.nome.Equals(value.nome)
                                     select c.nome).Count();
                if (nomeBanco > 0)
                {
                    value.mensagem.Code = 19;
                    value.mensagem.Message = MensagemPadrao.Message(19).ToString();
                    value.mensagem.MessageBase = "Nome do Banco já existente";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
            }
            return value.mensagem;
        }

        #endregion
    }

    public class ListViewBanco : ListViewRepository<BancoViewModel, ApplicationContext>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<BancoViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _descricao = param != null && param.Count() > 0 && param[0] != null ? param[0].ToString() : null;

            return (from b in db.Bancos
                    where b.empresaId.Equals(sessaoCorrente.empresaId) &&
                          (_descricao == null || String.IsNullOrEmpty(_descricao) || b.nome.Contains(_descricao.Trim())) 
                    orderby b.nome
                    select new BancoViewModel
                    {
                        empresaId = sessaoCorrente.empresaId,
                        bancoId = b.bancoId,
                        nome = b.nome,
                        sigla = b.sigla,
                        classificacao = b.classificacao,
                        numero = b.numero,
                        PageSize = pageSize,
                        TotalCount = (from b1 in db.Bancos
                                      where b1.empresaId.Equals(sessaoCorrente.empresaId) &&
                                            (_descricao == null || String.IsNullOrEmpty(_descricao) || b1.nome.Contains(_descricao.Trim())) 
                                      select b1).Count()
                    }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new BancoModel().getObject((BancoViewModel)id);
        }
        #endregion
    }

    #region Formulário Modal
    public class LookupBancoModel : ListViewBanco
    {
        public override string action()
        {
            return "../Bancos/ListBancoModal";
        }

        public override string DivId()
        {
            return "div-ban";
        }
    }

    public class LookupBancoFiltroModel : ListViewBanco
    {
        public override string action()
        {
            return "../Bancos/_ListBancoModal";
        }

        public override string DivId()
        {
            return "div-ban";
        }
    }

    public class LookupBanco2Model : ListViewBanco
    {
        public override string action()
        {
            return "../Bancos/ListBanco2Modal";
        }

        public override string DivId()
        {
            return "div-ban2";
        }
    }

    public class LookupBanco2FiltroModel : ListViewBanco
    {
        public override string action()
        {
            return "../Bancos/_ListBanco2Modal";
        }

        public override string DivId()
        {
            return "div-ban2";
        }
    }

    public class LookupBanco3Model : ListViewBanco
    {
        public override string action()
        {
            return "../Bancos/ListBanco3Modal";
        }

        public override string DivId()
        {
            return "div-ban3";
        }
    }

    public class LookupBanco3FiltroModel : ListViewBanco
    {
        public override string action()
        {
            return "../Bancos/_ListBanco3Modal";
        }

        public override string DivId()
        {
            return "div-ban3";
        }
    }

    #endregion
}