using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;
using System.Web.Mvc;


namespace DWM.Models.Repositories
{
    public class HomeViewModel : Repository
    {
        public string nome_empresa { get; set; }
        public IPagedList Cobranca { get; set; }
        public IPagedList Pagamentos { get; set; }
        public IEnumerable<MovtoBancarioViewModel> FluxoCaixa { get; set; }
        public IEnumerable<ContabilidadeViewModel> ResumoFinanceiro { get; set; }
    }
}