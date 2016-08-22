using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;

namespace DWM.Models.Repositories
{
    public class _ContaPagarViewModel : Repository
    {
        [DisplayName("ID")]
        public int operacaoId { get; set; }

        [DisplayName("Credor")]
        [Required(ErrorMessage="Campo obrigatório: Credor")]
        public int credorId { get; set; }

        public string nome_credor { get; set; }
        public string descricao_grupoCredor { get; set; }

        [DisplayName("Histórico")]
        [Required(ErrorMessage = "Campo obrigatório: Histórico")]
        public int historicoId { get; set; }

        public string descricao_historico { get; set; }

        [DisplayName("Contabilidade")]
        public ContabilidadeViewModel Contabilidade { get; set; }

        [DisplayName("ContabilidadeID")]
        public System.Nullable<int> contabilidadeId { get; set; }

        public System.Nullable<int> enquadramentoId { get; set; }

        public string descricao_enquadramento { get; set; }

        [DisplayName("Centro de Custo")]
        public System.Nullable<int> centroCustoId { get; set; }

        public string descricao_centroCusto { get; set; }

        [DisplayName("Complemento")]
        [StringLength(300, ErrorMessage = "Complemento do histórico dever possuir no máximo 300 caracteres")]
        public string complementoHist { get; set; }

        [DisplayName("Dt.Emissão")]
        [Required(ErrorMessage = "Campo obrigatório: Dt. Emissão")]
        public DateTime dt_emissao { get; set; }

        [DisplayName("Juros diário de Mora (%)")]
        public System.Nullable<decimal> vr_jurosMora { get; set; }

        [DisplayName("Multa por atraso (%)")]
        public System.Nullable<decimal> vr_multa { get; set; }

        [DisplayName("Documento")]
        public string documento { get; set; }

        [DisplayName("Recorrência")]
        public string recorrencia { get; set; }

        public bool recorrencia_mensal { get; set; }

        public DateTime? dt_movto { get; set; }

        public int? enquadramento_amortizacaoId { get; set; }

        public string descricao_enquadramentoAmortizacao { get; set; }

        public string fileBoleto { get; set; }

        public string fileComprovante { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public int num_parcelas { get; set; }

        public virtual ContaPagarParcelaViewModel ContaPagarParcela { get; set; }

        public virtual IEnumerable<ContaPagarParcelaViewModel> ContaPagarParcelas { get; set; }
    }
}