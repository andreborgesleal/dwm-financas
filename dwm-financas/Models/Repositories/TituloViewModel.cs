using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;

namespace DWM.Models.Repositories
{
    public class TituloViewModel : Repository
    {
        [DisplayName("ID")]
        public int operacaoId { get; set; }

        [DisplayName("Parcela")]
        public int parcelaId { get; set; }

        [DisplayName("Sequencial")]
        public int SequencialID { get; set; }

        [DisplayName("Banco")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        [Required(ErrorMessage = "Banco deve ser informado")]
        public string BancoID { get; set; }

        [DisplayName("Convênio")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        [Required(ErrorMessage = "Convênio deve ser informado")]
        public string ConvenioID { get; set; }

        [DisplayName("Nº Remessa")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string RemessaID { get; set; }

        [DisplayName("Cliente")]
        [Required(ErrorMessage = "Cliente deve ser informado")]
        public int clienteId { get; set; }

        /// <summary>
        /// Identificação do título na empresa(vai no arquivo remessa e volta no arquivo retorno)
        /// Exemplo: "0000002501010002229102017"
        /// operacaoId(8 posições)
        /// parcelaId(2 posições)
        /// sequencialId(2 posições)
        /// clienteId(5 posições)
        /// Data Vencimento(8 posições formato AAAAMMDD)</summary>
        [DisplayName("TituloID")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string TituloID
        {
            get
            {
                return operacaoId.ToString().PadLeft(8, '0') +
                       parcelaId.ToString().PadLeft(2, '0') +
                       SequencialID.ToString().PadLeft(2, '0') +
                       clienteId.ToString().PadLeft(5, '0') +
                       DataVencimento.ToString("yyyyMMdd");
            }
        }

        [DisplayName("OcorrenciaID")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        [Required(ErrorMessage = "Ocorrência deve ser informada")]
        public string OcorrenciaID { get; set; }

        [DisplayName("NossoNumero")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string NossoNumero { get; set; }

        [DisplayName("NossoNumeroDV")]
        public string NossoNumeroDV { get; set; }

        [DisplayName("SeuNumero")]
        public string SeuNumero { get; set; }

        [DisplayName("DataVencimento")]
        public System.DateTime DataVencimento { get; set; }

        [DisplayName("Valor Nominal")]
        public System.Nullable<decimal> ValorPrincipal { get; set; }

        [DisplayName("Espécie")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        [Required(ErrorMessage = "Espécie do Título deve ser informada")]
        public string Especie { get; set; }

        [DisplayName("Aceite")]
        [Required(ErrorMessage = "Aceite deve ser informado")]
        public string Aceite { get; set; }

        [DisplayName("Data Emissão")]
        public System.DateTime DataEmissao { get; set; }

        [DisplayName("Data Juros")]
        public System.Nullable<System.DateTime> DataJuros { get; set; }

        [DisplayName("Data Desconto 1")]
        public System.Nullable<System.DateTime> DataDesconto1 { get; set; }

        [DisplayName("Valor Desconto 1")]
        public System.Nullable<decimal> ValorDesconto1 { get; set; }

        [DisplayName("Data Desconto 2")]
        public System.Nullable<System.DateTime> DataDesconto2 { get; set; }

        [DisplayName("Valor Desconto 2")]
        public System.Nullable<decimal> ValorDesconto2 { get; set; }

        [DisplayName("Data Desconto 3")]
        public System.Nullable<System.DateTime> DataDesconto3 { get; set; }

        [DisplayName("Valor Desconto 3")]
        public System.Nullable<decimal> ValorDesconto3 { get; set; }

        [DisplayName("Valor IOF")]
        public System.Nullable<decimal> ValorIOF { get; set; }

        [DisplayName("Valor Abatimento")]
        public System.Nullable<decimal> ValorAbatimento { get; set; }

        [DisplayName("Nº Dias p/ Devolução")]
        [Required(ErrorMessage = "Nº Dias p/ Devolução deve ser informado")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string NumDiasDevolucao { get; set; }

        [DisplayName("Multa ID")]
        [Required(ErrorMessage = "Tipo de cálculo da multa deve ser informado")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string MultaID { get; set; }

        [DisplayName("Data Multa")]
        public System.Nullable<System.DateTime> DataMulta { get; set; }

        [DisplayName("Valor Multa")]
        public System.Nullable<decimal> ValorMulta { get; set; }

        [DisplayName("Instrução Rodapé")]
        public string InstrucaoRodape { get; set; }
    }
}