using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("Enquadramento")]
    public class Enquadramento
    {
        [Key]
        [DisplayName("EnquadramentoID")]
        public int enquadramentoId { get; set; }

        public int exercicio { get; set; }

        [DisplayName("Descricao")]
        public string descricao { get; set; }

        public int empresaId { get; set; }
        public virtual ICollection<EnquadramentoItem> EnquadramentoItems { get; set; }
    }
}