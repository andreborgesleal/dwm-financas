using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("Convenio")]
    public class Convenio
    {
        [Key, Column(Order = 0)]
        [DisplayName("BancoID")]
        public string BancoID { get; set; }

        [Key, Column(Order = 1)]
        [DisplayName("ConvenioID")]
        public string ConvenioID { get; set; }

        [DisplayName("empresaId")]
        public int empresaId { get; set; }

        [DisplayName("NomeBanco")]
        public string NomeBanco { get; set; }

        [DisplayName("AgenciaID")]
        public string AgenciaID { get; set; }

        [DisplayName("AgenciaDV")]
        public string AgenciaDV { get; set; }

        [DisplayName("ContaID")]
        public string ContaID { get; set; }

        [DisplayName("ContaDV")]
        public string ContaDV { get; set; }

        [DisplayName("CarteiraID")]
        public string CarteiraID { get; set; }

        [DisplayName("Instrucao1")]
        public string Instrucao1 { get; set; }

        [DisplayName("Instrucao2")]
        public string Instrucao2 { get; set; }

        [DisplayName("LayoutArquivo")]
        public string LayoutArquivo { get; set; }

        [DisplayName("NossoNumeroInicio")]
        public string NossoNumeroInicio { get; set; }

        [DisplayName("NossoNumeroFim")]
        public string NossoNumeroFim { get; set; }

        [DisplayName("NossoNumeroUltimo")]
        public string NossoNumeroUltimo { get; set; }
    }
}