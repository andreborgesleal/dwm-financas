using App_Dominio.Component;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;

namespace DWM.Models.BI
{
    public class ContaPagarOperacaoAlterarBI : OperacaoAlterarBI<ContaPagarViewModel,
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
        protected override string Operacao_Table()
        {
            return "ContaPagar";
        }
        protected override int Cliente_Credor_ID(Repository value)
        {
            return ((ContaPagarViewModel)value).credorId;
        }
        protected override string Cliente_Credor_Atributo()
        {
            return "credorId";
        }
        #endregion
    }
}