using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace System.Web.Mvc
{
    public class SelectItemsTypeahead : SelectListItem
    {
        public string nextField { get; set; }
    }
}