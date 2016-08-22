using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("ContaPagar")]
    public class ContaPagar : Operacao<ContaPagarParcela, ContaPagarParcelaEvento>
    {
        [DisplayName("Fornecedor")]
        public int credorId { get; set; }
    }
}