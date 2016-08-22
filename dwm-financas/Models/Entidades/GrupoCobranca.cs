using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("GrupoCobranca")]
    public class GrupoCobranca
    {
        [Key]
        [DisplayName("ID")]
        public int grupoCobrancaId { get; set; }

        [DisplayName("Empresa")]
        public int empresaId { get; set; }

        [DisplayName("Descricao")]
        public string descricao { get; set; }
    }
}