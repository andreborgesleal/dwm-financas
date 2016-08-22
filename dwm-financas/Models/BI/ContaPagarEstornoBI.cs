using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;

namespace DWM.Models.BI
{
    public class ContaPagarEstornoBI : OperacaoEstornoBI<EditarContaPagarViewModel,
                                                           EditarContaPagarParcelaEventoViewModel,
                                                           ContaPagarParcelaCrudModel,
                                                           ContaPagarParcelaEventoModel,
                                                           ContaPagarParcelaViewModel,
                                                           ContaPagarParcelaEventoViewModel,
                                                           ContaPagarParcela,
                                                           ContaPagarParcelaEvento,
                                                           ContaPagarEditarBI>
    {
    }
}