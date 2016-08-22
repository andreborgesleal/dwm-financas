using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;
using App_Dominio.Contratos;
using System.Linq;

namespace DWM.Models.Repositories
{
    public class MovtoBancarioViewModel : Repository
    {
        [DisplayName("ID")]
        public int movtoBancarioId { get; set; }

        [DisplayName("Banco")]
        [Required(ErrorMessage="Campo obrigatório: Banco")]
        public int bancoId { get; set; }

        [DisplayName("Banco")]
        public string nome_banco { get; set; }

        [DisplayName("Histórico")]
        [Required(ErrorMessage="Campo obrigatório: Histórico")]
        public int historicoId { get; set; }

        [DisplayName("Histórico")]
        public string descricao_historico { get; set; }

        [DisplayName("Complemento")]
        public string complementoHist { get; set; }

        [DisplayName("Dt.Movimento")]
        [Required(ErrorMessage="Campo obrigatório: Data do movimento")]
        public DateTime dt_movto { get; set; }

        public string dt_movimento
        {
            get
            {
                return dt_movto.ToString("dd/MM/yyyy");
            }
        }

        [DisplayName("Valor")]
        [Required(ErrorMessage="Campo obrigatório: Valor")]
        public decimal valor { get; set; }

        [DisplayName("Documento")]
        public string documento { get; set; }

        [DisplayName("Tipo")]
        public string tipoMovto { get; set; }

        public string HasOperacao { get; set; }
    }

    public class ExtratoViewModel : MovtoBancarioViewModel, IReportRepository<ExtratoViewModel>
    {
        public int id { get; set; }
        public int _bancoid { get; set; }
        public DateTime? _dt_movto { get; set; }
        public decimal? vr_debito { get; set; }
        public decimal? vr_credito { get; set; }
        public decimal? vr_saldo { get; set; }

        #region métodos da Interface
        public object getValueColumn1()
        {
            return _dt_movto;
        }

        public object getValueColumn2()
        {
            return tipoMovto;
        }

        public void ClearColumn1()
        {
            _dt_movto = null;
        }

        public void ClearColumn2()
        {
            tipoMovto = null;
        }

        public ExtratoViewModel getKey(object group = null, object subGroup = null)
        {
            return new ExtratoViewModel() { _dt_movto = (DateTime?)group, tipoMovto = (string)subGroup };
        }

        public ExtratoViewModel Create(ExtratoViewModel key, IEnumerable<ExtratoViewModel> list)
        {
            ExtratoViewModel ext = new ExtratoViewModel();

            if (key.dt_movto == null && key.tipoMovto == "")
            {
                ext.vr_debito = list.Sum(m => m.vr_debito);
                ext.vr_credito = list.Sum(m => m.vr_credito);
                ext.descricao_historico = "<b>Total geral:</b> ";
            }
            else if (key.tipoMovto == "") // coluna 2 
            {
                ext.vr_debito = list.Where(info => info._dt_movto.Equals(key.dt_movto)).Sum(m => m.vr_debito);
                ext.vr_credito = list.Where(info => info._dt_movto.Equals(key.dt_movto)).Sum(m => m.vr_credito);
                ext.descricao_historico = "<b>Total da conta: </b>"; // grupo
            }
            else if (key.dt_movto != null && key.dt_movto > Convert.ToDateTime("1980-01-01")) // coluna 1 
            {
                ext.vr_debito = list.Where(info => info.tipoMovto.Equals(key.tipoMovto) && info._dt_movto == key.dt_movto).Sum(m => m.vr_debito);
                ext.vr_credito = list.Where(info => info.tipoMovto.Equals(key.tipoMovto) && info._dt_movto == key.dt_movto).Sum(m => m.vr_credito);
                ext.descricao_historico = "<b>Total do dia:</b> "; // sub-grupo
            }

            return ext;
        }

        #endregion
    }
}