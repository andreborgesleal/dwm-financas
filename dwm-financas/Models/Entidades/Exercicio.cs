using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("Exercicio")]
    public class Exercicio
    {
        [Key, Column(Order = 0)]
        [DisplayName("EmpresaID")]
        public int empresaId { get; set; }

        [Key, Column(Order = 1)]
        [DisplayName("Exercicio")]
        public int exercicio { get; set; }

        [DisplayName("Dt_Inicio")]
        public System.DateTime dt_inicio { get; set; }

        [DisplayName("Dt_Fim")]
        public System.DateTime dt_fim { get; set; }

        [DisplayName("MascaraPC")]
        public string mascaraPc { get; set; }

        [DisplayName("Encerrado")]
        public string encerrado { get; set; }
    }
}