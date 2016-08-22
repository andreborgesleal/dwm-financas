using DWM.Models.Repositories;
using DWM.Models.Entidades;

namespace DWM.Models.Persistence
{
    public class ContaReceberParcelaEventoModel : OperacaoParcelaEventoModel<ContaReceberParcelaEvento, ContaReceberParcelaEventoViewModel>
    {
        public override ContaReceberParcelaEvento Find(ContaReceberParcelaEventoViewModel key)
        {
            return db.ContaReceberParcelaEventos.Find(key.operacaoId, key.parcelaId, key.dt_evento);
        }
    }
}