using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("Titulo")]
    public class Titulo
    {
        [Key, Column(Order = 0)]
        [DisplayName("operacaoId")]
        public int operacaoId { get; set; }

        [Key, Column(Order = 1)]
        [DisplayName("parcelaId")]
        public int parcelaId { get; set; }

        [Key, Column(Order = 2)]
        [DisplayName("SequencialID")]
        public int SequencialID { get; set; }

        [DisplayName("BancoID")]
        public string BancoID { get; set; }

        [DisplayName("ConvenioID")]
        public string ConvenioID { get; set; }

        [DisplayName("RemessaID")]
        public string RemessaID { get; set; }

        [DisplayName("empresaId")]
        public int empresaId { get; set; }

        [DisplayName("ClienteId")]
        public int clienteId { get; set; }

        [DisplayName("TituloID")]
        public string TituloID { get; set; }

        [DisplayName("OcorrenciaID")]
        public string OcorrenciaID { get; set; }

        [DisplayName("NossoNumero")]
        public string NossoNumero { get; set; }

        [DisplayName("NossoNumeroDV")]
        public string NossoNumeroDV { get; set; }

        [DisplayName("SeuNumero")]
        public string SeuNumero { get; set; }

        [DisplayName("DataVencimento")]
        public System.Nullable<System.DateTime> DataVencimento { get; set; }

        [DisplayName("ValorPrincipal")]
        public System.Nullable<decimal> ValorPrincipal { get; set; }

        [DisplayName("Especie")]
        public string Especie { get; set; }

        [DisplayName("Aceite")]
        public string Aceite { get; set; }

        [DisplayName("DataEmissao")]
        public System.DateTime DataEmissao { get; set; }

        [DisplayName("DataJuros")]
        public System.Nullable<System.DateTime> DataJuros { get; set; }

        [DisplayName("DataDesconto1")]
        public System.Nullable<System.DateTime> DataDesconto1 { get; set; }

        [DisplayName("ValorDesconto1")]
        public System.Nullable<decimal> ValorDesconto1 { get; set; }

        [DisplayName("DataDesconto2")]
        public System.Nullable<System.DateTime> DataDesconto2 { get; set; }

        [DisplayName("ValorDesconto2")]
        public System.Nullable<decimal> ValorDesconto2 { get; set; }

        [DisplayName("DataDesconto3")]
        public System.Nullable<System.DateTime> DataDesconto3 { get; set; }

        [DisplayName("ValorDesconto3")]
        public System.Nullable<decimal> ValorDesconto3 { get; set; }

        [DisplayName("ValorIOF")]
        public System.Nullable<decimal> ValorIOF { get; set; }

        [DisplayName("ValorAbatimento")]
        public System.Nullable<decimal> ValorAbatimento { get; set; }

        [DisplayName("NumDiasDevolucao")]
        public string NumDiasDevolucao { get; set; }

        [DisplayName("MultaID")]
        public string MultaID { get; set; }

        [DisplayName("DataMulta")]
        public System.Nullable<System.DateTime> DataMulta { get; set; }

        [DisplayName("ValorMulta")]
        public System.Nullable<decimal> ValorMulta { get; set; }

        [DisplayName("InstrucaoRodape")]
        public string InstrucaoRodape { get; set; }
    }
}
