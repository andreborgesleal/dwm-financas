using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;

namespace DWM.Models.BI
{
    public class ContaReceberExcluirOperacaoBI : OperacaoExcluirBI<ContaReceberViewModel, 
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
        protected override string spr_excluir_operacao()
        {
            return "spr_excluir_operacao_contas_receber";
        }
        #endregion
    }
}