using System.ComponentModel;

namespace DWM.Models.Repositories
{
    public class TituloIncluirViewModel : TituloViewModel
    {
        [DisplayName("Histórico")]
        public int historicoId { get; set; }

        [DisplayName("Histórico Complemento")]
        public string complementoHist { get; set; }

        public string descricao_historico { get; set; }

        [DisplayName("Enquadramento")]
        public int? enquadramentoId { get; set; }

        public string descricao_enquadramento { get; set; }

        [DisplayName("C.Custo")]
        public int? centroCustoId { get; set; }

        public string descricao_centroCusto { get; set; }

        [DisplayName("Documento")]
        public string documento { get; set; }
    }
}