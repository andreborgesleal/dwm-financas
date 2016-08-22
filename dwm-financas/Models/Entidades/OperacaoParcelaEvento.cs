using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    public abstract class OperacaoParcelaEvento
    {
        [Key, Column(Order = 0)]
        [DisplayName("OperacaoID")]
        public int operacaoId { get; set; }

        [Key, Column(Order = 1)]
        [DisplayName("ParcelaID")]
        public int parcelaId { get; set; }

        [Key, Column(Order = 2)]
        [DisplayName("Dt_Evento")]
        public DateTime dt_evento { get; set; }

        [DisplayName("Evento")]
        public int eventoId { get; set; }

        [DisplayName("Contabilidade")]
        public virtual Contabilidade Contabilidade { get; set; }

        public System.Nullable<int> contabilidadeId { get; set; }

        [DisplayName("Movto_Bancario")]
        public virtual MovtoBancario MovtoBancario { get; set; }

        public System.Nullable<int> movtoBancarioId { get; set; }

        [DisplayName("Dt_Ocorrencia")]
        public DateTime dt_ocorrencia { get; set; }

        [DisplayName("Valor")]
        public decimal valor { get; set; }

        [DisplayName("Arquivo")]
        public string arquivo { get; set; }

        [DisplayName("Ind_Operacao")]
        public string ind_operacao { get; set; }

        [DisplayName("Ind_Estorno")]
        public string ind_estorno { get; set; }

        [DisplayName("Ind_TipoEvento")]
        public string ind_tipoEvento { get; set; }
    }
}