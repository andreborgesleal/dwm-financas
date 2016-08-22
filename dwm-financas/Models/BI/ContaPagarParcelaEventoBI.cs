using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;

namespace DWM.Models.BI
{
    public class ContaPagarParcelaEventoBI : OperacaoParcelaEventoBI<EditarContaPagarViewModel,
                                                                       EditarContaPagarParcelaEventoViewModel,
                                                                       ContaPagarViewModel,
                                                                       ContaPagarParcelaViewModel,
                                                                       ContaPagarParcelaEventoViewModel,
                                                                       ContaPagarParcelaEventoModel,
                                                                       ContaPagarParcelaEvento,
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