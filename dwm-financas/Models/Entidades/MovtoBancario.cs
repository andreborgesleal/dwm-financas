using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("MovtoBancario")]
    public class MovtoBancario
    {
        [Key]
        [DisplayName("ID")]
        public int movtoBancarioId { get; set; }

        [DisplayName("EmpresaID")]
        public int empresaId { get; set; }

        [DisplayName("Banco")]
        public int bancoId { get; set; }

        [DisplayName("Historico")]
        public int historicoId { get; set; }

        [DisplayName("Complemento")]
        public string complementoHist { get; set; }

        [DisplayName("Dt_Movimento")]
        public DateTime dt_movto { get; set; }

        [DisplayName("Valor")]
        public decimal valor { get; set; }

        [DisplayName("Documento")]
        public string documento { get; set; }

        [DisplayName("Tipo")]
        public string tipoMovto { get; set; }
    }
}