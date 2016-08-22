using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;

namespace DWM.Models.BI
{
    public class ContaPagarCancelarOperacaoBI : OperacaoCancelarBI<ContaPagarViewModel,
                                                                     ContaPagarParcelaViewModel,
                                                                     ContaPagarParcelaEventoViewModel,
                                                                     ContaPagarModel,
                                                                     ContaPagarParcelaCrudModel,
                                                                     ContaPagarParcelaEventoModel,
                                                                     ContaPagar,
                                                                     ContaPagarParcela,
                                                                     ContaPagarParcelaEvento>
    {
    }
}