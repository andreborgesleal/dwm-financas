using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("Cidade")]
    public class Cidade
    {
        [Key]
        [DisplayName("ID")]
        public int cidadeId { get; set; }

        [DisplayName("Nome")]
        public string nome { get; set; }
    }
}