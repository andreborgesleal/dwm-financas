using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("CentroCusto")]
    public class CentroCusto
    {
        [Key]
        [DisplayName("ID")]
        public int centroCustoId { get; set; }

        [DisplayName("EmpresaID")]
        public int empresaId { get; set; }

        [DisplayName("Descricao")]
        public string descricao { get; set; }
    }
}