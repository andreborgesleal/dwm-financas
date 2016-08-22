using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("GrupoCliente")]
    public class GrupoCliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("ID")]
        public int grupoClienteId { get; set; }

        [DisplayName("EmpresaID")]
        public int empresaId { get; set; }

        [DisplayName("Nome")]
        public string nome { get; set; }
    }
}