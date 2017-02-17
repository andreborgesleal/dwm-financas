using App_Dominio.Component;
using App_Dominio.Contratos;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DWM.Models.Repositories
{
    public class BalanceteViewModel : PlanoContaViewModel
    {
        public decimal vr_saldo_ant { get; set; }
        public decimal vr_debito { get; set; }
        public decimal vr_credito { get; set; }
        public decimal vr_saldo_atual { get; set; }
    }
}