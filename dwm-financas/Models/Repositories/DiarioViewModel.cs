using App_Dominio.Component;
using App_Dominio.Contratos;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DWM.Models.Repositories
{
    public class DiarioViewModel : Repository, IReportRepository<DiarioViewModel>
    {
        public int? contabilidadeId { get; set; }
        public DateTime? dt_lancamento { get; set; }
        public int _contabilidadeId { get; set; }
        public DateTime _dt_lancamento { get; set; }
        public string documento { get; set; }
        public int sequencial { get; set; }
        public System.Nullable<int> centroCustoId { get; set; }
        public string descricao_centroCusto { get; set; }
        public string codigoPleno { get; set; }
        public string descricao_planoConta { get; set; }
        public int historicoId { get; set; }
        public string descricao_historico { get; set; }
        public string complementoHist { get; set; }
        public System.Nullable<decimal> vr_debito { get; set; }
        public System.Nullable<decimal> vr_credito { get; set; }
        public string DocumentoURL { get; set; }
        //public IEnumerable<Repository> r { get; set; }

        #region métodos da Interface
        public object getValueColumn1()
        {
            return dt_lancamento;
        }

        public object getValueColumn2()
        {
            return contabilidadeId;
        }

        public void ClearColumn1()
        {
            dt_lancamento = null;
        }

        public void ClearColumn2()
        {
            contabilidadeId = null;
        }

        public DiarioViewModel getKey(object group = null, object subGroup = null)
        {
            return new DiarioViewModel() { dt_lancamento = (DateTime?)group, contabilidadeId = (int?)subGroup };
        }

        public DiarioViewModel Create(DiarioViewModel key, IEnumerable<DiarioViewModel> list)
        {
            DiarioViewModel d = new DiarioViewModel();

            if (key.contabilidadeId == null && key.dt_lancamento == null)
            {
                d.vr_debito = list.Sum(m => m.vr_debito);
                d.vr_credito = list.Sum(m => m.vr_credito);
                d.descricao_historico = "<b>Total geral:</b> ";
            }
            else if (key.contabilidadeId == null) // coluna 2 
            {
                d.vr_debito = list.Where(info => info._dt_lancamento.Equals(key.dt_lancamento)).Sum(m => m.vr_debito);
                d.vr_credito = list.Where(info => info._dt_lancamento.Equals(key.dt_lancamento)).Sum(m => m.vr_credito);
                d.descricao_historico = "<b>Total do dia: </b>"; // grupo
            }
            else if (key.dt_lancamento != null) // coluna 1 
            {
                d.vr_debito = list.Where(info => info._dt_lancamento.Equals(key.dt_lancamento) && info._contabilidadeId == key.contabilidadeId).Sum(m => m.vr_debito);
                d.vr_credito = list.Where(info => info._dt_lancamento.Equals(key.dt_lancamento) && info._contabilidadeId == key.contabilidadeId).Sum(m => m.vr_credito);
                d.descricao_historico = "<b>Total do lançamento:</b> "; // sub-grupo
            }

            return d;
        }

        #endregion

    }
}