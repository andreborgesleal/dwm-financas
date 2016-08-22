using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("ContaPagar")]
    public class _ContaPagar
    {
        [Key]
        [DisplayName("ID")]
        public int operacaoId { get; set; }

        [DisplayName("EmpresaID")]
        public int empresaId { get; set; }

        [DisplayName("Credor")]
        public int credorId { get; set; }

        [DisplayName("Historico")]
        public int historicoId { get; set; }

        [DisplayName("ContabilidadeID")]
        public System.Nullable<int> contabilidadeId { get; set; }

        [DisplayName("Contabilidade")]
        public virtual Contabilidade Contabilidade { get; set; }

        [DisplayName("CentroCusto")]
        public System.Nullable<int> centroCustoId { get; set; }

        [DisplayName("ComplementoHist")]
        public string complementoHist { get; set; }

        [DisplayName("Dt_Emissao")]
        public DateTime dt_emissao { get; set; }

        [DisplayName("Juros_Mora")]
        public System.Nullable<decimal> vr_jurosMora { get; set; }

        [DisplayName("Vr_Multa")]
        public System.Nullable<decimal> vr_multa { get; set; }

        [DisplayName("Documento")]
        public string documento { get; set; }

        [DisplayName("Recorrencia")]
        public string recorrencia { get; set; }

        public virtual ICollection<ContaPagarParcela> ContaPagarParcelas { get; set; }
    }
}