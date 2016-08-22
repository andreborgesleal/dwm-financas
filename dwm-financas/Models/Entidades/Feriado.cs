using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("Feriado")]
    public class Feriado
    {
        [Key]
        [DisplayName("ID")]
        public int feriadoId { get; set; }

        [DisplayName("CidadeID")]
        public Nullable<int> cidadeId { get; set; }

        [DisplayName("Descrição")]
        public string descricao { get; set; }

        [DisplayName("Data do Feriado")]
        public DateTime dt_feriado { get; set; }
    }
}