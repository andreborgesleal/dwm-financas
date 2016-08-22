using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;

namespace DWM.Models.BI
{
    public class ContaPagarModifyBI : OperacaoModifyBI<EditarContaPagarViewModel,
                                                         EditarContaPagarParcelaEventoViewModel,
                                                         ContaPagarParcelaViewModel,
                                                         ContaPagarParcelaEventoViewModel,
                                                         ContaPagarParcelaCrudModel,
                                                         ContaPagarParcelaEventoModel,
                                                         ContaPagarParcela,
                                                         ContaPagarParcelaEvento,
                                                         ContaPagarEditarBI>
    {
    }
}