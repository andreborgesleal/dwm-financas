using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("ContaPagarParcela")]
    public class ContaPagarParcela : OperacaoParcela<ContaPagarParcelaEvento>
    {
    }
}