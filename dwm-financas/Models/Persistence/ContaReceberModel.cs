﻿using System;
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
using DWM.Models.BI;

namespace DWM.Models.Persistence
{
    public class ContaReceberModel : OperacaoModel<ContaReceber,ContaReceberViewModel, ContaReceberParcela, ContaReceberParcelaViewModel, ContaReceberParcelaEvento,ContaReceberParcelaEventoViewModel,ContaReceberParcelaCrudModel,ContaReceberParcelaEventoModel>
    {
        #region Métodos da classe CrudModel
        protected override string getTipoMovto()
        {
            return "C";
        }
        protected override DateTime getDataUltimaInclusao()
        {
            DateTime d = Funcoes.Brasilia().Date;
            if (db.ContaRecebers.ToList().Where(info => info.empresaId.Equals(sessaoCorrente.empresaId)).Count() > 0)
                d = db.ContaRecebers.ToList().Where(info => info.empresaId.Equals(sessaoCorrente.empresaId)).OrderByDescending(ord => ord.operacaoId).Take(1).Last().dt_emissao;

            return d;
        }

        public override ContaReceber MapToEntity(ContaReceberViewModel value)
        {
            ContaReceber c = base.MapToEntity(value);
            c.clienteId = value.clienteId;

            return c;
        }

        public override ContaReceberViewModel MapToRepository(ContaReceber entity)
        {
            ContaReceberViewModel r = base.MapToRepository(entity);
            r.clienteId = entity.clienteId;
            r.nome_cliente = db.Clientes.Find(entity.clienteId).nome;
            r.descricao_grupoCliente = db.Clientes.Find(entity.clienteId).grupoClienteId.HasValue ? db.GrupoClientes.Find(db.Clientes.Find(entity.clienteId).grupoClienteId).nome : "";

            return r;
        }

        public override ContaReceber Find(ContaReceberViewModel key)
        {
            ContaReceber entity = db.ContaRecebers.Find(key.operacaoId);
            if (entity != null && (entity.OperacaoParcelas.Count() == 0 || entity.empresaId != sessaoCorrente.empresaId))
                return null;

            return entity;
        }

        public override Validate Validate(ContaReceberViewModel value, Crud operation)
        {
            value.mensagem = base.Validate(value, operation);
            if (value.mensagem.Code == 0 && operation != Crud.EXCLUIR && value.clienteId == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.MessageBase = MensagemPadrao.Message(5, "Cliente").ToString();
                value.mensagem.Message = "Campo obrigatório: Cliente";
                value.mensagem.MessageType = MsgType.WARNING;
            }

            return value.mensagem;
        }

        public override ContaReceberViewModel CreateRepository(System.Web.HttpRequestBase Request = null)
        {
            ContaReceberViewModel r = base.CreateRepository(Request);

            r.nome_cliente = r.nome_cliente == null ? "" : r.nome_cliente;
            r.descricao_grupoCliente = r.descricao_grupoCliente == null ? "" : r.descricao_grupoCliente;

            return r;
        }
        #endregion

        #region Métodos Customizados

        #endregion
    }

