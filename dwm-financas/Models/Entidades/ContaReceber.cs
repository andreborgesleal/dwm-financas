using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("ContaReceber")]
    public class ContaReceber : Operacao<ContaReceberParcela, ContaReceberParcelaEvento>
    {
        [DisplayName("Cliente")]
        public int clienteId { get; set; }
    }
}