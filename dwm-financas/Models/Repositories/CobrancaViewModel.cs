using System.ComponentModel;
using App_Dominio.Component;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DWM.Models.Repositories
{
    public class CobrancaViewModel : Repository
    {
        [DisplayName("ID")]
        public int cobrancaId { get; set; }

        [DisplayName("Grupo Cobrança")]
        public int grupoCobrancaId { get; set; }

        public string descricao_grupoCobranca { get; set; }

        [DisplayName("Histórico")]
        public int historicoId { get; set; }

        public string descricao_historico { get; set; }

        [DisplayName("Banco")]
        public int bancoId { get; set; }

        public string nome_banco { get; set; }

        [DisplayName("Enquadramento")]
        public int enquadramentoId { get; set; }

        public string descricao_enquadramento { get; set; }

        [DisplayName("Dt.Início")]
        public DateTime dt_inicio { get; set; }

        [DisplayName("Dt.Fim")]
        public Nullable<DateTime> dt_fim { get; set; }

        [DisplayName("Nº Parcelas")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public int num_parcelas { get; set; }

        [DisplayName("Dia Vencimento")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public int dia_vencimento { get; set; }

        [DisplayName("Mês Dia")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public int mes_dia { get; set; }

        [DisplayName("Valor")]
        public decimal valor { get; set; }

        [DisplayName("Juros")]
        public decimal vr_jurosMora { get; set; }

        [DisplayName("Multa")]
        public decimal vr_multa { get; set; }

        public virtual CobrancaClienteViewModel CobrancaClienteViewModel { get; set; }

        public virtual IEnumerable<CobrancaClienteViewModel> CobrancaClientes { get; set; }

        public int grupoClienteId { get; set; }

        public int total_clientes { get; set; }

        public IPagedList pagedList { get; set; }
    }
}