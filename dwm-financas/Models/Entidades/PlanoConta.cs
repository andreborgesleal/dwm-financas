using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("PlanoConta")]
    public class PlanoConta
    {
        [Key]
        [DisplayName("ID")]
        public int planoContaId { get; set; }

        [DisplayName("EmpresaID")]
        public int empresaId { get; set; }

        [DisplayName("Conta_pai")]
        public System.Nullable<int> planoContaId_pai { get; set; }

        [DisplayName("Codigo_Pleno")]
        public string codigoPleno { get; set; }

        [DisplayName("Descricao")]
        public string descricao { get; set; }

        [DisplayName("Tipo_Conta")]
        public string tipoConta { get; set; }

        [DisplayName("Saldo_Inicial")]
        public System.Nullable<decimal> vr_saldo_inicial { get; set; }

        [DisplayName("Exercicio")]
        public int exercicio { get; set; }

        [DisplayName("Codigo_Reduzido")]
        public int codigoReduzido { get; set; }
    }
}