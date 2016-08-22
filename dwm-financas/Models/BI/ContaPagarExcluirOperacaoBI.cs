using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;

namespace DWM.Models.BI
{
    public class ContaPagarExcluirOperacaoBI : OperacaoExcluirBI<ContaPagarViewModel,
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
        protected override string spr_excluir_operacao()
        {
            return "spr_excluir_operacao_contas_pagar";
        }
        #endregion
    }
}