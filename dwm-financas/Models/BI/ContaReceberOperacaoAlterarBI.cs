using App_Dominio.Component;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;

namespace DWM.Models.BI
{
    public class ContaReceberOperacaoAlterarBI : OperacaoAlterarBI<ContaReceberViewModel,
                                                                   ContaReceberParcelaViewModel,
                                                                   ContaReceberParcelaEventoViewModel,
                                                                   ContaReceberModel,
                                                                   ContaReceberParcelaCrudModel,
                                                                   ContaReceberParcelaEventoModel,
                                                                   ContaReceber,
                                                                   ContaReceberParcela,
                                                                   ContaReceberParcelaEvento>
    {
        #region Abstract Methods
        protected override string Operacao_Table()
        {
            return "ContaReceber";
        }
        protected override int Cliente_Credor_ID(Repository value)
        {
            return ((ContaReceberViewModel)value).clienteId;
        }
        protected override string Cliente_Credor_Atributo()
        {
            return "clienteId";
        }
        #endregion
    }
}