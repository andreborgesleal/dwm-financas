namespace DWM.Models.Repositories
{
    public class EditarContaPagarViewModel : EditarOperacaoViewModel<EditarContaPagarParcelaEventoViewModel>
    {
        public int credorId { get; set; }
        public string nome_credor { get; set; }
        public System.Nullable<int> grupoCredorId { get; set; }
        public string descricao_grupoCredor { get; set; }
    }
}