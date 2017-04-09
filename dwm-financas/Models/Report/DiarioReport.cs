using App_Dominio.Component;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DWM.Models.Report
{
    public class DiarioReport : ReportViewModel<DiarioViewModel>
    {
        #region Métodos da classe ReportRepository
        public override IEnumerable<DiarioViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            int? centroCustoId = (int?)param[2];
            DateTime dt1 = Convert.ToDateTime(param[0].ToString());
            DateTime dt2 = Convert.ToDateTime(param[1].ToString());

            //int _exercicio = int.Parse(sessaoCorrente.value1);
            int _exercicio = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EXERCICIO_CONTABIL, sessaoCorrente.empresaId).valor);

            totalizaColuna1 = param[3].ToString();
            totalizaColuna2 = param[4].ToString();

            System.Nullable<decimal> nulo = null;

            #region LINQ
            var q = (from c in db.Contabilidades
                     join i in db.ContabilidadeItems on c.contabilidadeId equals i.contabilidadeId
                     join ccu in db.CentroCustos on i.centroCustoId equals ccu.centroCustoId
                     join pc in db.PlanoContas on i.planoContaId equals pc.planoContaId
                     join his in db.Historicos on i.historicoId equals his.historicoId
                     where c.empresaId.Equals(sessaoCorrente.empresaId)
                            && c.exercicio == _exercicio
                            && c.dt_lancamento >= dt1 && c.dt_lancamento <= dt2
                            && (centroCustoId == null || i.centroCustoId == centroCustoId)
                     orderby c.dt_lancamento, c.contabilidadeId, i.sequencial
                     select new DiarioViewModel
                     {
                         contabilidadeId = c.contabilidadeId,
                         _contabilidadeId = c.contabilidadeId,
                         empresaId = c.empresaId,
                         dt_lancamento = c.dt_lancamento,
                         _dt_lancamento = c.dt_lancamento,
                         documento = c.documento,
                         sequencial = i.sequencial,
                         centroCustoId = i.centroCustoId.Value,
                         descricao_centroCusto = ccu.descricao,
                         codigoPleno = pc.codigoPleno,
                         descricao_planoConta = pc.descricao,
                         historicoId = i.historicoId,
                         descricao_historico = his.descricao,
                         complementoHist = i.complementoHist,
                         vr_debito = i.tipoLancamento == "D" ? i.valor : nulo,
                         vr_credito = i.tipoLancamento == "C" ? i.valor : nulo,
                         PageSize = pageSize,
                         TotalCount = (from c1 in db.Contabilidades
                                       join i1 in db.ContabilidadeItems on c1.contabilidadeId equals i1.contabilidadeId
                                       where c1.empresaId.Equals(sessaoCorrente.empresaId)
                                             && c1.exercicio == _exercicio
                                             && c1.dt_lancamento >= dt1 && c.dt_lancamento <= dt2
                                             && (centroCustoId == null || i1.centroCustoId == centroCustoId)
                                       select c1).Count()
                     }).ToList();
            #endregion

            return q;
        }

        public override Repository getRepository(Object id)
        {
            throw new NotImplementedException();
        }

        public override string action()
        {
            return "ListParam";
        }

        public override string DivId()
        {
            return "div-list-static";
        }

        public override IEnumerable<DiarioViewModel> BindReport(params object[] param)
        {
            int? centroCustoId = (int?)param[2];
            DateTime dt1 = Convert.ToDateTime(param[0].ToString());
            DateTime dt2 = Convert.ToDateTime(param[1].ToString());

            //int _exercicio = int.Parse(sessaoCorrente.value1);
            int _exercicio = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EXERCICIO_CONTABIL, sessaoCorrente.empresaId).valor);

            totalizaColuna1 = param[3].ToString();
            totalizaColuna2 = param[4].ToString();

            System.Nullable<decimal> nulo = null;

            #region LINQ
            var q = (from c in db.Contabilidades
                     join i in db.ContabilidadeItems on c.contabilidadeId equals i.contabilidadeId
                     join ccu in db.CentroCustos on i.centroCustoId equals ccu.centroCustoId
                     join pc in db.PlanoContas on i.planoContaId equals pc.planoContaId
                     join his in db.Historicos on i.historicoId equals his.historicoId
                     where c.empresaId.Equals(sessaoCorrente.empresaId)
                            && c.exercicio == _exercicio
                            && c.dt_lancamento >= dt1 && c.dt_lancamento <= dt2
                            && (centroCustoId == null || i.centroCustoId == centroCustoId)
                     orderby c.dt_lancamento, c.contabilidadeId, i.sequencial
                     select new DiarioViewModel
                     {
                         contabilidadeId = c.contabilidadeId,
                         _contabilidadeId = c.contabilidadeId,
                         empresaId = c.empresaId,
                         dt_lancamento = c.dt_lancamento,
                         _dt_lancamento = c.dt_lancamento,
                         documento = c.documento,
                         sequencial = i.sequencial,
                         centroCustoId = i.centroCustoId.Value,
                         descricao_centroCusto = ccu.descricao,
                         codigoPleno = pc.codigoPleno,
                         descricao_planoConta = pc.descricao,
                         historicoId = i.historicoId,
                         descricao_historico = his.descricao,
                         complementoHist = i.complementoHist,
                         vr_debito = i.tipoLancamento == "D" ? i.valor : nulo,
                         vr_credito = i.tipoLancamento == "C" ? i.valor : nulo
                     }).ToList();
            #endregion

            return q;
        }
        #endregion
    }
}