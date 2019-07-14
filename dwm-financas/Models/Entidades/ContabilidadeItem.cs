using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("ContabilidadeItem")]
    public class ContabilidadeItem
    {
        [Key, Column(Order = 0)]
        [DisplayName("ID")]
        public int contabilidadeId { get; set; }

        [Key, Column(Order = 1)]
        [DisplayName("Sequencial")]
        public int sequencial { get; set; }

        [DisplayName("CCusto")]
        public System.Nullable<int> centroCustoId { get; set; }

        [DisplayName("Conta_Contabil")]
        public int planoContaId { get; set; }

        [DisplayName("Historico")]
        public int historicoId { get; set; }

        [DisplayName("Complemento")]
        public string complementoHist { get; set; }

        [DisplayName("Tipo")]
        public string tipoLancamento { get; set; }

        [DisplayName("Valor")]
        public decimal valor { get; set; }

        [DisplayName("Documento")]
        public string DocumentoURL { get; set; }
    }
}