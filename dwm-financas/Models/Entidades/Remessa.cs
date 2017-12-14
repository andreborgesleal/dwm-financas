using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("Remessa")]
    public class Remessa
    {
        [Key, Column(Order = 0)]
        [DisplayName("BancoID")]
        public string BancoID { get; set; }

        [Key, Column(Order = 1)]
        [DisplayName("ConvenioID")]
        public string ConvenioID { get; set; }

        [Key, Column(Order = 2)]
        [DisplayName("RemessaID")]
        public string RemessaID { get; set; }

        [DisplayName("DataGeracao")] 
        public System.Nullable<System.DateTime> DataGeracao { get; set; }

        [DisplayName("LayoutArquivo")]
        public string LayoutArquivo { get; set; }
    }
}