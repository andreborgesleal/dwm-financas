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
using App_Dominio.Security;

namespace DWM.Models.Persistence
{
    public class CobrancaModel : CrudModel<Cobranca, CobrancaViewModel, ApplicationContext>
    {
        #region Constructor
        public CobrancaModel() { }

        public CobrancaModel(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe CrudModel
        public override Cobranca MapToEntity(CobrancaViewModel value)
        {
            Cobranca c = Find(value);

            if (c == null)
                c = new Cobranca()
                {
                    CobrancaClientes = new List<CobrancaCliente>()
                };

            c.cobrancaId = value.cobrancaId;
            c.empresaId = value.empresaId;
            c.grupoCobrancaId = value.grupoCobrancaId;
            c.historicoId = value.historicoId;
            c.bancoId = value.bancoId;
            c.enquadramentoId = value.enquadramentoId;
            c.dt_inicio = value.dt_inicio;
            c.dt_fim = value.dt_fim;
            c.num_parcelas = value.num_parcelas;
            c.dia_vencimento = value.dia_vencimento;
            c.mes_dia = value.mes_dia;
            c.valor = value.valor;
            c.vr_jurosMora = value.vr_jurosMora;
            c.vr_multa = value.vr_multa;

            if (value.CobrancaClientes != null)
            {
                CobrancaClienteModel model = new CobrancaClienteModel();
                model.Create(this.db, this.seguranca_db);

                foreach (CobrancaClienteViewModel i in value.CobrancaClientes)
                {
                    CobrancaCliente x = model.MapToEntity(i);
                    c.CobrancaClientes.Add(x);
                }
            }
            return c;
        }

        public override CobrancaViewModel MapToRepository(Cobranca entity)
        {
            CobrancaViewModel c = new CobrancaViewModel()
            {
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS },
                cobrancaId = entity.cobrancaId,
                empresaId = entity.empresaId,
                grupoCobrancaId = entity.grupoCobrancaId,
                descricao_grupoCobranca = entity.grupoCobrancaId > 0 ? db.GrupoCobrancas.Find(entity.grupoCobrancaId).descricao : "",
                historicoId = entity.historicoId,
                descricao_historico = entity.historicoId > 0 ? db.Historicos.Find(entity.historicoId).descricao : "",
                bancoId = entity.bancoId,
                nome_banco = entity.bancoId > 0 ? db.Bancos.Find(entity.bancoId).nome : "",
                enquadramentoId = entity.enquadramentoId,
                descricao_enquadramento = entity.enquadramentoId > 0 ? db.Enquadramentos.Find(entity.enquadramentoId).descricao : "",
                dt_inicio = entity.dt_inicio,
                dt_fim = entity.dt_fim,
                num_parcelas = entity.num_parcelas,
                dia_vencimento = entity.dia_vencimento,
                mes_dia = entity.mes_dia,
                valor = entity.valor,
                vr_jurosMora = entity.vr_jurosMora,
                vr_multa = entity.vr_multa,
                CobrancaClienteViewModel = new CobrancaClienteViewModel(),
                CobrancaClientes = new List<CobrancaClienteViewModel>()
            };

            CobrancaClienteModel model = new CobrancaClienteModel();
            model.Create(this.db, this.seguranca_db);

            foreach (CobrancaCliente i in entity.CobrancaClientes)
            {
                CobrancaClienteViewModel CobrancaClienteViewModel = model.MapToRepository(i);
                ((List<CobrancaClienteViewModel>)c.CobrancaClientes).Add(CobrancaClienteViewModel);
            }

            return c;
        }

        public override Cobranca Find(CobrancaViewModel key)
        {
            Cobranca entity = db.Cobrancas.Find(key.cobrancaId);
            if (entity != null && (entity.CobrancaClientes.Count() == 0 || entity.empresaId != sessaoCorrente.empresaId))
                return null;

            return entity;
        }

        public override Validate Validate(CobrancaViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };

