using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using App_Dominio.Contratos;
using System.IO;

namespace DWM.Models.Repositories
{
    public abstract class OperacaoParcelaEventoViewModel : Repository
    {
        [DisplayName("Operacao ID")]
        public int operacaoId { get; set; }

        [DisplayName("Parcela ID")]
        [Required(ErrorMessage = "Campo obrigatório: Nº Parcela")]
        public int parcelaId { get; set; }

        [DisplayName("Dt_Evento")]
        [Required(ErrorMessage = "Campo obrigatório: Dt.Evento")]
        public DateTime dt_evento { get; set; }

        [DisplayName("Evento")]
        [Required(ErrorMessage = "Campo obrigatório: Evento")]
        public int eventoId { get; set; }

        /// <summary>
        /// Código do evento 0-Inclusão de contas a pagar, etc.
        /// </summary>
        public int codigo { get; set; }

        public string descricao_evento { get; set; }

        [DisplayName("Contabilidade ID")]
        public System.Nullable<int> contabilidadeId { get; set; }

        public System.Nullable<int> enquadramentoId { get; set; }

        [DisplayName("Contabilidade")]
        public ContabilidadeViewModel Contabilidade { get; set; }

        [DisplayName("Movto_Bancario")]
        public System.Nullable<int> movtoBancarioId { get; set; }

        public MovtoBancarioViewModel MovtoBancario { get; set; }

        [DisplayName("Dt.Ocorrencia")]
        [Required(ErrorMessage = "Campo obrigatório: Dt.Ocorrência")]
        public DateTime dt_ocorrencia { get; set; }

        [DisplayName("Dt.Movimento")]
        [Required(ErrorMessage = "Campo obrigatório: Dt.Movimento")]
        public DateTime dt_movto { get; set; }

        [DisplayName("Valor")]
        public decimal valor { get; set; }

        [DisplayName("Arquivo")]
        [StringLength(100, ErrorMessage = "Nome do arquivo deve ter no máximo 100 caracteres")]
        public string arquivo { get; set; }

        public string arquivo_extensao
        {
            get
            {
                var path = System.Configuration.ConfigurationManager.AppSettings["Users_Data"];
                System.IO.FileInfo f = new System.IO.FileInfo(Path.Combine(path, arquivo));
                return f.Extension;
            }
        }

        public string ind_operacao { get; set; }

        [DisplayName("Estorno")]
        public string ind_estorno { get; set; }

        [DisplayName("Tipo Evento")]
        public string ind_tipoEvento { get; set; }

        public string descricao_tipoEvento
        {
            get
            {
                switch (ind_tipoEvento)
                {
                    case "0":
                        return "Inclusão de operação";
                    case "1":
                        return "Encargos";
                    case "2":
                        return "Desconto";
                    case "3":
                        return "Amortização";
                    case "4":
                        return "Baixa por motivo de liquidação";
                    case "5":
                        return "Baixa por motivo de cancelamento";
                    case "6":
                        return "Baixa por motivo de renegociação";
                    case "7":
                        return "Baixa por motivo de desconto";
                    case "9":
                        return "Estorno de lançamento";
                    default:
                        return "Não informado";
                };
            }
        }

        public OperacaoParcelaEventoViewModel()
        {
            mensagem = new Validate() { Code = 0, Message = "Evento incluído com sucesso" };
        }
    }
}