using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;

namespace DWM.Models.BI
{
    public class ContaPagarBaixarBI : OperacaoBaixarBI<EditarContaPagarViewModel,
                                                        EditarContaPagarParcelaEventoViewModel,
                                                        ContaPagarViewModel,
                                                        ContaPagarParcelaViewModel,
                                                        ContaPagarParcelaEventoViewModel,
                                                        ContaPagarParcelaEventoModel,
                                                        ContaPagarParcelaCrudModel,
                                                        ContaPagarModel,
                                                        ContaPagarParcelaEvento,
                                                        ContaPagarParcela,
                                                        ContaPagar,
                                                        ContaPagarEditarBI>
    {
        #region Abstract Methods
        protected override string getTipoMovto()
        {
            return "D";
        }
        #endregion

    }
}