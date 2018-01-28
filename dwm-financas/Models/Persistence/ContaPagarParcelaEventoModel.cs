using DWM.Models.Repositories;
using DWM.Models.Entidades;

namespace DWM.Models.Persistence
{
    public class ContaPagarParcelaEventoModel : OperacaoParcelaEventoModel<ContaPagarParcelaEvento, ContaPagarParcelaEventoViewModel>
    {
        public override ContaPagarParcelaEvento Find(ContaPagarParcelaEventoViewModel key)
        {
            return db.ContaPagarParcelaEventos.Find(key.operacaoId, key.parcelaId, key.dt_evento);
        }

        public override string getComplementoHist(int operacaoId)
        {
            if (operacaoId != 0)
                return db.ContaPagars.Find(operacaoId).complementoHist;
            else
                return "";
        }
    }
}