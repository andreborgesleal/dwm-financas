using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DWM.Models.BI
{
    public class ContaReceberBaixarBI : OperacaoBaixarBI<EditarContaReceberViewModel, 
                                                        EditarContaReceberParcelaEventoViewModel, 
                                                        ContaReceberViewModel, 
                                                        ContaReceberParcelaViewModel, 
                                                        ContaReceberParcelaEventoViewModel, 
                                                        ContaReceberParcelaEventoModel, 
                                                        ContaReceberParcelaCrudModel, 
                                                        ContaReceberModel, 
                                                        ContaReceberParcelaEvento, 
                                                        ContaReceberParcela, 
                                                        ContaReceber, 
                                                        ContaReceberEditarBI >
    {
        #region Abstract Methods
        protected override string getTipoMovto()
        {
            return "C";
        }
        #endregion

    }
}