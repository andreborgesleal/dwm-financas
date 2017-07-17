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