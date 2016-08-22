using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;

namespace DWM.Models.BI
{
    public class ContaReceberParcelaEventoBI : OperacaoParcelaEventoBI<EditarContaReceberViewModel, 
                                                                       EditarContaReceberParcelaEventoViewModel, 
                                                                       ContaReceberViewModel,
                                                                       ContaReceberParcelaViewModel,
                                                                       ContaReceberParcelaEventoViewModel,
                                                                       ContaReceberParcelaEventoModel,
                                                                       ContaReceberParcelaEvento,
                                                                       ContaReceberEditarBI>
    {
        #region Abstract Methods
        protected override string getTipoMovto()
        {
            return "C";
        }
        #endregion
    }
}