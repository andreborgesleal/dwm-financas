using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("EnquadramentoItem")]
    public class EnquadramentoItem
    {
        [Key, Column(Order = 0)]
        [DisplayName("EnquadramentoID")]
        public int enquadramentoId { get; set; }

        [Key, Column(Order = 1)]
        [DisplayName("Sequencial")]
        public int sequencial { get; set; }

        [DisplayName("CCusto")]
        public System.Nullable<int> centroCustoId { get; set; }

        [DisplayName("Conta")]
        public int planoContaId { get; set; }

        [DisplayName("Historico")]
        public int historicoId { get; set; }

        [DisplayName("Complemento")]
        public string complementoHist { get; set; }

        [DisplayName("Tipo")]
        public string tipoLancamento { get; set; }

        [DisplayName("Valor")]
        public Nullable<decimal> valor { get; set; }
    }
}