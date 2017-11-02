using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;

namespace DWM.Models.Repositories
{
    public class ConvenioViewModel : Repository
    {
        [DisplayName("Banco")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string BancoID { get; set; }

        [DisplayName("Convênio")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string ConvenioID { get; set; }

        [DisplayName("Nome Banco")]
        [Required(ErrorMessage = "Nome do Banco deve ser informado")]
        public string NomeBanco { get; set; }

        [DisplayName("Agência")]
        [Required(ErrorMessage = "Agência deve ser informada")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string AgenciaID { get; set; }

        [DisplayName("Agência DV")]
        public string AgenciaDV { get; set; }

        [DisplayName("Conta")]
        [Required(ErrorMessage = "Conta deve ser informada")]
        public string ContaID { get; set; }

        [DisplayName("Conta DV")]
        public string ContaDV { get; set; }

        [DisplayName("Carteira")]
        [Required(ErrorMessage = "Carteira deve ser informada")]
        public string CarteiraID { get; set; }

        [DisplayName("Instrução 1")]
        [StringLength(40, ErrorMessage = "Campo Instrução deve possuir no máximo 40 caracteres")]
        public string Instrucao1 { get; set; }

        [DisplayName("Instrução 2")]
        [StringLength(40, ErrorMessage = "Campo Instrução deve possuir no máximo 40 caracteres")]
        public string Instrucao2 { get; set; }

        [DisplayName("Layout CNAB")]
        public string LayoutArquivo { get; set; }

        [DisplayName("Nosso Número Início")]
        [StringLength(8, ErrorMessage = "Campo [Nosso Número] deve possuir no máximo 8 caracteres")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string NossoNumeroInicio { get; set; }

        [DisplayName("Nosso Número Fim")]
        [StringLength(8, ErrorMessage = "Campo [Nosso Número] deve possuir no máximo 8 caracteres")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string NossoNumeroFim { get; set; }

        [DisplayName("Nosso Número")]
        [StringLength(8, ErrorMessage = "Campo [Nosso Número] deve possuir no máximo 8 caracteres")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string NossoNumeroUltimo { get; set; }
    }
}