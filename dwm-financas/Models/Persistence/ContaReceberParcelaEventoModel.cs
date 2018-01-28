using DWM.Models.Repositories;
using DWM.Models.Entidades;
using System;

namespace DWM.Models.Persistence
{
    public class ContaReceberParcelaEventoModel : OperacaoParcelaEventoModel<ContaReceberParcelaEvento, ContaReceberParcelaEventoViewModel>
    {
        public override ContaReceberParcelaEvento Find(ContaReceberParcelaEventoViewModel key)
        {
            return db.ContaReceberParcelaEventos.Find(key.operacaoId, key.parcelaId, key.dt_evento);
        }

        public override string getComplementoHist(int operacaoId)
        {
            if (operacaoId != 0)
                return db.ContaRecebers.Find(operacaoId).complementoHist;
            else
                return "";
        }
    }
}