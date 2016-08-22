using System.ComponentModel;
using App_Dominio.Component;
using System;
using System.ComponentModel.DataAnnotations;

namespace DWM.Models.Repositories
{
    public class CobrancaClienteViewModel : Repository
    {
        [DisplayName("ID")]
        public int cobrancaId { get; set; }

        [DisplayName("ClienteID")]
        public int clienteId { get; set; }

        public string nome_cliente { get; set; }

        [DisplayName("Dia_vencimento")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public Nullable<int> dia_vencimento { get; set; }

        [DisplayName("Mes_Dia")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public Nullable<int> mes_dia { get; set; }

        [DisplayName("Valor")]
        public Nullable<decimal> valor { get; set; }

        [DisplayName("Dt_Desativacao")]
        public Nullable<DateTime> dt_desativacao { get; set; }
    }
}