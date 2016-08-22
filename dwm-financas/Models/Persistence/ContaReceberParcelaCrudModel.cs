using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using System;
using App_Dominio.Models;
using App_Dominio.Component;

namespace DWM.Models.Persistence
{
    public class ContaReceberParcelaCrudModel 
        : OperacaoParcelaCrudModel<ContaReceberParcela, 
                                    ContaReceberParcelaViewModel, 
                                    ContaReceberParcelaEvento, 
                                    ContaReceberParcelaEventoViewModel,
                                    ContaReceberParcelaEventoModel>
    {
        #region Métodos da classe CrudModel
        public override ContaReceberParcela Find(ContaReceberParcelaViewModel key)
        {
            if (key.operacaoId == 0 || key.parcelaId == 0)
                return null;

            return db.ContaReceberParcelas.SingleOrDefault(info => info.operacaoId == key.operacaoId && info.parcelaId == key.parcelaId); //db.ContaReceberParcelas.Find(new { key.operacaoId, key.parcelaId });
        }
        #endregion

        #region Métodos Customizados

        #endregion
    }

    public class ListViewContaReceberParcela : ListViewModel<ContaReceberParcelaViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewContaReceberParcela() { }
        public ListViewContaReceberParcela(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe ListViewRepository
        public override IEnumerable<ContaReceberParcelaViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            #region Parâmetros
            string _num_titulo = param[0] != null ? param[0].ToString() : "";
            DateTime _dt_vencimento = (DateTime)param[1];
            string _ind_forma_pagamento = param[2].ToString();
            string _nome_banco = param[3] != null ? param[3].ToString() : "";
            int? _cheque_banco = (int?)param[4];
            string _cheque_agencia = param[5] != null ? param[5].ToString() : null;
            string _cheque_numero = param[6] != null ? param[6].ToString() : null;
            decimal _vr_principal = (decimal)param[7];
            decimal _vr_amortizacao = param[8] != null ? (decimal)param[8] : 0;
            decimal _vr_juros = param[9] != null ? (decimal)param[9] : 0;
            decimal _vr_mora = param[10] != null ? (decimal)param[10] : 0;
            decimal _vr_desconto = param[11] != null ? (decimal)param[11] : 0;
            int _num_parcelas = (int)param[12];
            int _bancoId = (int)(param[13]);
            #endregion

            IList<ContaReceberParcelaViewModel> Parcelas = new List<ContaReceberParcelaViewModel>();
            decimal vr_parcela = Math.Round((_vr_principal - _vr_amortizacao) / _num_parcelas, 2);

            int cont = 0;
            int dia_vencimento = _dt_vencimento.Day;
            int _num_cheque = 0;
            DateTime data_suporte;
            bool flag = false;

            if (_cheque_numero != null && _cheque_numero != "")
                try
                {
                    _num_cheque = int.Parse(_cheque_numero);
                    flag = true;
                }
                catch
                {
                    flag = false;
                };

            _num_parcelas = _vr_amortizacao == 0 ? _num_parcelas - 1 : _num_parcelas;

            decimal total = 0;
            using (ApplicationContext db = new ApplicationContext())
            {
                while (cont++ <= _num_parcelas)
                {
                    if (flag)
                    {
                        _cheque_numero = _num_cheque.ToString();
                        _num_cheque++;
                    }

                    if (cont > _num_parcelas)
                    {
                        vr_parcela = _vr_principal - total;
                    }

                    total += (cont == 1 && _vr_amortizacao > 0 ? _vr_amortizacao : vr_parcela);

                    ContaReceberParcelaViewModel p = new ContaReceberParcelaViewModel()
                    {
                        parcelaId = cont,
                        num_titulo = _num_titulo,
                        dt_vencimento = _dt_vencimento,
                        ind_forma_pagamento = _ind_forma_pagamento,
                        nome_banco = db.Bancos.Find(_bancoId).sigla ?? _nome_banco,
                        cheque_banco = _cheque_banco,
                        cheque_agencia = _cheque_agencia,
                        cheque_numero = _cheque_numero,
                        vr_principal = (cont == 1 && _vr_amortizacao > 0 ? _vr_amortizacao : vr_parcela),
                        vr_amortizacao = (cont == 1 && _vr_amortizacao > 0 ? _vr_amortizacao : 0),
                        vr_encargos = (cont == 1 && _vr_amortizacao > 0 ? _vr_juros + _vr_mora : 0),
                        vr_desconto = (cont == 1 && _vr_amortizacao > 0 ? _vr_desconto : 0),
                        vr_saldo_devedor = (cont == 1 && _vr_amortizacao > 0 ? 0 : vr_parcela)
                    };

                    Parcelas.Add(p);

                    _dt_vencimento = _dt_vencimento.AddMonths(1);

                    if (dia_vencimento > 28)
                    {
                        try
                        {
                            data_suporte = Convert.ToDateTime(_dt_vencimento.ToString("yyyy-MM-") + dia_vencimento.ToString());
                            _dt_vencimento = data_suporte;
                        }
                        catch
                        {

                        }
                    }
                }
            }

            return Parcelas;
        }

        public override Repository getRepository(Object id)
        {
            throw new NotImplementedException();
        }

        public override string DivId()
        {
            return "div-item";
        }
        #endregion

    }

    public class ListViewContaReceberParcela2 : ListViewOperacaoParcela<ContaReceberParcelaViewModel, ContaReceberParcelaEventoViewModel>
    {

    }

}