using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Models;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Linq;

namespace DWM.Models.BI
{
    public class ContaReceberEditarBI : OperacaoEditarBI<EditarContaReceberViewModel,EditarContaReceberParcelaEventoViewModel>
    {
        public override EditarContaReceberViewModel Run(Repository value)
        {
            EditarContaReceberViewModel r = (EditarContaReceberViewModel)value;
            try
            {
                #region Calcula o próximo dia útil em relação à data de referência
                DateTime? dt_referencia = Funcoes.Brasilia().Date;
                ObterProximoDiaUtil obterProximoDiaUtil = new ObterProximoDiaUtil(this.db, this.seguranca_db, dt_referencia);
                obterProximoDiaUtil.Run(new FeriadoViewModel());
                DateTime dt_proximo_diaUtil = obterProximoDiaUtil.dt_referencia;
                #endregion

                r = (from cr in db.ContaRecebers
                     join his in db.Historicos on cr.historicoId equals his.historicoId
                     join cli in db.Clientes on cr.clienteId equals cli.clienteId
                     join gcli in db.GrupoClientes on cli.grupoClienteId equals gcli.grupoClienteId into GCLI
                     from gcli in GCLI.DefaultIfEmpty()
                     join ccu in db.CentroCustos on cr.centroCustoId equals ccu.centroCustoId into CCU
                     from ccu in CCU.DefaultIfEmpty()
                     join par in db.ContaReceberParcelas on cr.operacaoId equals par.operacaoId
                     join ban in db.Bancos on par.bancoId equals ban.bancoId
                     where cr.operacaoId == r.operacaoId
                             && par.parcelaId == r.parcelaId
                     select new EditarContaReceberViewModel()
                     {
                         operacaoId = r.operacaoId,
                         parcelaId = r.parcelaId,
                         dt_emissao = cr.dt_emissao,
                         clienteId = cr.clienteId,
                         nome_cliente = cli.nome,
                         grupoClienteId = gcli.grupoClienteId,
                         descricao_grupoCliente = gcli.nome,
                         historicoId = cr.historicoId,
                         descricao_historico = his.descricao,
                         complementoHist = cr.complementoHist,
                         centroCustoId = cr.centroCustoId,
                         descricao_centroCusto = ccu.descricao,
                         dt_pagamento = dt_proximo_diaUtil,
                         dt_movto = dt_proximo_diaUtil,
                         hasMultaMora = (cr.vr_jurosMora.HasValue && cr.vr_jurosMora > 0) || (cr.vr_multa.HasValue && cr.vr_multa > 0),
                         vr_total = (from cr1 in db.ContaReceberParcelas where cr1.operacaoId == r.operacaoId select cr1.vr_principal).Sum(),
                         qte_parcelas = (from cr2 in db.ContaReceberParcelas where cr2.operacaoId == r.operacaoId select cr2.parcelaId).Count(),
                         Contabilidades = (from con in db.ContabilidadeItems
                                           join pc in db.PlanoContas on con.planoContaId equals pc.planoContaId
                                           where cr.contabilidadeId != null && con.contabilidadeId == cr.contabilidadeId
                                           select new ContabilidadeItemViewModel
                                           {
                                               contabilidadeId = con.contabilidadeId,
                                               codigoReduzido = pc.codigoReduzido,
                                               descricao_planoConta = pc.descricao,
                                               tipoLancamento = con.tipoLancamento,
                                               valor = con.valor
                                           }).ToList(),
                         editarOperacaoParcelaEventoViewModel = new EditarContaReceberParcelaEventoViewModel()
                         {
                             historicoId = cr.historicoId,
                             complementoHist = cr.complementoHist,
                             dt_ocorrencia = DateTime.Today
                         },
                         bancoId = par.bancoId,
                         nome_banco = ban.nome,
                         num_titulo = par.num_titulo,
                         dt_vencimento = par.dt_vencimento,
                         vr_principal = par.vr_principal,
                         vr_amortizacao = par.vr_amortizacao,
                         vr_total_pago = par.vr_total_pago,
                         vr_desconto = par.vr_desconto,
                         vr_encargos = par.vr_encargos,
                         vr_saldo_devedor = par.vr_saldo_devedor,
                         vr_multa_atraso_baixa = (from creve1 in db.ContaReceberParcelaEventos
                                                  join eve1 in db.Eventos on creve1.eventoId equals eve1.eventoId
                                                  where creve1.operacaoId == r.operacaoId && creve1.parcelaId == r.parcelaId
                                                        && eve1.codigo == 10
                                                        && creve1.ind_estorno == "N"
                                                  select creve1.valor).DefaultIfEmpty(0).Sum(),
                         vr_juros_mora_baixa = (from creve2 in db.ContaReceberParcelaEventos
                                                join eve2 in db.Eventos on creve2.eventoId equals eve2.eventoId
                                                where creve2.operacaoId == r.operacaoId && creve2.parcelaId == r.parcelaId
                                                      && eve2.codigo == 1
                                                      && creve2.ind_estorno == "N"
                                                select creve2.valor).DefaultIfEmpty(0).Sum(),
                         vr_baixa = 0,
                         vr_desconto_baixa = 0,
                         ind_forma_pagamento = par.ind_forma_pagamento,
                         codigo_barras = par.codigo_barras,
                         dt_ultima_amortizacao = par.dt_ultima_amortizacao,
                         ind_baixa = par.ind_baixa,
                         dt_baixa = par.dt_baixa,
                         cheque_banco = par.cheque_banco,
                         cheque_agencia = par.cheque_agencia,
                         cheque_numero = par.cheque_numero,
                         OperacaoParcelaEventos = (from creve in db.ContaReceberParcelaEventos
                                                     join eve in db.Eventos on creve.eventoId equals eve.eventoId
                                                     where creve.operacaoId == r.operacaoId && creve.parcelaId == r.parcelaId
                                                     select new ContaReceberParcelaEventoViewModel
                                                     {
                                                         operacaoId = creve.operacaoId,
                                                         parcelaId = creve.parcelaId,
                                                         dt_evento = creve.dt_evento,
                                                         dt_ocorrencia = creve.dt_ocorrencia,
                                                         descricao_evento = eve.descricao,
                                                         ind_estorno = creve.ind_estorno,
                                                         contabilidadeId = creve.contabilidadeId,
                                                         movtoBancarioId = creve.movtoBancarioId,
                                                         ind_tipoEvento = creve.ind_tipoEvento,
                                                         ind_operacao = creve.ind_operacao == "D" ? "Débito" : "Crédito",
                                                         valor = creve.valor,
                                                         arquivo = creve.arquivo
                                                     }).ToList()

                     }).FirstOrDefault();

                if ((from creve3 in db.ContaReceberParcelaEventos
                     join eve3 in db.Eventos on creve3.eventoId equals eve3.eventoId
                     where creve3.operacaoId == r.operacaoId && creve3.parcelaId == r.parcelaId
                           && eve3.codigo == 11
                           && creve3.ind_estorno == "N"
                     select creve3.valor).DefaultIfEmpty(0).Sum() > 0)
                {
                    r.vr_juros_mora_baixa = (from creve2 in db.ContaReceberParcelaEventos
                                             join eve2 in db.Eventos on creve2.eventoId equals eve2.eventoId
                                             where creve2.operacaoId == r.operacaoId && creve2.parcelaId == r.parcelaId
                                                   && (eve2.codigo == 1 || eve2.codigo == 10)
                                                   && creve2.ind_estorno == "N"
                                             select creve2.valor).DefaultIfEmpty(0).Sum() -
                            (from creve3 in db.ContaReceberParcelaEventos
                             join eve3 in db.Eventos on creve3.eventoId equals eve3.eventoId
                             where creve3.operacaoId == r.operacaoId && creve3.parcelaId == r.parcelaId
                                   && eve3.codigo == 11
                                   && creve3.ind_estorno == "N"
                             select creve3.valor).DefaultIfEmpty(0).Sum();

                    r.vr_multa_atraso_baixa = (from creve1 in db.ContaReceberParcelaEventos
                                               join eve1 in db.Eventos on creve1.eventoId equals eve1.eventoId
                                               where creve1.operacaoId == r.operacaoId && creve1.parcelaId == r.parcelaId
                                                     && eve1.codigo == 10
                                                     && creve1.ind_estorno == "N"
                                               select creve1.valor).DefaultIfEmpty(0).Sum() -
                                                 ((from creve3 in db.ContaReceberParcelaEventos
                                                   join eve3 in db.Eventos on creve3.eventoId equals eve3.eventoId
                                                   where creve3.operacaoId == r.operacaoId && creve3.parcelaId == r.parcelaId
                                                         && eve3.codigo == 11
                                                         && creve3.ind_estorno == "N"
                                                   select creve3.valor).DefaultIfEmpty(0).Sum() -
                                                  (from creve4 in db.ContaReceberParcelaEventos
                                                   join eve4 in db.Eventos on creve4.eventoId equals eve4.eventoId
                                                   where creve4.operacaoId == r.operacaoId && creve4.parcelaId == r.parcelaId
                                                         && eve4.codigo == 1
                                                         && creve4.ind_estorno == "N"
                                                   select creve4.valor).DefaultIfEmpty(0).Sum()) -
                                               r.vr_juros_mora_baixa;
                }

                r.vr_multa_atraso = r.vr_multa_atraso ?? 0;
            }
            catch (Exception ex)
            {
                r.mensagem = new Validate() { Code = 999, MessageBase = ex.Message, Message = "Ocorreu um erro na edição da comissão" };
            }

            return r;
        }
    }
}