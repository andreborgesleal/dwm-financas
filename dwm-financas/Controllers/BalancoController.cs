namespace DWM.Controllers
{
    public class BalancoController : BalanceteMensalController
    {
        #region Herança
        public override string getListName()
        {
            return "Balanço";
        }
        #endregion    
    }
}