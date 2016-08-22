using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("EmpresaInterna")]
    public class EmpresaInterna
    {
        [Key]
        [DisplayName("ID")]
        public int empresaId { get; set; }

        [DisplayName("Cidade")]
        public int cidadeId { get; set; }

        [DisplayName("Nome")]
        public string nome { get; set; }

        [DisplayName("email")]
        public string email { get; set; }
    }
}