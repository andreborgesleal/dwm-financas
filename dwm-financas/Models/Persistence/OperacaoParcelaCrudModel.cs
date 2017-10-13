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
    public abstract class OperacaoParcelaCrudModel<OP, OPRepo, OPE, OPERepo, OPEModel> 
        : CrudModel<OP, OPRepo, ApplicationContext>
        where OP : OperacaoParcela<OPE>
        where OPRepo : OperacaoParcelaViewModel<OPERepo>
        where OPE : OperacaoParcelaEvento
        where OPERepo : OperacaoParcelaEventoViewModel
        where OPEModel : OperacaoParcelaEventoModel<OPE,OPERepo>
    {
        #region Constructor
        public OperacaoParcelaCrudModel() { }
        public OperacaoParcelaCrudModel(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        protected OPEModel getModelInstance()
        {
            Type typeInstance = typeof(OPEModel);
            return (OPEModel)Activator.CreateInstance(typeInstance);
        }
        protected OPRepo getOperacaoParcelaRepositoryInstance()
        {
            Type typeInstance = typeof(OPRepo);
            return (OPRepo)Activator.CreateInstance(typeInstance);
        }
        protected OPERepo getOperacaoParcelaEventoRepositoryInstance()
        {
            Type typeInstance = typeof(OPERepo);
            return (OPERepo)Activator.CreateInstance(typeInstance);
        }
        #endregion

        #region Métodos da classe CrudModel
        public override OP MapToEntity(OPRepo value)
        {
            OP p = Find(value);

            #region ContaReceberParcela
            if (p == null)
            {
                p = getEntityInstance();
                p.OperacaoParcelaEventos = new List<OPE>();
            }
            else if (p.OperacaoParcelaEventos != null)
                p.OperacaoParcelaEventos.Clear();
            else
                p.OperacaoParcelaEventos = new List<OPE>();

            p.operacaoId = value.operacaoId;
            p.parcelaId = value.parcelaId;
            p.bancoId = value.bancoId;
            p.num_titulo = value.num_titulo;
            p.dt_vencimento = value.dt_vencimento;
            p.vr_principal = value.vr_principal;
            p.vr_amortizacao = value.vr_amortizacao;
            p.vr_desconto = value.vr_desconto;
            p.vr_encargos = value.vr_encargos;
            p.vr_saldo_devedor = value.vr_saldo_devedor;
            p.ind_forma_pagamento = value.ind_forma_pagamento;
            p.codigo_barras = value.codigo_barras;
            p.dt_ultima_amortizacao = value.dt_ultima_amortizacao;
            p.ind_baixa = value.ind_baixa;
            p.dt_baixa = value.dt_baixa;
            p.cheque_banco = value.cheque_banco;
            p.cheque_agencia = value.cheque_agencia;
            p.cheque_numero = value.cheque_numero;
            #endregion

            #region Eventos da parcela
            OPEModel operacaoParcelaEventoModel = getModelInstance(); //new ContaReceberParcelaEventoModel(this.db, this.seguranca_db);
            operacaoParcelaEventoModel.Create(this.db, this.seguranca_db);

            foreach (OPERepo pev in value.OperacaoParcelaEventos)
                p.OperacaoParcelaEventos.Add(operacaoParcelaEventoModel.MapToEntity(pev));
            #endregion

            return p;
        }

        public override OPRepo MapToRepository(OP entity)
        {
            #region OperacaoParcelaViewModel
            OPRepo x = getOperacaoParcelaRepositoryInstance();

            x.operacaoId = entity.operacaoId;
            x.parcelaId = entity.parcelaId;
            x.bancoId = entity.bancoId;
            x.nome_banco = entity.bancoId != null && entity.bancoId > 0 ? db.Bancos.Find(entity.bancoId).nome : "";
            x.num_titulo = entity.num_titulo;
            x.dt_vencimento = entity.dt_vencimento;
            x.vr_principal = entity.vr_principal;
            x.vr_encargos = entity.vr_encargos;
            x.vr_amortizacao = entity.vr_amortizacao;
            x.vr_total_pago = entity.vr_total_pago;
            x.dt_ultima_amortizacao = entity.dt_ultima_amortizacao;
            x.vr_saldo_devedor = entity.vr_saldo_devedor;
            x.ind_forma_pagamento = entity.ind_forma_pagamento;
            x.codigo_barras = entity.codigo_barras;
            x.ind_baixa = entity.ind_baixa;
            x.dt_baixa = entity.dt_baixa;
            x.cheque_banco = entity.cheque_banco;
            x.cheque_agencia = entity.cheque_agencia;
            x.cheque_numero = entity.cheque_numero;
            x.OperacaoParcelaEventos = new List<OPERepo>();
            #endregion

            #region OperacaoParcelaEventos

            OPEModel operacaoParcelaEventoModel = getModelInstance();
            operacaoParcelaEventoModel.Create(this.db, this.seguranca_db);
            foreach (OPE e in entity.OperacaoParcelaEventos)
            {
                OPERepo pev = operacaoParcelaEventoModel.MapToRepository(e);
                ((List<OPERepo>)x.OperacaoParcelaEventos).Add(pev);
            }
            #endregion

            return x;
        }

        public override Validate Validate(OPRepo value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };

            if (operation != Crud.INCLUIR && value.operacaoId <= 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Operação ID").ToString();
                value.mensagem.MessageBase = "Campo obrigatório: Operação ID";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.parcelaId <= 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Nº Parcela").ToString();
                value.mensagem.MessageBase = "Campo obrigatório: Número da parcela";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }


            if (operation != Crud.EXCLUIR)
            {
                if (value.dt_vencimento <= Convert.ToDateTime("1980-01-01"))
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Dt.Vencimento").ToString();
                    value.mensagem.MessageBase = "Campo obrigatório: Dt. Vencimento";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.vr_principal <= 0)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Vr.Principal").ToString();
                    value.mensagem.MessageBase = "Campo obrigatório: Valor Principal";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (!"1|2|3|4|5|6|9".Contains(value.ind_forma_pagamento))
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Vr.Principal").ToString();
                    value.mensagem.MessageBase = "Campo obrigatório: Forma de pagamento";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                //if (!value.bancoId.HasValue && value.vr_amortizacao > 0)
                //{
                //    value.mensagem.Code = 5;
                //    value.mensagem.Message = MensagemPadrao.Message(5, "Banco").ToString();
                //    value.mensagem.MessageBase = "Campo obrigatório: Banco";
                //    value.mensagem.MessageType = MsgType.WARNING;
                //    return value.mensagem;
                //}


                if (value.OperacaoParcelaEventos.Count() == 0)
                {
                    value.mensagem.Code = 46;
                    value.mensagem.MessageBase = MensagemPadrao.Message(46, "evento de contas a pagar").ToString();
                    value.mensagem.Message = "Deve ser incluído pelo menos um evento para a parcela.";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                // Validar eventos da parcela
                OPEModel operacaoParcelaEventoModel = getModelInstance();
                operacaoParcelaEventoModel.Create(this.db, this.seguranca_db);
                foreach (OPERepo pev in value.OperacaoParcelaEventos)
                {
                    Validate validate = operacaoParcelaEventoModel.Validate(pev, operation);
                    if (validate.Code > 0)
                        return validate;
                }

            }

            return value.mensagem;
        }

        public override OPRepo CreateRepository(System.Web.HttpRequestBase Request = null)
        {
            OPRepo r = base.CreateRepository();
            r.parcelaId = 1;
            r.dt_vencimento = Funcoes.Brasilia().Date;
            r.OperacaoParcelaEvento = getOperacaoParcelaEventoRepositoryInstance();
            r.OperacaoParcelaEvento.parcelaId = 1;
            r.OperacaoParcelaEvento.dt_ocorrencia = Funcoes.Brasilia().Date;
            r.OperacaoParcelaEvento.dt_movto = Funcoes.Brasilia().Date;

            return r;
        }
        #endregion
    }

    public abstract class ListViewOperacaoParcela<OPRepo, OPERepo> : ListViewModel<OPRepo, ApplicationContext>
        where OPRepo : OperacaoParcelaViewModel<OPERepo>
        where OPERepo : OperacaoParcelaEventoViewModel
    {
        #region Constructor
        public ListViewOperacaoParcela() { }
        public ListViewOperacaoParcela(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        protected OPRepo getOperacaoParcelaRepositoryInstance()
        {
            Type typeInstance = typeof(OPRepo);
            return (OPRepo)Activator.CreateInstance(typeInstance);
        }
        #endregion

        #region Métodos da classe ListViewRepository
        public override IEnumerable<OPRepo> Bind(int? index, int receSize = 50, params object[] param)
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

            IList<OPRepo> Parcelas = new List<OPRepo>();
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

                    OPRepo p = getOperacaoParcelaRepositoryInstance();

                    p.parcelaId = cont;
                    p.num_titulo = _num_titulo;
                    p.dt_vencimento = _dt_vencimento;
                    p.ind_forma_pagamento = _ind_forma_pagamento;
                    p.nome_banco = db.Bancos.Find(_bancoId).sigla ?? _nome_banco;
                    p.cheque_banco = _cheque_banco;
                    p.cheque_agencia = _cheque_agencia;
                    p.cheque_numero = _cheque_numero;
                    p.vr_principal = (cont == 1 && _vr_amortizacao > 0 ? _vr_amortizacao : vr_parcela);
                    p.vr_amortizacao = (cont == 1 && _vr_amortizacao > 0 ? _vr_amortizacao : 0);
                    p.vr_encargos = (cont == 1 && _vr_amortizacao > 0 ? _vr_juros + _vr_mora : 0);
                    p.vr_desconto = (cont == 1 && _vr_amortizacao > 0 ? _vr_desconto : 0);
                    p.vr_saldo_devedor = (cont == 1 && _vr_amortizacao > 0 ? 0 : vr_parcela);

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
}