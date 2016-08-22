namespace DWM.Models.Repositories
{
    public class EditarContaReceberViewModel : EditarOperacaoViewModel<EditarContaReceberParcelaEventoViewModel>
    {
        public int clienteId { get; set; }
        public string nome_cliente { get; set; }
        public System.Nullable<int> grupoClienteId { get; set; }
        public string descricao_grupoCliente { get; set; }
    }
}