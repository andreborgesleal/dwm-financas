using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("ContaReceberParcela")]
    public class ContaReceberParcela : OperacaoParcela<ContaReceberParcelaEvento>
    {
    }
}