    public class ListViewContaReceber : ListViewModel<ContaReceberViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewContaReceber() { }
        public ListViewContaReceber(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe ListViewRepository
        public override IEnumerable<ContaReceberViewModel> Bind(int? index, int receSize = 50, params object[] param)
        {
            #region Parâmetros
            #region Títulos em Aberto
            bool titulos_vencidos_atraso = (bool)param[0];
            DateTime? dt_vencidos_atraso1 = (DateTime?)param[1];
            DateTime? dt_vencidos_atraso2 = (DateTime?)param[2];

            bool titulos_a_vencer = (bool)param[3];
            DateTime? dt_vencimento1 = (DateTime?)param[4];
            DateTime? dt_vencimento2 = (DateTime?)param[5];

            bool titulos_amortizados = (bool)param[6];
            bool titulos_nao_recos = (bool)param[7];
            #endregion

            #region Títulos Baixados
            bool baixa_liquidacao = (bool)param[8];
            bool baixa_cancelamento = (bool)param[9];
            DateTime? dt_baixa1 = (DateTime?)param[10];
            DateTime? dt_baixa2 = (DateTime?)param[11];
            #endregion

            #region Demais parâmetros
            int? clienteId = (int?)param[12];
            DateTime? dt_emissao1 = (DateTime?)param[13];
            DateTime? dt_emissao2 = (DateTime?)param[14];
            int? centroCustoId = (int?)param[15];
            int? grupoId = (int?)param[16];
            int? bancoId = (int?)param[17];
            #endregion
            #endregion

            DateTime hoje = Funcoes.Brasilia().Date;

            #region LINQ
            var q = (from rec in db.ContaRecebers
                     join par in db.ContaReceberParcelas on rec.operacaoId equals par.operacaoId
                     //join pge in db.ContaReceberParcelaEventos on new { par.operacaoId, par.parcelaId } equals new { pge.operacaoId, pge.parcelaId }
                     join cli in db.Clientes on rec.clienteId equals cli.clienteId
                     where rec.empresaId.Equals(sessaoCorrente.empresaId)
                            && ((((titulos_vencidos_atraso && par.vr_saldo_devedor > 0
                                && ((dt_vencidos_atraso1.HasValue && par.dt_vencimento >= dt_vencidos_atraso1 && par.dt_vencimento <= dt_vencidos_atraso2) ||
                                     (!dt_vencidos_atraso1.HasValue && par.dt_vencimento < hoje)))
                                    || (titulos_a_vencer && par.vr_saldo_devedor > 0
                                        && ((dt_vencimento1.HasValue && par.dt_vencimento >= dt_vencimento1 && par.dt_vencimento <= dt_vencimento2) ||
                                         (!dt_vencimento1.HasValue && par.dt_vencimento >= hoje))))
                                && ((titulos_amortizados && par.vr_saldo_devedor > 0 && par.vr_amortizacao > 0)
                                    || (titulos_nao_recos && par.vr_saldo_devedor > 0 && par.vr_amortizacao == 0)))
                                || (baixa_liquidacao && par.ind_baixa == "4" && par.dt_baixa >= dt_baixa1 && par.dt_baixa <= dt_baixa2)
                                || (baixa_cancelamento && par.ind_baixa == "5" && par.dt_baixa >= dt_baixa1 && par.dt_baixa <= dt_baixa2))
                            && (!clienteId.HasValue || rec.clienteId == clienteId)
                            && (!centroCustoId.HasValue || rec.centroCustoId == centroCustoId)
                            && (!grupoId.HasValue || cli.grupoClienteId.Value == grupoId)
                            && (!bancoId.HasValue || par.bancoId.Value == bancoId)
                            && (!dt_emissao1.HasValue || rec.dt_emissao >= dt_emissao1 && rec.dt_emissao <= dt_emissao2)
                     orderby par.dt_vencimento
                     select new ContaReceberViewModel
                     {
                         operacaoId = rec.operacaoId,
                         empresaId = rec.empresaId,
                         nome_cliente = cli.nome,
                         dt_emissao = rec.dt_emissao,
                         documento = rec.documento,
                         recorrencia = rec.recorrencia,
                         OperacaoParcela = new ContaReceberParcelaViewModel()
                         {
                             operacaoId = par.operacaoId,
                             parcelaId = par.parcelaId,
                             dt_vencimento = par.dt_vencimento,
                             ind_baixa = par.ind_baixa,
                             dt_baixa = par.dt_baixa,
                             dt_ultima_amortizacao = par.dt_ultima_amortizacao,
                             vr_principal = par.vr_principal,
                             vr_encargos = par.vr_encargos,
                             vr_desconto = par.vr_desconto,
                             vr_amortizacao = par.vr_amortizacao,
                             vr_total_pago = par.vr_total_pago,
                             vr_saldo_devedor = par.vr_saldo_devedor
                         },
                         PageSize = receSize,
                         TotalCount = 0,
                         //TotalCount = (from rec1 in db.ContaRecebers
                         //               join par1 in db.ContaReceberParcelas on rec1.operacaoId equals par1.operacaoId
                         //               join cli1 in db.Clientes on rec1.clienteId equals cli1.clienteId
                         //               where rec1.empresaId.Equals(sessaoCorrente.empresaId)
                         //                      && ((((titulos_vencidos_atraso && par1.vr_saldo_devedor > 0
                         //                          && ((dt_vencidos_atraso1.HasValue && par1.dt_vencimento >= dt_vencidos_atraso1 && par1.dt_vencimento <= dt_vencidos_atraso2) ||
                         //                               (!dt_vencidos_atraso1.HasValue && par1.dt_vencimento < hoje)))
                         //                              || (titulos_a_vencer && par1.vr_saldo_devedor > 0
                         //                                  && ((dt_vencimento1.HasValue && par1.dt_vencimento >= dt_vencimento1 && par1.dt_vencimento <= dt_vencimento2) ||
                         //                                   (!dt_vencimento1.HasValue && par1.dt_vencimento >= hoje))))
                         //                          && ((titulos_amortizados && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao > 0)
                         //                              || (titulos_nao_recos && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao == 0)))
                         //                          || (baixa_liquidacao && par1.ind_baixa == "4" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2)
                         //                          || (baixa_cancelamento && par1.ind_baixa == "5" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2))
                         //                      && (!clienteId.HasValue || rec1.clienteId == clienteId)
                         //                      && (!centroCustoId.HasValue || rec1.centroCustoId == centroCustoId)
                         //                      && (!grupoId.HasValue || cli1.grupoClienteId.Value == grupoId)
                         //                      && (!bancoId.HasValue || par1.bancoId.Value == bancoId)
                         //                      && (!dt_emissao1.HasValue || rec1.dt_emissao >= dt_emissao1 && rec1.dt_emissao <= dt_emissao2)
                         //               orderby par1.dt_vencimento
                         //               select rec1).Count()
                     }).ToList();
            #endregion

            return q;
        }

        public override Repository getRepository(Object id)
        {
            ContaReceberModel model = new ContaReceberModel();
            model.Create(this.db, this.seguranca_db);
            return model.getObject((ContaReceberViewModel)id);
        }

        public override string action()
        {
            return "ListParam";
        }


        public override string DivId()
        {
            return "div-list-static";
        }
        #endregion
    }
}