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
    public class ClienteModel : CrudContext<Cliente, ClienteViewModel, ApplicationContext>
    {
        #region Métodos da classe CrudContext
        public override Cliente MapToEntity(ClienteViewModel value)
        {
            Cliente cliente = Find(value);

            if (cliente == null)
                cliente = new Cliente();

            cliente.clienteId = value.clienteId;
            cliente.empresaId = value.empresaId;
            cliente.nome = value.nome.Replace("&", "e");
            cliente.grupoClienteId = value.grupoClienteId;
            cliente.ind_tipo_pessoa = value.ind_tipo_pessoa.Substring(1, 1);
            cliente.cpf_cnpj = value.cpf_cnpj != null ? value.cpf_cnpj.Replace(".", "").Replace("-", "").Replace("/", "") : null;
            cliente.dt_inclusao = value.dt_inclusao;
            cliente.dt_alteracao = Funcoes.Brasilia();
            cliente.codigo = value.codigo;
            cliente.endereco = value.endereco != null ? value.endereco.Replace("&", "e") : null;
            cliente.complemento = value.complemento != null ? value.complemento.Replace("&", "e") : null;
            cliente.cidade = value.cidade != null ? value.cidade.Replace("&", "e") : null;
            cliente.uf = value.uf;
            cliente.cep = value.cep != null ? value.cep.Replace(".", "").Replace("-", "") : null;
            cliente.bairro = value.bairro != null ? value.bairro.Replace("&", "e") : null;
            cliente.fone1 = value.fone1 != null ? value.fone1.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : null;
            cliente.fone2 = value.fone2 != null ? value.fone2.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : null;
            cliente.fone3 = value.fone3 != null ? value.fone3.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : null;
            cliente.email = value.email != null ? value.email.ToLower() : null;
            cliente.sexo = value.sexo;
            cliente.dt_nascimento = value.dt_nascimento;
            cliente.observacao = value.observacao != null ? value.observacao.Replace("&", "e") : null;

            return cliente;
        }

        public override ClienteViewModel MapToRepository(Cliente entity)
        {
            return new ClienteViewModel()
            {
                clienteId = entity.clienteId,
                nome = entity.nome,
                empresaId = entity.empresaId,
                grupoClienteId = entity.grupoClienteId,
                ind_tipo_pessoa = "P" + entity.ind_tipo_pessoa,
                cpf_cnpj = entity.cpf_cnpj,
                dt_inclusao = entity.dt_inclusao,
                dt_alteracao = entity.dt_alteracao,
                codigo = entity.codigo,
                endereco = entity.endereco,
                complemento = entity.complemento,
                cidade = entity.cidade,
                uf = entity.uf,
                cep = entity.cep,
                bairro = entity.bairro,
                fone1 = entity.fone1,
                fone2 = entity.fone2,
                fone3 = entity.fone3,
                email = entity.email,
                sexo = entity.sexo,
                dt_nascimento = entity.dt_nascimento,
                observacao = entity.observacao,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Cliente Find(ClienteViewModel key)
        {
            return db.Clientes.Find(key.clienteId);
        }

        public override Validate Validate(ClienteViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };

            if (value.nome.Trim().Length == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Nome").ToString();
                value.mensagem.MessageBase = "Campo Nome do Cliente deve ser informado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            else if (operation == Crud.INCLUIR)
            {
                // Verifica se o cliente já foi cadastrado com o mesmo nome
                if (db.Clientes.Where(info => info.nome == value.nome && info.empresaId == value.empresaId).Count() > 0)
                {
                    value.mensagem.Code = 19;
                    value.mensagem.Message = MensagemPadrao.Message(19).ToString();
                    value.mensagem.MessageBase = "Campo Nome do Cliente já existe";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
            }
            else if (operation == Crud.ALTERAR)
                // Verifica se o cliente já foi cadastrado com o mesmo nome
                if (db.Clientes.Where(info => info.nome == value.nome && info.empresaId == value.empresaId && info.clienteId != value.clienteId).Count() > 0)
                {
                    value.mensagem.Code = 19;
                    value.mensagem.Message = MensagemPadrao.Message(19).ToString();
                    value.mensagem.MessageBase = "Campo Nome do Cliente já existe";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

            #region Valida CPF/CNPJ
            if (value.cpf_cnpj != null)
            {
                // CPF
                if (value.cpf_cnpj.Replace(".", "").Replace("-", "").Replace("/", "").Length == 11)
                {
                    if (!Funcoes.ValidaCpf(value.cpf_cnpj.Replace(".", "").Replace("-", "").Replace("/", "")))
                    {
                        value.mensagem.Code = 29;
                        value.mensagem.Message = MensagemPadrao.Message(29).ToString();
                        value.mensagem.MessageBase = "Número de CPF incorreto.";
                        return value.mensagem;
                    }
                } // CNPJ
                else if (!Funcoes.ValidaCnpj(value.cpf_cnpj.Replace(".", "").Replace("-", "").Replace("/", "")))
                {
                    value.mensagem.Code = 30;
                    value.mensagem.Message = MensagemPadrao.Message(30).ToString();
                    value.mensagem.MessageBase = "Número de CNPJ incorreto.";
                    return value.mensagem;
                }
                if (operation == Crud.ALTERAR)
                {
                    if (db.Clientes.Where(info => info.cpf_cnpj == value.cpf_cnpj.Replace(".", "").Replace("-", "").Replace("/", "") && info.clienteId != value.clienteId && info.empresaId == sessaoCorrente.empresaId).Count() > 0)
                    {
                        value.mensagem.Code = 31;
                        value.mensagem.Message = MensagemPadrao.Message(31).ToString();
                        value.mensagem.MessageBase = "CPF/CNPJ informado para o fornecedor já se encontra cadastrado para outro fornecedor.";
                        return value.mensagem;
                    }
                }
                else
                {
                    if (db.Clientes.Where(info => info.cpf_cnpj == value.cpf_cnpj.Replace(".", "").Replace("-", "").Replace("/", "") && info.empresaId == sessaoCorrente.empresaId).Count() > 0)
                    {
                        value.mensagem.Code = 31;
                        value.mensagem.Message = MensagemPadrao.Message(31).ToString();
                        value.mensagem.MessageBase = "CPF/CNPJ informado para o fornecedor já se encontra cadastrado para outro fornecedor.";
                        return value.mensagem;
                    }
                }
            }
            #endregion

            return value.mensagem;
        }

        public override ClienteViewModel CreateRepository(System.Web.HttpRequestBase Request = null)
        {

            ClienteViewModel c = base.CreateRepository(Request);
            c.dt_inclusao = Funcoes.Brasilia();
            c.ind_tipo_pessoa = "PF";
            return c;
        }
        #endregion
    }

    public class ListViewModelCliente : ListViewModel<ClienteViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewModelCliente() { }
        public ListViewModelCliente(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe ListViewRepository
        public override IEnumerable<ClienteViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _nome = param != null && param.Count() > 0 && param[0] != null ? param[0].ToString() : null;
            return (from clnt in db.Clientes
                    join gru in db.GrupoClientes on clnt.grupoClienteId equals gru.grupoClienteId into GRU
                    from gru in GRU.DefaultIfEmpty()
                    where clnt.empresaId == sessaoCorrente.empresaId &&
                          (_nome == null || String.IsNullOrEmpty(_nome) || clnt.nome.Contains(_nome.Trim()) || clnt.cpf_cnpj == _nome || (gru != null && gru.nome.Contains(_nome.Trim())))
                    orderby clnt.nome
                    select new ClienteViewModel
                    {
                        clienteId = clnt.clienteId,
                        empresaId = clnt.empresaId,
                        cpf_cnpj = clnt.cpf_cnpj,
                        nome = clnt.nome,
                        nome_grupo = gru.nome,
                        fone1 = clnt.fone1,
                        fone2 = clnt.fone2,
                        email = clnt.email,
                        endereco = clnt.endereco,
                        PageSize = pageSize,
                        TotalCount = (from clnt1 in db.Clientes
                                      join gru1 in db.GrupoClientes on clnt1.grupoClienteId equals gru1.grupoClienteId into GRU1
                                      from gru1 in GRU1.DefaultIfEmpty()
                                      where clnt1.empresaId == sessaoCorrente.empresaId &&
                                            (_nome == null || String.IsNullOrEmpty(_nome) || clnt1.nome.Contains(_nome.Trim()) || clnt1.cpf_cnpj == _nome || (gru1 != null && gru1.nome.Contains(_nome.Trim())))
                                      select clnt1.clienteId).Count()
                    }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new ClienteModel().getObject((ClienteViewModel)id);
        }
        #endregion
    }

    public class ListViewCliente : ListViewRepository<ClienteViewModel, ApplicationContext>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<ClienteViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _nome = param != null && param.Count() > 0 && param[0] != null ? param[0].ToString() : null;
            return (from clnt in db.Clientes
                    join gru in db.GrupoClientes on clnt.grupoClienteId equals gru.grupoClienteId into GRU
                    from gru in GRU.DefaultIfEmpty()
                    where clnt.empresaId == sessaoCorrente.empresaId &&
                          (_nome == null || String.IsNullOrEmpty(_nome) || clnt.nome.Contains(_nome.Trim()) || clnt.cpf_cnpj == _nome || (gru != null && gru.nome.Contains(_nome.Trim())))
                    orderby clnt.nome
                    select new ClienteViewModel
                    {
                        clienteId = clnt.clienteId,
                        empresaId = clnt.empresaId,
                        cpf_cnpj = clnt.cpf_cnpj,
                        nome = clnt.nome,
                        nome_grupo = gru.nome,
                        fone1 = clnt.fone1,
                        fone2 = clnt.fone2,
                        email = clnt.email,
                        endereco = clnt.endereco,
                        complemento = clnt.complemento,
                        PageSize = pageSize,
                        TotalCount = (from clnt1 in db.Clientes
                                      join gru1 in db.GrupoClientes on clnt1.grupoClienteId equals gru1.grupoClienteId into GRU1
                                      from gru1 in GRU1.DefaultIfEmpty()
                                      where clnt1.empresaId == sessaoCorrente.empresaId &&
                                            (_nome == null || String.IsNullOrEmpty(_nome) || clnt1.nome.Contains(_nome.Trim()) || clnt1.cpf_cnpj == _nome || (gru1 != null && gru1.nome.Contains(_nome.Trim())))
                                      select clnt1.clienteId).Count()
                    }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new ClienteModel().getObject((ClienteViewModel)id);
        }
        #endregion
    }

    public class LookupClienteModel : ListViewCliente
    {
        public override string action()
        {
            return "../Clientes/ListClienteModal";
        }

        public override string DivId()
        {
            return "div-cli";
        }
    }

    public class LookupClienteFiltroModel : ListViewCliente
    {
        public override string action()
        {
            return "../Clientes/_ListClienteModal";
        }

        public override string DivId()
        {
            return "div-cli";
        }
    }

}