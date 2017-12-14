using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;

namespace DWM.Models.Repositories
{
    public class RemessaViewModel : Repository
    {
        [DisplayName("BancoID")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string BancoID { get; set; }

        [DisplayName("ConvenioID")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string ConvenioID { get; set; }

        [DisplayName("RemessaID")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Campo deve ser preenchido somente com números")]
        public string RemessaID { get; set; }

        [DisplayName("DataGeracao")]
        public System.Nullable<System.DateTime> DataGeracao { get; set; }

        [DisplayName("LayoutArquivo")] 
        public string LayoutArquivo { get; set; }
    }
}

