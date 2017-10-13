using System;
using System.Collections.Generic;

namespace DWM.Models.Repositories
{
    public abstract class EditarOperacaoViewModel<OPE> : OperacaoParcelaViewModel<OperacaoParcelaEventoViewModel>
        where OPE : EditarOperacaoParcelaEventoViewModel
    {
        #region Dados da operação
        public DateTime dt_emissao { get; set; }
        public int historicoId { get; set; }
        public string descricao_historico { get; set; }
        public string complementoHist { get; set; }
        public System.Nullable<int> centroCustoId { get; set; }
        public string descricao_centroCusto { get; set; }
        public System.Nullable<decimal> vr_multa_atraso { get; set; }
        public decimal vr_total { get; set; }
        public int qte_parcelas { get; set; }
        public System.Nullable<int> enquadramentoId { get; set; }
        public string descricao_enquadramento { get; set; }
        public System.Nullable<DateTime> dt_pagamento { get; set; }
        public System.Nullable<DateTime> dt_movto { get; set; }
        public bool hasMultaMora { get; set; }
        public decimal? vr_juros_mora_baixa { get; set; }
        public decimal? vr_multa_atraso_baixa { get; set; }
        public decimal? vr_desconto_baixa { get; set; }
        public decimal? vr_baixa { get; set; }
        public string recorrencia { get; set; }
        #endregion

        public OPE editarOperacaoParcelaEventoViewModel { get; set; }

        #region Contabilidade
        public IEnumerable<ContabilidadeItemViewModel> Contabilidades { get; set; }
        #endregion

    }
}