            if (value.empresaId == 0)
            {
                value.mensagem.Code = 35;
                value.mensagem.MessageBase = MensagemPadrao.Message(35).ToString();
                value.mensagem.Message = "Sua sessão expirou. Faça um novo login no sistema";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (operation != Crud.INCLUIR && value.cobrancaId <= 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Cobrança ID").ToString();
                value.mensagem.MessageBase = "Campo obrigatório: Cobrança ID";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (operation != Crud.EXCLUIR)
            {
                if (value.grupoCobrancaId <= 0)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.MessageBase = MensagemPadrao.Message(5, "Grupo de cobrança").ToString();
                    value.mensagem.Message = "Campo obrigatório: Grupo de cobrança";
                    value.mensagem.MessageType = MsgType.WARNING;

                    return value.mensagem;
                }

                if (value.historicoId <= 0)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.MessageBase = MensagemPadrao.Message(5, "Histórico").ToString();
                    value.mensagem.Message = "Campo obrigatório: Histórico";
                    value.mensagem.MessageType = MsgType.WARNING;

                    return value.mensagem;
                }

                if (value.bancoId <= 0)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Banco").ToString();
                    value.mensagem.MessageBase = "Campo obrigatório: Banco";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.enquadramentoId <= 0)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Enquadramento").ToString();
                    value.mensagem.MessageBase = "Campo obrigatório: Enquadramento";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.dt_inicio <= Convert.ToDateTime("1980-01-01"))
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Dt.Início").ToString();
                    value.mensagem.MessageBase = "Campo obrigatório: Dt. Início";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.dt_fim.HasValue && value.dt_fim <= value.dt_inicio)
                {
                    value.mensagem.Code = 11;
                    value.mensagem.Message = MensagemPadrao.Message(11, "Dt.Fim", "Dt.Início").ToString();
                    value.mensagem.MessageBase = "Data final inválida";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.num_parcelas <= 0)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Nº Parcelas").ToString();
                    value.mensagem.MessageBase = "Campo obrigatório: Nº Parcelas";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.dia_vencimento <= 0 || value.dia_vencimento > 31)
                {
                    value.mensagem.Code = 48;
                    value.mensagem.Message = MensagemPadrao.Message(48, "Dia Vencimento", "01", "31").ToString();
                    value.mensagem.MessageBase = "Período de vencimento inválido";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.valor <= 0)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Valor").ToString();
                    value.mensagem.MessageBase = "Campo obrigatório: Valor";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                // Validar clientes da cobrança
                if (operation == Crud.INCLUIR)
                {
                    CobrancaClienteModel model = new CobrancaClienteModel();

                    model.Create(this.db, this.seguranca_db);
                    foreach (CobrancaClienteViewModel cli in value.CobrancaClientes)
                    {
                        Validate validate = model.Validate(cli, operation);
                        if (validate.Code > 0)
                            return validate;
                    }
                }
            }

            return value.mensagem;
        }

        public override CobrancaViewModel CreateRepository(System.Web.HttpRequestBase Request = null)
        {
            CobrancaViewModel r = new CobrancaViewModel()
            {
                empresaId = sessaoCorrente.empresaId,
                dia_vencimento = 1,
                dt_inicio = Funcoes.Brasilia().Date,
                num_parcelas = 1,
                CobrancaClienteViewModel = new CobrancaClienteViewModel(),
                CobrancaClientes = new List<CobrancaClienteViewModel>()
            };

            return r;
        }
        #endregion
    }

    public class ListViewCobranca : ListViewModel<CobrancaViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewCobranca() { }
        public ListViewCobranca(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe ListViewRepository
        public override IEnumerable<CobrancaViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _descricao = param != null && param.Count() > 0 && param[0] != null ? param[0].ToString() : null;

            return (from cob in db.Cobrancas 
                    join gcob in db.GrupoCobrancas on cob.grupoCobrancaId equals gcob.grupoCobrancaId
                    join ban in db.Bancos on cob.bancoId equals ban.bancoId
                    join his in db.Historicos on cob.historicoId equals his.historicoId
                    join enq in db.Enquadramentos on cob.enquadramentoId equals enq.enquadramentoId
                    where cob.empresaId.Equals(sessaoCorrente.empresaId) &&
                          (_descricao == null || String.IsNullOrEmpty(_descricao) || gcob.descricao.Contains(_descricao.Trim()))
                    orderby gcob.descricao
                    select new CobrancaViewModel
                    {
                        cobrancaId = cob.cobrancaId,
                        grupoCobrancaId = cob.grupoCobrancaId,
                        descricao_grupoCobranca = gcob.descricao,
                        empresaId = sessaoCorrente.empresaId,
                        bancoId = cob.bancoId,
                        nome_banco = ban.sigla,
                        historicoId = cob.historicoId,
                        descricao_historico = his.descricao,
                        enquadramentoId = cob.enquadramentoId,
                        descricao_enquadramento = enq.descricao,
                        dt_inicio = cob.dt_inicio,
                        dt_fim = cob.dt_fim,
                        num_parcelas = cob.num_parcelas,
                        dia_vencimento = cob.dia_vencimento,
                        mes_dia = cob.mes_dia,
                        valor = cob.valor,
                        vr_jurosMora = cob.vr_jurosMora,
                        vr_multa = cob.vr_multa,
                        total_clientes = (from cobcli in db.CobrancaClientes where cobcli.cobrancaId == cob.cobrancaId select cobcli.cobrancaId).Count(),
                        PageSize = pageSize,
                        TotalCount = (from cob1 in db.Cobrancas
                                      join gcob1 in db.GrupoCobrancas on cob1.grupoCobrancaId equals gcob1.grupoCobrancaId
                                      join ban1 in db.Bancos on cob1.bancoId equals ban1.bancoId
                                      join his1 in db.Historicos on cob1.historicoId equals his1.historicoId
                                      join enq1 in db.Enquadramentos on cob1.enquadramentoId equals enq1.enquadramentoId
                                      where cob1.empresaId.Equals(sessaoCorrente.empresaId) &&
                                            (_descricao == null || String.IsNullOrEmpty(_descricao) || gcob1.descricao.Contains(_descricao.Trim()))
                                      select cob1).Count()
                    }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new CobrancaModel().getObject((CobrancaViewModel)id);
        }
        #endregion
    }

    public class ListViewCobrancaByCobrancaID : ListViewModel<CobrancaViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewCobrancaByCobrancaID() { }
        public ListViewCobrancaByCobrancaID(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe ListViewRepository
        public override IEnumerable<CobrancaViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            int _cobrancaId = param != null && param.Count() > 0 && param[0] != null ? int.Parse(param[0].ToString()) : 0;
            string _descricao = param != null && param.Count() > 1 && param[1] != null ? param[1].ToString() : null;

            return (from cob in db.Cobrancas
                    join cob_cli in db.CobrancaClientes on cob.cobrancaId equals cob_cli.cobrancaId
                    join cli in db.Clientes on cob_cli.clienteId equals cli.clienteId
                    join gcob in db.GrupoCobrancas on cob.grupoCobrancaId equals gcob.grupoCobrancaId
                    join ban in db.Bancos on cob.bancoId equals ban.bancoId
                    join his in db.Historicos on cob.historicoId equals his.historicoId
                    join enq in db.Enquadramentos on cob.enquadramentoId equals enq.enquadramentoId
                    where cob.cobrancaId == _cobrancaId &&
                          cob.empresaId.Equals(sessaoCorrente.empresaId) &&
                          (_descricao == null || String.IsNullOrEmpty(_descricao) || cli.nome.Contains(_descricao.Trim()))
                    orderby cli.nome
                    select new CobrancaViewModel
                    {
                        cobrancaId = cob.cobrancaId,
                        grupoCobrancaId = cob.grupoCobrancaId,
                        descricao_grupoCobranca = gcob.descricao,
                        empresaId = sessaoCorrente.empresaId,
                        bancoId = cob.bancoId,
                        nome_banco = ban.sigla,
                        historicoId = cob.historicoId,
                        descricao_historico = his.descricao,
                        enquadramentoId = cob.enquadramentoId,
                        descricao_enquadramento = enq.descricao,
                        dt_inicio = cob.dt_inicio,
                        dt_fim = cob.dt_fim,
                        num_parcelas = cob.num_parcelas,
                        dia_vencimento = cob.dia_vencimento,
                        mes_dia = cob.mes_dia,
                        valor = cob.valor,
                        vr_jurosMora = cob.vr_jurosMora,
                        vr_multa = cob.vr_multa,
                        CobrancaClienteViewModel = new CobrancaClienteViewModel()
                        {
                            cobrancaId = cob.cobrancaId,
                            clienteId = cob_cli.clienteId,
                            nome_cliente = cli.nome,
                            dia_vencimento = cob_cli.dia_vencimento > 0 ? cob_cli.dia_vencimento.Value : cob.dia_vencimento,
                            mes_dia = cob_cli.mes_dia,
                            valor = cob_cli.valor > 0 ? cob_cli.valor.Value : cob.valor,
                            dt_desativacao = cob_cli.dt_desativacao
                        },
                        total_clientes = (from cobcli in db.CobrancaClientes where cobcli.cobrancaId == cob.cobrancaId select cobcli.cobrancaId).Count(),
                        PageSize = pageSize,
                        TotalCount = (from cob1 in db.Cobrancas
                                      join cob_cli1 in db.CobrancaClientes on cob1.cobrancaId equals cob_cli1.cobrancaId
                                      join cli1 in db.Clientes on cob_cli1.clienteId equals cli1.clienteId
                                      join gcob1 in db.GrupoCobrancas on cob1.grupoCobrancaId equals gcob1.grupoCobrancaId
                                      join ban1 in db.Bancos on cob1.bancoId equals ban1.bancoId
                                      join his1 in db.Historicos on cob1.historicoId equals his1.historicoId
                                      join enq1 in db.Enquadramentos on cob1.enquadramentoId equals enq1.enquadramentoId
                                      where cob1.cobrancaId == _cobrancaId &&
                                            cob1.empresaId.Equals(sessaoCorrente.empresaId) &&
                                            (_descricao == null || String.IsNullOrEmpty(_descricao) || cli1.nome.Contains(_descricao.Trim()))
                                      orderby cli1.nome
                                      select cob1).Count()
                    }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();
        }

        public override string action()
        {
            return "ListClientes";
        }

        public override string DivId()
        {
            return "div-list-cob-cli";
        }

        public override Repository getRepository(Object id)
        {
            return new CobrancaModel().getObject((CobrancaViewModel)id);
        }
        #endregion
    }

}