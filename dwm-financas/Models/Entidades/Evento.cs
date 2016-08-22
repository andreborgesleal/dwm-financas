using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("Evento")]
    public class Evento
    {
        [Key]
        [DisplayName("ID")]
        public int eventoId { get; set; }

        [DisplayName("EmpresaID")]
        public int empresaId { get; set; }

        [DisplayName("Codigo")]
        public int codigo { get; set; }

        [DisplayName("Descricao")]
        public string descricao { get; set; }

        [DisplayName("Ind_tipoEvento")]
        public string ind_tipoEvento { get; set; }

        [DisplayName("Ind_eventoFixo")]
        public string ind_eventoFixo { get; set; }

        [DisplayName("Ind_operacao")]
        public string ind_operacao { get; set; }

        [DisplayName("Ind_modalidade")]
        public string ind_modalidade { get; set; }

    }
}