using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("Cobranca")]
    public class Cobranca
    {
        [Key]
        [DisplayName("ID")]
        public int cobrancaId { get; set; }

        [DisplayName("GrupoCobrancaID")]
        public int grupoCobrancaId { get; set; }

        [DisplayName("Empresa")]
        public int empresaId { get; set; }

        [DisplayName("HistoricoID")]
        public int historicoId { get; set; }

        [DisplayName("BancoID")]
        public int bancoId { get; set; }

        [DisplayName("EnquadramentoID")]
        public int enquadramentoId { get; set; }

        [DisplayName("Dt_Inicio")]
        public DateTime dt_inicio { get; set; }

        [DisplayName("Dt_Fim")]
        public Nullable<DateTime> dt_fim { get; set; }

        [DisplayName("Num_Parcelas")]
        public int num_parcelas { get; set; }

        [DisplayName("Dia_Vencimento")]
        public int dia_vencimento { get; set; }

        [DisplayName("Mes_Dia")]
        public int mes_dia { get; set; }

        [DisplayName("Valor")]
        public decimal valor { get; set; }

        [DisplayName("Vr_JurosMora")]
        public decimal vr_jurosMora { get; set; }

        [DisplayName("Vr_Multa")]
        public decimal vr_multa { get; set; }

        public virtual ICollection<CobrancaCliente> CobrancaClientes { get; set; }
    }
}