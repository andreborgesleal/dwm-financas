using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DWM.Models.Repositories
{
    public class ContaReceberDemonstrativoViewModel : ContaReceberViewModel
    {
        public System.Nullable<decimal> TotalEmitido { get; set; }
        public System.Nullable<decimal> TotalAmortizado { get; set; }
        public System.Nullable<decimal> TotalEmAberto { get; set; }
        public System.Nullable<int> TotalTitulos { get; set; }
    }
}