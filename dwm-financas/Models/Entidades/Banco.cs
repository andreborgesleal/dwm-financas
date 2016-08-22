using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("Banco")]
    public class Banco
    {
        [Key]
        [DisplayName("ID")]
        public int bancoId { get; set; }

        [DisplayName("Empresa")]
        public int empresaId { get; set; }

        [DisplayName("Nome")]
        public string nome { get; set; }

        [DisplayName("Classificacao")]
        public string classificacao { get; set; }

        [DisplayName("Sigla")]
        public string sigla { get; set; }

        [DisplayName("Numero")]
        public string numero { get; set; }
    }
}