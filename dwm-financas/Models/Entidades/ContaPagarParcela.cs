using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("ContaPagarParcela")]
    public class ContaPagarParcela : OperacaoParcela<ContaPagarParcelaEvento>
    {
        [DisplayName("ind_autorizacao")]
        public string ind_autorizacao { get; set; }
    }
}