using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("CobrancaCliente")]
    public class CobrancaCliente
    {
        [Key, Column(Order = 0)]
        [DisplayName("ID")]
        public int cobrancaId { get; set; }

        [Key, Column(Order = 1)]
        [DisplayName("ClienteID")]
        public int clienteId { get; set; }

        [DisplayName("Dia Vencimento")]
        public Nullable<int> dia_vencimento { get; set; }

        [DisplayName("Mês Dia")]
        public Nullable<int> mes_dia { get; set; }

        [DisplayName("Valor")]
        public Nullable<decimal> valor { get; set; }

        [DisplayName("Dt_Desativacao")]
        public Nullable<DateTime> dt_desativacao { get; set; }
    }
}