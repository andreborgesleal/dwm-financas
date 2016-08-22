using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;

namespace DWM.Models.BI
{
    public class ContaPagarLiquidarOperacaoBI : OperacaoLiquidarBI<ContaPagarViewModel,
                                                                     ContaPagarParcelaViewModel,
                                                                     ContaPagarParcelaEventoViewModel,
                                                                     ContaPagarModel,
                                                                     ContaPagarParcelaCrudModel,
                                                                     ContaPagarParcelaEventoModel,
                                                                     ContaPagar,
                                                                     ContaPagarParcela,
                                                                     ContaPagarParcelaEvento>
    {
        #region Abstract Methods
        protected override string spr_liquidar_operacao()
        {
            return "spr_liquidar_conta_pagar";
        }
        #endregion
    }
}