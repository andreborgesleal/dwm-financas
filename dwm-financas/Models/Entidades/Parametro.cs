using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("Parametro")]
    public class Parametro
    {
        [Key, Column(Order = 0)]
        [DisplayName("ID")]
        public int paramId { get; set; }

        [Key, Column(Order = 1)]
        [DisplayName("EmpresaID")]
        public int empresaId { get; set; }

        [DisplayName("Nome")]
        public string nome { get; set; }

        [DisplayName("Descrição")]
        public string descricao { get; set; }

        [DisplayName("Tipo")]
        public string tipo { get; set; }

        [DisplayName("Valor")]
        public string valor { get; set; }
    }
}