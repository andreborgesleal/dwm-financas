using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;

namespace DWM.Models.Repositories
{
    public class ConvenioViewModel : Repository
    {
        [DisplayName("BancoID")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string BancoID { get; set; }

        [DisplayName("ConvenioID")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string ConvenioID { get; set; }

        [DisplayName("NomeBanco")]
        [Required(ErrorMessage = "Nome do Banco deve ser informado")]
        public string NomeBanco { get; set; }

        [DisplayName("AgenciaID")]
        [Required(ErrorMessage = "Agência deve ser informada")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string AgenciaID { get; set; }

        [DisplayName("AgenciaDV")]
        public string AgenciaDV { get; set; }

        [DisplayName("ContaID")]
        [Required(ErrorMessage = "Conta deve ser informada")]
        public string ContaID { get; set; }

        [DisplayName("ContaDV")]
        public string ContaDV { get; set; }

        [DisplayName("CarteiraID")]
        [Required(ErrorMessage = "Carteira deve ser informada")]
        public string CarteiraID { get; set; }

        [DisplayName("Instrucao1")]
        [StringLength(40, ErrorMessage = "Campo Instrução deve possuir no máximo 40 caracteres")]
        public string Instrucao1 { get; set; }

        [DisplayName("Instrucao2")]
        [StringLength(40, ErrorMessage = "Campo Instrução deve possuir no máximo 40 caracteres")]
        public string Instrucao2 { get; set; }

        [DisplayName("LayoutArquivo")]
        public string LayoutArquivo { get; set; }

        [DisplayName("NossoNumeroInicio")]
        [StringLength(40, ErrorMessage = "Campo [Nosso Número] deve possuir no máximo 8 caracteres")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string NossoNumeroInicio { get; set; }

        [DisplayName("NossoNumeroFim")]
        [StringLength(40, ErrorMessage = "Campo [Nosso Número] deve possuir no máximo 8 caracteres")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string NossoNumeroFim { get; set; }

        [DisplayName("NossoNumeroUltimo")]
        [StringLength(40, ErrorMessage = "Campo [Nosso Número] deve possuir no máximo 8 caracteres")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string NossoNumeroUltimo { get; set; }
    }
}