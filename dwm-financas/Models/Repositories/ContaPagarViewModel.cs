using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DWM.Models.Repositories
{
    public class ContaPagarViewModel : OperacaoViewModel<ContaPagarParcelaViewModel, ContaPagarParcelaEventoViewModel>
    {
        [DisplayName("Fornecedor")]
        [Required(ErrorMessage = "Campo obrigatório: Fornecedor")]
        public int credorId { get; set; }

        public string nome_credor { get; set; }
        public string descricao_grupoCredor { get; set; }
    }
}