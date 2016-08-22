using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("Historico")]
    public class Historico
    {
        [Key]
        [DisplayName("ID")]
        public int historicoId { get; set; }

        [DisplayName("EmpresaID")]
        public int empresaId { get; set; }

        [DisplayName("Descricao")]
        public string descricao { get; set; }

        [DisplayName("Ind_TipoHistorico")]
        public string ind_tipoHistorico { get; set; }
    }
}