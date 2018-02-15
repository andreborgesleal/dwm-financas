using System.ComponentModel;

namespace DWM.Models.Repositories
{
    public class TituloIncluirViewModel : TituloViewModel
    {
        [DisplayName("Histórico")]
        public int historicoId { get; set; }

        [DisplayName("Histórico Complemento")]
        public string complementoHist { get; set; }

        [DisplayName("Enquadramento")]
        public int? enquadramentoId { get; set; }

        [DisplayName("C.Custo")]
        public int? centroCustoId { get; set; }

        [DisplayName("Documento")]
        public string Documento { get; set; }
    }
}