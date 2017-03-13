using System.Collections.Generic;
using System.Linq;
using App_Dominio.Entidades;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using System;
using App_Dominio.Component;
using App_Dominio.Models;

namespace DWM.Models.Persistence
{
    public class ContaPagarParcelaCrudModel
        : OperacaoParcelaCrudModel<ContaPagarParcela,
                                    ContaPagarParcelaViewModel,
                                    ContaPagarParcelaEvento,
                                    ContaPagarParcelaEventoViewModel,
                                    ContaPagarParcelaEventoModel>
    {
        #region Métodos da classe CrudModel
        public override ContaPagarParcela Find(ContaPagarParcelaViewModel key)
        {
            if (key.operacaoId == 0 || key.parcelaId == 0)
                return null;

            return db.ContaPagarParcelas.SingleOrDefault(info => info.operacaoId == key.operacaoId && info.parcelaId == key.parcelaId); //db.ContaPagarParcelas.Find(new { key.operacaoId, key.parcelaId });
        }

        public override ContaPagarParcela MapToEntity(ContaPagarParcelaViewModel value)
        {
            ContaPagarParcela entity = base.MapToEntity(value);

            if (!String.IsNullOrEmpty(value.ind_autorizacao))
                entity.ind_autorizacao = value.ind_autorizacao;
            else if (String.IsNullOrEmpty(entity.ind_autorizacao))
                entity.ind_autorizacao = "N";
            return entity;
        }

        public override ContaPagarParcelaViewModel MapToRepository(ContaPagarParcela entity)
        {
            ContaPagarParcelaViewModel value = base.MapToRepository(entity);
            value.ind_autorizacao = String.IsNullOrEmpty(entity.ind_autorizacao) ? "N" : entity.ind_autorizacao;
            return value;
        }
        #endregion

        #region Métodos Customizados

        #endregion
    }

    public class ListViewContaPagarParcela : ListViewModel<ContaPagarParcelaViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewContaPagarParcela() { }
        public ListViewContaPagarParcela(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe ListViewRepository
        public override IEnumerable<ContaPagarParcelaViewModel> Bind(int? index, int pageSize = 50, params object[] param)
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

            IList<ContaPagarParcelaViewModel> Parcelas = new List<ContaPagarParcelaViewModel>();
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

                    ContaPagarParcelaViewModel p = new ContaPagarParcelaViewModel()
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
                        vr_saldo_devedor = (cont == 1 && _vr_amortizacao > 0 ? 0 : vr_parcela),
                        ind_autorizacao = "N"
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

    public class ListViewContaPagarParcela2 : ListViewOperacaoParcela<ContaPagarParcelaViewModel, ContaPagarParcelaEventoViewModel>
    {

    }

    public class ListViewContaPagarAutorizar : ListViewModel<ContaPagarViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewContaPagarAutorizar() { }
        public ListViewContaPagarAutorizar(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe ListViewRepository
        public override IEnumerable<ContaPagarViewModel> Bind(int? index, int receSize = 50, params object[] param)
        {
            DateTime proximos5dias = Funcoes.Brasilia().Date.AddDays(5);

            #region LINQ
            var q = (from pag in db.ContaPagars
                     join par in db.ContaPagarParcelas on pag.operacaoId equals par.operacaoId
                     join cre in db.Credores on pag.credorId equals cre.credorId
                     where pag.empresaId.Equals(sessaoCorrente.empresaId)
                           && par.dt_vencimento <= proximos5dias
                           && (par.ind_baixa == null || par.ind_baixa=="")
                     orderby par.dt_vencimento
                     select new ContaPagarViewModel
                     {
                         operacaoId = pag.operacaoId,
                         empresaId = pag.empresaId,
                         nome_credor = cre.nome,
                         dt_emissao = pag.dt_emissao,
                         documento = pag.documento,
                         OperacaoParcela = new ContaPagarParcelaViewModel()
                         {
                             operacaoId = par.operacaoId,
                             parcelaId = par.parcelaId,
                             dt_vencimento = par.dt_vencimento,
                             ind_baixa = par.ind_baixa,
                             dt_baixa = par.dt_baixa,
                             dt_ultima_amortizacao = par.dt_ultima_amortizacao,
                             vr_principal = par.vr_principal,
                             vr_encargos = par.vr_encargos,
                             vr_desconto = par.vr_desconto,
                             vr_amortizacao = par.vr_amortizacao,
                             vr_total_pago = par.vr_total_pago,
                             vr_saldo_devedor = par.vr_saldo_devedor,
                             ind_autorizacao = par.ind_autorizacao ?? "N"
                         },
                         PageSize = receSize,
                         TotalCount = (from pag1 in db.ContaPagars
                                       join par1 in db.ContaPagarParcelas on pag1.operacaoId equals par1.operacaoId
                                       join cre1 in db.Credores on pag1.credorId equals cre1.credorId
                                       where pag1.empresaId.Equals(sessaoCorrente.empresaId)
                                             && par1.dt_vencimento <= proximos5dias
                                             && (par1.ind_baixa == null || par1.ind_baixa == "")
                                       orderby par1.dt_vencimento
                                       select pag1).Count()
                     }).Skip((index ?? 0) * receSize).Take(receSize).ToList();
            #endregion

            return q;
        }

        public override Repository getRepository(Object id)
        {
            ContaPagarModel model = new ContaPagarModel();
            model.Create(this.db, this.seguranca_db);
            return model.getObject((ContaPagarViewModel)id);
        }

        #endregion
    }
}