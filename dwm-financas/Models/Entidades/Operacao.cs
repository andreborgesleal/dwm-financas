using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DWM.Models.Entidades
{
    public abstract class Operacao<OP, OPE>
        where OP : OperacaoParcela<OPE>
        where OPE : OperacaoParcelaEvento
    {
        [Key]
        [DisplayName("ID")]
        public int operacaoId { get; set; }

        [DisplayName("EmpresaID")]
        public int empresaId { get; set; }

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

        public virtual ICollection<OP> OperacaoParcelas { get; set; }
    }
}