using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;

namespace DWM.Models.BI
{
    public class ContaReceberEstornoBI : OperacaoEstornoBI<EditarContaReceberViewModel, 
                                                           EditarContaReceberParcelaEventoViewModel, 
                                                           ContaReceberParcelaCrudModel, 
                                                           ContaReceberParcelaEventoModel, 
                                                           ContaReceberParcelaViewModel, 
                                                           ContaReceberParcelaEventoViewModel, 
                                                           ContaReceberParcela, 
                                                           ContaReceberParcelaEvento, 
                                                           ContaReceberEditarBI>
    {
    }
}