using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("Contabilidade")]
    public class Contabilidade
    {
        [Key]
        [DisplayName("ID")]
        public int contabilidadeId { get; set; }

        [DisplayName("EmpresaID")]
        public int empresaId { get; set; }

        [DisplayName("Exercicio")]
        public int exercicio { get; set; }

        [DisplayName("Dt_Lancamento")]
        public DateTime dt_lancamento { get; set; }

        [DisplayName("Documento")]
        public string documento { get; set; }

        public virtual ICollection<ContabilidadeItem> ContabilidadeItems { get; set; }
    }
}