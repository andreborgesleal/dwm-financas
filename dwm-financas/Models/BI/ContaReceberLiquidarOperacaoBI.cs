using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;

namespace DWM.Models.BI
{
    public class ContaReceberLiquidarOperacaoBI : OperacaoLiquidarBI<ContaReceberViewModel,
                                                                     ContaReceberParcelaViewModel,
                                                                     ContaReceberParcelaEventoViewModel,
                                                                     ContaReceberModel,
                                                                     ContaReceberParcelaCrudModel,
                                                                     ContaReceberParcelaEventoModel,
                                                                     ContaReceber,
                                                                     ContaReceberParcela,
                                                                     ContaReceberParcelaEvento>
    {
        #region Abstract Methods
        protected override string spr_liquidar_operacao()
        {
            return "spr_liquidar_conta_receber";
        }
        #endregion
    }
}