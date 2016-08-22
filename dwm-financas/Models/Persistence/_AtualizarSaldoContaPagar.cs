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
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace DWM.Models.Persistence
{
    public class AtualizarSaldoContaPagar : DWMContext<ApplicationContext>, IProcess<ContaPagarParcelaViewModel, ApplicationContext>
    {
        public decimal? vr_jurosMora { get; set; }
        public decimal? vr_multa { get; set; }
        public DateTime dt_referencia { get; set; }

        #region Constructor
        public AtualizarSaldoContaPagar() { }

        public AtualizarSaldoContaPagar(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public ContaPagarParcelaViewModel Run(Repository value)
        {
            if (value == null)
                return (ContaPagarParcelaViewModel)value;

            ContaPagarParcelaViewModel r = (ContaPagarParcelaViewModel)value;

            // calcular o total amortizado (RN-010)
            r.vr_amortizacao = (from pev in db.ContaPagarParcelaEventos
                                  where new { pev.operacaoId, pev.parcelaId } == new { r.operacaoId, r.parcelaId } 
                                        && "3|4".Contains(pev.ind_tipoEvento) 
                                        && pev.ind_estorno == "N"
                                  select pev.valor).Sum();

            // calcular o total de encargos (RN-011)
            r.vr_encargos = (from pev in db.ContaPagarParcelaEventos
                               where new { pev.operacaoId, pev.parcelaId } == new { r.operacaoId, r.parcelaId } 
                                     && pev.ind_tipoEvento == "1"
                                     && pev.ind_estorno == "N"
                               select pev.valor).Sum();

            // calcular o total de descontos (RN-012)
            r.vr_desconto = (from pev in db.ContaPagarParcelaEventos
                               where new { pev.operacaoId, pev.parcelaId } == new { r.operacaoId, r.parcelaId } 
                                     && pev.ind_tipoEvento == "2"
                                     && pev.ind_estorno == "N"
                               select pev.valor).Sum();

            // calcular o saldo devedor (RN-013)
            r.vr_saldo_devedor = (from pev in db.ContaPagarParcelaEventos
                                    where new { pev.operacaoId, pev.parcelaId } == new { r.operacaoId, r.parcelaId } 
                                          && pev.ind_operacao == "C"
                                    select pev.valor).Sum() -
                                   (from pev in db.ContaPagarParcelaEventos
                                    where new { pev.operacaoId, pev.parcelaId } == new { r.operacaoId, r.parcelaId } 
                                          && pev.ind_operacao == "D"
                                    select pev.valor).Sum() ;

            // calcula a data da última amortização
            r.dt_ultima_amortizacao = (from pev in db.ContaPagarParcelaEventos
                                         where new { pev.operacaoId, pev.parcelaId } == new { r.operacaoId, r.parcelaId } 
                                               && "3|4".Contains(pev.ind_tipoEvento) 
                                               && pev.ind_estorno == "N"
                                         select pev.dt_ocorrencia).Max();

            // Verifica a situação do título (RN-001) 
            // 4-Título Liquidado ou 5-Título cancelado ou Null-Título em aberto
            if (r.vr_saldo_devedor == 0)
            {
                r.ind_baixa = (from pev in db.ContaPagarParcelaEventos
                               where new { pev.operacaoId, pev.parcelaId } == new { r.operacaoId, r.parcelaId }
                               && pev.dt_evento == (from pev1 in db.ContaPagarParcelaEventos
                                                    where new { pev1.operacaoId, pev1.parcelaId } == new { r.operacaoId, r.parcelaId } 
                                                    select pev1.dt_evento).Max()
                               select pev.ind_tipoEvento).FirstOrDefault();

                r.dt_baixa = (from pev in db.ContaPagarParcelaEventos
                              where new { pev.operacaoId, pev.parcelaId } == new { r.operacaoId, r.parcelaId } 
                              select pev.dt_evento).Max();
            }

            ContaPagarParcelaCrudModel contaPagarParcelaCrudModel = new ContaPagarParcelaCrudModel(this.db, this.seguranca_db);
            r = contaPagarParcelaCrudModel.Update(r);

            return r;
        }

        public IEnumerable<ContaPagarParcelaViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }
}