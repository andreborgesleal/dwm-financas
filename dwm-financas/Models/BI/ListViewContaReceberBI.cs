using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Models;
using DWM.Models.Entidades;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DWM.Models.BI
{
    public class ListViewContaReceberBI : DWMContext<ApplicationContext>, IProcess<EditarContaReceberViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewContaReceberBI() { }

        public ListViewContaReceberBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual EditarContaReceberViewModel Run(Repository value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EditarContaReceberViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int receSize = 50, params object[] param)
        {
            int receIndex = index ?? 0;

            #region Parâmetros
            int? clienteId = (int?)param[0];
            DateTime? dt_emissao1 = (DateTime?)param[1];
            DateTime? dt_emissao2 = (DateTime?)param[2];
            #endregion

            DateTime hoje = Funcoes.Brasilia().Date;

            #region LINQ
            var q = (from rec in db.ContaRecebers
                     join cli in db.Clientes on rec.clienteId equals cli.clienteId
                     join his in db.Historicos on rec.historicoId equals his.historicoId
                     join par in db.ContaReceberParcelas on rec.operacaoId equals par.operacaoId
                     group par by new
                     {
                         rec.empresaId,
                         par.operacaoId,
                         rec.dt_emissao,
                         cli.clienteId,
                         cli.nome,
                         his.historicoId,
                         rec.complementoHist,
                         his.descricao,
                     } into PAR
                     where (PAR.Key.empresaId == sessaoCorrente.empresaId
                            && !clienteId.HasValue || PAR.Key.clienteId == clienteId)
                            && (!dt_emissao1.HasValue || PAR.Key.dt_emissao >= dt_emissao1 && PAR.Key.dt_emissao <= dt_emissao2)
                     orderby PAR.Key.nome, PAR.Key.operacaoId
                     select new EditarContaReceberViewModel
                     {
                         operacaoId = PAR.Key.operacaoId,
                         empresaId = PAR.Key.empresaId,
                         nome_cliente = PAR.Key.nome,
                         descricao_historico = PAR.Key.descricao,
                         complementoHist = PAR.Key.complementoHist,
                         dt_emissao = PAR.Key.dt_emissao,
                         dt_baixa = PAR.Max(info => info.dt_baixa),
                         dt_ultima_amortizacao = PAR.Max(info => info.dt_ultima_amortizacao),
                         vr_principal = PAR.Sum(info => info.vr_principal),
                         vr_encargos = PAR.Sum(info => info.vr_encargos),
                         vr_desconto = PAR.Sum(info => info.vr_desconto),
                         vr_amortizacao = PAR.Sum(info => info.vr_amortizacao),
                         vr_total_pago = PAR.Sum(info => info.vr_total_pago),
                         vr_saldo_devedor = PAR.Sum(info => info.vr_saldo_devedor),
                         PageSize = receSize,
                         TotalCount = 0
                         //TotalCount = (from rec1 in db.ContaRecebers
                         //              join cli1 in db.Clientes on rec1.clienteId equals cli1.clienteId
                         //              join his1 in db.Historicos on rec1.historicoId equals his1.historicoId
                         //              join par1 in db.ContaReceberParcelas on rec1.operacaoId equals par1.operacaoId
                         //              group par1 by new
                         //              {
                         //                  rec1.empresaId,
                         //                  par1.operacaoId,
                         //                  rec1.dt_emissao,
                         //                  cli1.clienteId,
                         //                  cli1.nome,
                         //                  his1.historicoId,
                         //                  rec1.complementoHist,
                         //                  his1.descricao,
                         //              } into PAR1
                         //              where (PAR1.Key.empresaId == sessaoCorrente.empresaId
                         //                     && !clienteId.HasValue || PAR1.Key.clienteId == clienteId)
                         //                     && (!dt_emissao1.HasValue || PAR1.Key.dt_emissao >= dt_emissao1 && PAR1.Key.dt_emissao <= dt_emissao2)
                         //              orderby PAR1.Key.nome, PAR1.Key.operacaoId
                         //              select PAR1).Count()
                     }).ToList();
            #endregion

            return new PagedList<EditarContaReceberViewModel>(q.ToList(), receIndex, receSize, q.Count() > 0 ? q.First().TotalCount : 0, "ListOperacaoParam", null, "div-list-static");
        }

    }

    public class ListViewContaReceberDemonstrativoBI : DWMContext<ApplicationContext>, IProcess<ContaReceberDemonstrativoViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewContaReceberDemonstrativoBI() { }

        public ListViewContaReceberDemonstrativoBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual ContaReceberDemonstrativoViewModel Run(Repository value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContaReceberDemonstrativoViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int receSize = 50, params object[] param)
        {
            int receIndex = index ?? 0;

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
                     join his in db.Historicos on rec.historicoId equals his.historicoId
                     join gru in db.GrupoClientes on cli.grupoClienteId equals gru.grupoClienteId into GRU
                     from gru in GRU.DefaultIfEmpty()
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
                     select new ContaReceberDemonstrativoViewModel
                     {
                         operacaoId = rec.operacaoId,
                         empresaId = rec.empresaId,
                         nome_cliente = cli.nome,
                         descricao_historico = his.descricao,
                         complementoHist = rec.complementoHist,
                         dt_emissao = rec.dt_emissao,
                         documento = rec.documento,
                         recorrencia = rec.recorrencia,
                         descricao_grupoCliente = gru.nome,
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
                         TotalAmortizado = (from rec1 in db.ContaRecebers
                                            join par1 in db.ContaReceberParcelas on rec1.operacaoId equals par1.operacaoId
                                            join cli1 in db.Clientes on rec1.clienteId equals cli1.clienteId
                                            where rec1.empresaId.Equals(sessaoCorrente.empresaId)
                                                   && ((((titulos_vencidos_atraso && par1.vr_saldo_devedor > 0
                                                       && ((dt_vencidos_atraso1.HasValue && par1.dt_vencimento >= dt_vencidos_atraso1 && par1.dt_vencimento <= dt_vencidos_atraso2) ||
                                                            (!dt_vencidos_atraso1.HasValue && par1.dt_vencimento < hoje)))
                                                           || (titulos_a_vencer && par1.vr_saldo_devedor > 0
                                                               && ((dt_vencimento1.HasValue && par1.dt_vencimento >= dt_vencimento1 && par1.dt_vencimento <= dt_vencimento2) ||
                                                                (!dt_vencimento1.HasValue && par1.dt_vencimento >= hoje))))
                                                       && ((titulos_amortizados && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao > 0)
                                                           || (titulos_nao_recos && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao == 0)))
                                                       || (baixa_liquidacao && par1.ind_baixa == "4" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2)
                                                       || (baixa_cancelamento && par1.ind_baixa == "5" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2))
                                                   && (!clienteId.HasValue || rec1.clienteId == clienteId)
                                                   && (!centroCustoId.HasValue || rec1.centroCustoId == centroCustoId)
                                                   && (!grupoId.HasValue || cli1.grupoClienteId.Value == grupoId)
                                                   && (!bancoId.HasValue || par1.bancoId.Value == bancoId)
                                                   && (!dt_emissao1.HasValue || rec1.dt_emissao >= dt_emissao1 && rec1.dt_emissao <= dt_emissao2)
                                            orderby par1.dt_vencimento
                                            select par1).Select(info => info.vr_amortizacao).Sum(),
                         TotalEmAberto = (from rec1 in db.ContaRecebers
                                          join par1 in db.ContaReceberParcelas on rec1.operacaoId equals par1.operacaoId
                                          join cli1 in db.Clientes on rec1.clienteId equals cli1.clienteId
                                          where rec1.empresaId.Equals(sessaoCorrente.empresaId)
                                                 && ((((titulos_vencidos_atraso && par1.vr_saldo_devedor > 0
                                                     && ((dt_vencidos_atraso1.HasValue && par1.dt_vencimento >= dt_vencidos_atraso1 && par1.dt_vencimento <= dt_vencidos_atraso2) ||
                                                          (!dt_vencidos_atraso1.HasValue && par1.dt_vencimento < hoje)))
                                                         || (titulos_a_vencer && par1.vr_saldo_devedor > 0
                                                             && ((dt_vencimento1.HasValue && par1.dt_vencimento >= dt_vencimento1 && par1.dt_vencimento <= dt_vencimento2) ||
                                                              (!dt_vencimento1.HasValue && par1.dt_vencimento >= hoje))))
                                                     && ((titulos_amortizados && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao > 0)
                                                         || (titulos_nao_recos && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao == 0)))
                                                     || (baixa_liquidacao && par1.ind_baixa == "4" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2)
                                                     || (baixa_cancelamento && par1.ind_baixa == "5" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2))
                                                 && (!clienteId.HasValue || rec1.clienteId == clienteId)
                                                 && (!centroCustoId.HasValue || rec1.centroCustoId == centroCustoId)
                                                 && (!grupoId.HasValue || cli1.grupoClienteId.Value == grupoId)
                                                 && (!bancoId.HasValue || par1.bancoId.Value == bancoId)
                                                 && (!dt_emissao1.HasValue || rec1.dt_emissao >= dt_emissao1 && rec1.dt_emissao <= dt_emissao2)
                                          orderby par1.dt_vencimento
                                          select par1).Select(info => info.vr_saldo_devedor).Sum(),
                         TotalTitulos = 0,
                         TotalEmitido = (from rec1 in db.ContaRecebers
                                         join par1 in db.ContaReceberParcelas on rec1.operacaoId equals par1.operacaoId
                                         join cli1 in db.Clientes on rec1.clienteId equals cli1.clienteId
                                         where rec1.empresaId.Equals(sessaoCorrente.empresaId)
                                                && ((((titulos_vencidos_atraso && par1.vr_saldo_devedor > 0
                                                    && ((dt_vencidos_atraso1.HasValue && par1.dt_vencimento >= dt_vencidos_atraso1 && par1.dt_vencimento <= dt_vencidos_atraso2) ||
                                                         (!dt_vencidos_atraso1.HasValue && par1.dt_vencimento < hoje)))
                                                        || (titulos_a_vencer && par1.vr_saldo_devedor > 0
                                                            && ((dt_vencimento1.HasValue && par1.dt_vencimento >= dt_vencimento1 && par1.dt_vencimento <= dt_vencimento2) ||
                                                             (!dt_vencimento1.HasValue && par1.dt_vencimento >= hoje))))
                                                    && ((titulos_amortizados && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao > 0)
                                                        || (titulos_nao_recos && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao == 0)))
                                                    || (baixa_liquidacao && par1.ind_baixa == "4" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2)
                                                    || (baixa_cancelamento && par1.ind_baixa == "5" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2))
                                                && (!clienteId.HasValue || rec1.clienteId == clienteId)
                                                && (!centroCustoId.HasValue || rec1.centroCustoId == centroCustoId)
                                                && (!grupoId.HasValue || cli1.grupoClienteId.Value == grupoId)
                                                && (!bancoId.HasValue || par1.bancoId.Value == bancoId)
                                                && (!dt_emissao1.HasValue || rec1.dt_emissao >= dt_emissao1 && rec1.dt_emissao <= dt_emissao2)
                                         orderby par1.dt_vencimento
                                         select par1).Select(info => info.vr_principal).Sum(),
                         TotalCount = (from rec1 in db.ContaRecebers
                                       join par1 in db.ContaReceberParcelas on rec1.operacaoId equals par1.operacaoId
                                       join cli1 in db.Clientes on rec1.clienteId equals cli1.clienteId
                                       where rec1.empresaId.Equals(sessaoCorrente.empresaId)
                                              && ((((titulos_vencidos_atraso && par1.vr_saldo_devedor > 0
                                                  && ((dt_vencidos_atraso1.HasValue && par1.dt_vencimento >= dt_vencidos_atraso1 && par1.dt_vencimento <= dt_vencidos_atraso2) ||
                                                       (!dt_vencidos_atraso1.HasValue && par1.dt_vencimento < hoje)))
                                                      || (titulos_a_vencer && par1.vr_saldo_devedor > 0
                                                          && ((dt_vencimento1.HasValue && par1.dt_vencimento >= dt_vencimento1 && par1.dt_vencimento <= dt_vencimento2) ||
                                                           (!dt_vencimento1.HasValue && par1.dt_vencimento >= hoje))))
                                                  && ((titulos_amortizados && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao > 0)
                                                      || (titulos_nao_recos && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao == 0)))
                                                  || (baixa_liquidacao && par1.ind_baixa == "4" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2)
                                                  || (baixa_cancelamento && par1.ind_baixa == "5" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2))
                                              && (!clienteId.HasValue || rec1.clienteId == clienteId)
                                              && (!centroCustoId.HasValue || rec1.centroCustoId == centroCustoId)
                                              && (!grupoId.HasValue || cli1.grupoClienteId.Value == grupoId)
                                              && (!bancoId.HasValue || par1.bancoId.Value == bancoId)
                                              && (!dt_emissao1.HasValue || rec1.dt_emissao >= dt_emissao1 && rec1.dt_emissao <= dt_emissao2)
                                       orderby par1.dt_vencimento
                                       select rec1).Count()
                     }).ToList();
            #endregion

            IPagedList pagedList = new PagedList<ContaReceberDemonstrativoViewModel>(q.ToList(), receIndex, receSize, q.Count() > 0 ? q.First().TotalCount : 0, "ListPanorama", null, "div-list-static");

            pagedList.action = "ListParam";
            pagedList.DivId = "div-list-static";

            return pagedList;
        }
    }

    public class ListViewCNABDemonstrativoBI : DWMContext<ApplicationContext>, IProcess<ContaReceberDemonstrativoViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewCNABDemonstrativoBI() { }

        public ListViewCNABDemonstrativoBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual ContaReceberDemonstrativoViewModel Run(Repository value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContaReceberDemonstrativoViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int receSize = 50, params object[] param)
        {
            int receIndex = index ?? 0;

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
                     join his in db.Historicos on rec.historicoId equals his.historicoId
                     join gru in db.GrupoClientes on cli.grupoClienteId equals gru.grupoClienteId into GRU
                     from gru in GRU.DefaultIfEmpty()
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
                            && (from tit in db.Titulos where new { tit.operacaoId, tit.parcelaId } == new { par.operacaoId, par.parcelaId } select tit.operacaoId).Count() > 0
                     orderby par.dt_vencimento
                     select new ContaReceberDemonstrativoViewModel
                     {
                         operacaoId = rec.operacaoId,
                         empresaId = rec.empresaId,
                         nome_cliente = cli.nome,
                         descricao_historico = his.descricao,
                         complementoHist = rec.complementoHist,
                         dt_emissao = rec.dt_emissao,
                         documento = rec.documento,
                         recorrencia = rec.recorrencia,
                         descricao_grupoCliente = gru.nome,
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
                         TotalAmortizado = (from rec1 in db.ContaRecebers
                                            join par1 in db.ContaReceberParcelas on rec1.operacaoId equals par1.operacaoId
                                            join cli1 in db.Clientes on rec1.clienteId equals cli1.clienteId
                                            where rec1.empresaId.Equals(sessaoCorrente.empresaId)
                                                   && ((((titulos_vencidos_atraso && par1.vr_saldo_devedor > 0
                                                       && ((dt_vencidos_atraso1.HasValue && par1.dt_vencimento >= dt_vencidos_atraso1 && par1.dt_vencimento <= dt_vencidos_atraso2) ||
                                                            (!dt_vencidos_atraso1.HasValue && par1.dt_vencimento < hoje)))
                                                           || (titulos_a_vencer && par1.vr_saldo_devedor > 0
                                                               && ((dt_vencimento1.HasValue && par1.dt_vencimento >= dt_vencimento1 && par1.dt_vencimento <= dt_vencimento2) ||
                                                                (!dt_vencimento1.HasValue && par1.dt_vencimento >= hoje))))
                                                       && ((titulos_amortizados && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao > 0)
                                                           || (titulos_nao_recos && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao == 0)))
                                                       || (baixa_liquidacao && par1.ind_baixa == "4" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2)
                                                       || (baixa_cancelamento && par1.ind_baixa == "5" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2))
                                                   && (!clienteId.HasValue || rec1.clienteId == clienteId)
                                                   && (!centroCustoId.HasValue || rec1.centroCustoId == centroCustoId)
                                                   && (!grupoId.HasValue || cli1.grupoClienteId.Value == grupoId)
                                                   && (!bancoId.HasValue || par1.bancoId.Value == bancoId)
                                                   && (!dt_emissao1.HasValue || rec1.dt_emissao >= dt_emissao1 && rec1.dt_emissao <= dt_emissao2)
                                                   && (from tit1 in db.Titulos where new { tit1.operacaoId, tit1.parcelaId } == new { par1.operacaoId, par1.parcelaId } select tit1.operacaoId).Count() > 0
                                            orderby par1.dt_vencimento
                                            select par1).Select(info => info.vr_amortizacao).Sum(),
                         TotalEmAberto = (from rec1 in db.ContaRecebers
                                          join par1 in db.ContaReceberParcelas on rec1.operacaoId equals par1.operacaoId
                                          join cli1 in db.Clientes on rec1.clienteId equals cli1.clienteId
                                          where rec1.empresaId.Equals(sessaoCorrente.empresaId)
                                                 && ((((titulos_vencidos_atraso && par1.vr_saldo_devedor > 0
                                                     && ((dt_vencidos_atraso1.HasValue && par1.dt_vencimento >= dt_vencidos_atraso1 && par1.dt_vencimento <= dt_vencidos_atraso2) ||
                                                          (!dt_vencidos_atraso1.HasValue && par1.dt_vencimento < hoje)))
                                                         || (titulos_a_vencer && par1.vr_saldo_devedor > 0
                                                             && ((dt_vencimento1.HasValue && par1.dt_vencimento >= dt_vencimento1 && par1.dt_vencimento <= dt_vencimento2) ||
                                                              (!dt_vencimento1.HasValue && par1.dt_vencimento >= hoje))))
                                                     && ((titulos_amortizados && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao > 0)
                                                         || (titulos_nao_recos && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao == 0)))
                                                     || (baixa_liquidacao && par1.ind_baixa == "4" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2)
                                                     || (baixa_cancelamento && par1.ind_baixa == "5" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2))
                                                 && (!clienteId.HasValue || rec1.clienteId == clienteId)
                                                 && (!centroCustoId.HasValue || rec1.centroCustoId == centroCustoId)
                                                 && (!grupoId.HasValue || cli1.grupoClienteId.Value == grupoId)
                                                 && (!bancoId.HasValue || par1.bancoId.Value == bancoId)
                                                 && (!dt_emissao1.HasValue || rec1.dt_emissao >= dt_emissao1 && rec1.dt_emissao <= dt_emissao2)
                                                 && (from tit1 in db.Titulos where new { tit1.operacaoId, tit1.parcelaId } == new { par1.operacaoId, par1.parcelaId } select tit1.operacaoId).Count() > 0
                                          orderby par1.dt_vencimento
                                          select par1).Select(info => info.vr_saldo_devedor).Sum(),
                         TotalTitulos = 0,
                         TotalEmitido = (from rec1 in db.ContaRecebers
                                         join par1 in db.ContaReceberParcelas on rec1.operacaoId equals par1.operacaoId
                                         join cli1 in db.Clientes on rec1.clienteId equals cli1.clienteId
                                         where rec1.empresaId.Equals(sessaoCorrente.empresaId)
                                                && ((((titulos_vencidos_atraso && par1.vr_saldo_devedor > 0
                                                    && ((dt_vencidos_atraso1.HasValue && par1.dt_vencimento >= dt_vencidos_atraso1 && par1.dt_vencimento <= dt_vencidos_atraso2) ||
                                                         (!dt_vencidos_atraso1.HasValue && par1.dt_vencimento < hoje)))
                                                        || (titulos_a_vencer && par1.vr_saldo_devedor > 0
                                                            && ((dt_vencimento1.HasValue && par1.dt_vencimento >= dt_vencimento1 && par1.dt_vencimento <= dt_vencimento2) ||
                                                             (!dt_vencimento1.HasValue && par1.dt_vencimento >= hoje))))
                                                    && ((titulos_amortizados && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao > 0)
                                                        || (titulos_nao_recos && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao == 0)))
                                                    || (baixa_liquidacao && par1.ind_baixa == "4" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2)
                                                    || (baixa_cancelamento && par1.ind_baixa == "5" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2))
                                                && (!clienteId.HasValue || rec1.clienteId == clienteId)
                                                && (!centroCustoId.HasValue || rec1.centroCustoId == centroCustoId)
                                                && (!grupoId.HasValue || cli1.grupoClienteId.Value == grupoId)
                                                && (!bancoId.HasValue || par1.bancoId.Value == bancoId)
                                                && (!dt_emissao1.HasValue || rec1.dt_emissao >= dt_emissao1 && rec1.dt_emissao <= dt_emissao2)
                                                && (from tit1 in db.Titulos where new { tit1.operacaoId, tit1.parcelaId } == new { par1.operacaoId, par1.parcelaId } select tit1.operacaoId).Count() > 0
                                         orderby par1.dt_vencimento
                                         select par1).Select(info => info.vr_principal).Sum(),
                         TotalCount = (from rec1 in db.ContaRecebers
                                       join par1 in db.ContaReceberParcelas on rec1.operacaoId equals par1.operacaoId
                                       join cli1 in db.Clientes on rec1.clienteId equals cli1.clienteId
                                       where rec1.empresaId.Equals(sessaoCorrente.empresaId)
                                              && ((((titulos_vencidos_atraso && par1.vr_saldo_devedor > 0
                                                  && ((dt_vencidos_atraso1.HasValue && par1.dt_vencimento >= dt_vencidos_atraso1 && par1.dt_vencimento <= dt_vencidos_atraso2) ||
                                                       (!dt_vencidos_atraso1.HasValue && par1.dt_vencimento < hoje)))
                                                      || (titulos_a_vencer && par1.vr_saldo_devedor > 0
                                                          && ((dt_vencimento1.HasValue && par1.dt_vencimento >= dt_vencimento1 && par1.dt_vencimento <= dt_vencimento2) ||
                                                           (!dt_vencimento1.HasValue && par1.dt_vencimento >= hoje))))
                                                  && ((titulos_amortizados && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao > 0)
                                                      || (titulos_nao_recos && par1.vr_saldo_devedor > 0 && par1.vr_amortizacao == 0)))
                                                  || (baixa_liquidacao && par1.ind_baixa == "4" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2)
                                                  || (baixa_cancelamento && par1.ind_baixa == "5" && par1.dt_baixa >= dt_baixa1 && par1.dt_baixa <= dt_baixa2))
                                              && (!clienteId.HasValue || rec1.clienteId == clienteId)
                                              && (!centroCustoId.HasValue || rec1.centroCustoId == centroCustoId)
                                              && (!grupoId.HasValue || cli1.grupoClienteId.Value == grupoId)
                                              && (!bancoId.HasValue || par1.bancoId.Value == bancoId)
                                              && (!dt_emissao1.HasValue || rec1.dt_emissao >= dt_emissao1 && rec1.dt_emissao <= dt_emissao2)
                                              && (from tit1 in db.Titulos where new { tit1.operacaoId, tit1.parcelaId } == new { par1.operacaoId, par1.parcelaId } select tit1.operacaoId).Count() > 0
                                       orderby par1.dt_vencimento
                                       select rec1).Count()
                     }).ToList();
            #endregion

            IPagedList pagedList = new PagedList<ContaReceberDemonstrativoViewModel>(q.ToList(), receIndex, receSize, q.Count() > 0 ? q.First().TotalCount : 0, "ListPanorama", null, "div-list-static");

            pagedList.action = "ListParam";
            pagedList.DivId = "div-list-static";

            return pagedList;
        }
    }

}