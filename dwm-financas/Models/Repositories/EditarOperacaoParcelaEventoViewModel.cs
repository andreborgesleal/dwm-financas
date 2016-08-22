namespace DWM.Models.Repositories
{
    public abstract class EditarOperacaoParcelaEventoViewModel : OperacaoParcelaEventoViewModel
    {
        public int historicoId { get; set; }
        public string complementoHist { get; set; }
        public System.Nullable<int> bancoId { get; set; }
        public string nome_banco { get; set; }
        public string descricao_enquadramento { get; set; }
    }
}