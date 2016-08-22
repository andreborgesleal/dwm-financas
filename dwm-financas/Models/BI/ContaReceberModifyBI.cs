using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;

namespace DWM.Models.BI
{
    public class ContaReceberModifyBI : OperacaoModifyBI<EditarContaReceberViewModel,
                                                         EditarContaReceberParcelaEventoViewModel,
                                                         ContaReceberParcelaViewModel,
                                                         ContaReceberParcelaEventoViewModel,
                                                         ContaReceberParcelaCrudModel,
                                                         ContaReceberParcelaEventoModel,
                                                         ContaReceberParcela,
                                                         ContaReceberParcelaEvento,
                                                         ContaReceberEditarBI>
    {
    }
}