using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("ContaPagarParcela")]
    public class _ContaPagarParcela
    {
        [Key, Column(Order = 0)]
        [DisplayName("OperacaoID")]
        public int operacaoId { get; set; }

        [Key, Column(Order = 1)]
        [DisplayName("ParcelaID")]
        public int parcelaId { get; set; }

        [DisplayName("BancoID")]
        public System.Nullable<int> bancoId { get; set; }

        [DisplayName("Num_Titulo")]
        public string num_titulo { get; set; }

        [DisplayName("Dt_Vencimento")]
        public DateTime dt_vencimento { get; set; }

        [DisplayName("Vr_Principal")]
        public decimal vr_principal { get; set; }

        [DisplayName("Vr_amortizacao")]
        public System.Nullable<decimal> vr_amortizacao { get; set; }

        [DisplayName("Vr_desconto")]
        public System.Nullable<decimal> vr_desconto { get; set; }

        [DisplayName("Vr_Encargos")]
        public System.Nullable<decimal> vr_encargos { get; set; }

        [DisplayName("Vr_Total_pago")]
        public System.Nullable<decimal> vr_total_pago { get; set; }

        [DisplayName("Vr_Saldo_Devedor")]
        public System.Nullable<decimal> vr_saldo_devedor { get; set; }

        [DisplayName("Ind_Forma_Pagamento")]
        public string ind_forma_pagamento { get; set; }

        [DisplayName("Codigo_Barras")]
        public string codigo_barras { get; set; }

        [DisplayName("Ultima_Amortizacao")]
        public System.Nullable<DateTime> dt_ultima_amortizacao { get; set; }

        [DisplayName("Ind_Baixa")]
        public string ind_baixa { get; set; }

        [DisplayName("Dt_Baixa")]
        public System.Nullable<DateTime> dt_baixa { get; set; }

        [DisplayName("Cheque_Banco")]
        public System.Nullable<int> cheque_banco { get; set; }

        [DisplayName("Cheque_Agencia")]
        public string cheque_agencia { get; set; }

        [DisplayName("Cheque_Numero")]
        public string cheque_numero { get; set; }

        public virtual ICollection<ContaPagarParcelaEvento> ContaPagarParcelaEventos { get; set; }
    }
}