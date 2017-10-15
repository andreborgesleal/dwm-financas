namespace DWM.Models.Repositories
{
    public class EditarContaReceberViewModel : EditarOperacaoViewModel<EditarContaReceberParcelaEventoViewModel>
    {
        public int clienteId { get; set; }
        public string nome_cliente { get; set; }
        public System.Nullable<int> grupoClienteId { get; set; }
        public string descricao_grupoCliente { get; set; }
        public string cpf_cnpj { get; set; }
        public string endereco { get; set; }
        public string complemento { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
        public string cep { get; set; }
        public string bairro { get; set; }
    }
}