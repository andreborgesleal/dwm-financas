using App_Dominio.Component;
using App_Dominio.Contratos;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DWM.Models.Repositories
{
    public class RazaoViewModel : Repository, IReportRepository<RazaoViewModel>
    {
        public string codigoPleno { get; set; }
        public DateTime? dt_lancamento { get; set; }
        public string _codigoPleno { get; set; }
        public DateTime? _dt_lancamento { get; set; }
        public int? contabilidadeId { get; set; }
        public string documento { get; set; }
        public System.Nullable<int> sequencial { get; set; }
        public System.Nullable<int> centroCustoId { get; set; }
        public string descricao_centroCusto { get; set; }
        public string descricao_planoConta { get; set; }
        public System.Nullable<int> historicoId { get; set; }
        public string descricao_historico { get; set; }
        public string complementoHist { get; set; }
        public System.Nullable<decimal> vr_debito { get; set; }
        public System.Nullable<decimal> vr_credito { get; set; }
        public System.Nullable<decimal> vr_saldo { get; set; }

        #region métodos da Interface
        public object getValueColumn1()
        {
            return codigoPleno;
        }

        public object getValueColumn2()
        {
            return dt_lancamento;
        }

        public void ClearColumn1()
        {
            codigoPleno = null;
        }

        public void ClearColumn2()
        {
            dt_lancamento = null;
        }

        public RazaoViewModel getKey(object group = null, object subGroup = null)
        {
            return new RazaoViewModel() { codigoPleno = (string)group ?? "", dt_lancamento = (DateTime?)subGroup };
        }

        public RazaoViewModel Create(RazaoViewModel key, IEnumerable<RazaoViewModel> list)
        {
            RazaoViewModel raz = new RazaoViewModel();

            if (key.codigoPleno == "" && key.dt_lancamento == null)
            {
                raz.vr_debito = list.Sum(m => m.vr_debito);
                raz.vr_credito = list.Sum(m => m.vr_credito);
                raz.descricao_historico = "<b>Total geral:</b> ";
            }
            else if (key.dt_lancamento == null) // coluna 2 
            {
                raz.vr_debito = list.Where(info => info._codigoPleno.Equals(key.codigoPleno)).Sum(m => m.vr_debito);
                raz.vr_credito = list.Where(info => info._codigoPleno.Equals(key.codigoPleno)).Sum(m => m.vr_credito);
                raz.descricao_historico = "<b>Total da conta: </b>"; // grupo
            }
            else if (key.codigoPleno != "") // coluna 1 
            {
                raz.vr_debito = list.Where(info => info._dt_lancamento.Equals(key.dt_lancamento) && info._codigoPleno == key.codigoPleno).Sum(m => m.vr_debito);
                raz.vr_credito = list.Where(info => info._dt_lancamento.Equals(key.dt_lancamento) && info._codigoPleno == key.codigoPleno).Sum(m => m.vr_credito);
                raz.descricao_historico = "<b>Total do dia:</b> "; // sub-grupo
            }

            return raz;
        }

        #endregion
    }

    public class RazaoSinteticoViewModel : Repository
    {
        public string codigoPleno { get; set; }
        public string descricao { get; set; }
        public decimal vr_saldo_anterior { get; set; }
        public decimal tot_deb { get; set; }
        public decimal tot_cred { get; set; }
        public decimal vr_saldo_atual { get; set; }
    }
}