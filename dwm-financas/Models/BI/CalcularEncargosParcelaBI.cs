using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Models;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DWM.Models.BI
{
    public class _CalcularEncargosParcelaBI<OPRepo, OPERepo> : DWMContext<ApplicationContext>, IProcess<OPRepo, ApplicationContext>
        where OPRepo : OperacaoParcelaViewModel<OPERepo>
        where OPERepo : OperacaoParcelaEventoViewModel
    {
        public decimal? vr_jurosMora { get; set; }
        public decimal? vr_multa { get; set; }
        public DateTime dt_referencia { get; set; }

        #region Constructor
        public _CalcularEncargosParcelaBI() { }

        public _CalcularEncargosParcelaBI(ApplicationContext _db, SecurityContext _seguranca_db, decimal? vr_jurosMora, decimal? vr_multa, DateTime? dt_referencia = null)
        {
            base.Create(_db, _seguranca_db);
            this.vr_jurosMora = vr_jurosMora;
            this.vr_multa = vr_multa;
            this.dt_referencia = dt_referencia ?? Funcoes.Brasilia().Date;
        }
        #endregion

        public OPRepo Run(Repository value)
        {
            if (!vr_jurosMora.HasValue && !vr_multa.HasValue)
                return (OPRepo)value;

            OPRepo p = (OPRepo)value;

            #region Calcula o próximo dia útil em relação à data de vencimento
            ObterProximoDiaUtil obterProximoDiaUtil = new ObterProximoDiaUtil(this.db, this.seguranca_db, p.dt_vencimento);
            obterProximoDiaUtil.Run(new FeriadoViewModel());
            DateTime dt_proximo_diaUtil = obterProximoDiaUtil.dt_referencia;
            #endregion

            p.vr_encargos = 0;

            #region Calcula o total de encargos (mora e os juros se a parcela estiver atrasada)
            if (dt_referencia > dt_proximo_diaUtil && vr_jurosMora.HasValue && vr_jurosMora > 0)
            {
                TimeSpan ts = dt_referencia - p.dt_vencimento;
                int diasAtraso = Funcoes.DateDiff(p.dt_vencimento, dt_referencia);
                p.vr_encargos = diasAtraso * (vr_jurosMora / 100) * p.vr_principal;
            }

            if (dt_referencia > dt_proximo_diaUtil && vr_multa.HasValue && vr_multa > 0)
            {
                p.vr_encargos += (vr_multa / 100) * p.vr_principal;
                p.vr_encargos = Math.Round(p.vr_encargos.Value, 2);
            }
            #endregion

            return p;
        }

        public IEnumerable<OPRepo> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }

    public class _CalcularJurosMoraParcelaBI<OPRepo, OPERepo> : DWMContext<ApplicationContext>, IProcess<OPRepo, ApplicationContext>
        where OPRepo : OperacaoParcelaViewModel<OPERepo>
        where OPERepo : OperacaoParcelaEventoViewModel
    {
        public decimal? vr_jurosMora { get; set; }
        public DateTime dt_referencia { get; set; }

        #region Constructor
        public _CalcularJurosMoraParcelaBI() { }

        public _CalcularJurosMoraParcelaBI(ApplicationContext _db, SecurityContext _seguranca_db, decimal? vr_jurosMora, DateTime? dt_referencia = null)
        {
            base.Create(_db, _seguranca_db);
            this.vr_jurosMora = vr_jurosMora;
            this.dt_referencia = dt_referencia ?? Funcoes.Brasilia().Date;
        }
        #endregion

        public OPRepo Run(Repository value)
        {
            if (!vr_jurosMora.HasValue)
                return (OPRepo)value;

            OPRepo p = (OPRepo)value;

            #region Calcula o próximo dia útil em relação à data de vencimento
            ObterProximoDiaUtil obterProximoDiaUtil = new ObterProximoDiaUtil(this.db, this.seguranca_db, p.dt_vencimento);
            obterProximoDiaUtil.Run(new FeriadoViewModel());
            DateTime dt_proximo_diaUtil = obterProximoDiaUtil.dt_referencia;
            #endregion

            p.vr_encargos = 0;

            #region Calcula o total de encargos (mora e os juros se a parcela estiver atrasada)
            if (dt_referencia > dt_proximo_diaUtil && vr_jurosMora.HasValue && vr_jurosMora > 0)
            {
                TimeSpan ts = dt_referencia - p.dt_vencimento;
                int diasAtraso = Funcoes.DateDiff(p.dt_vencimento, dt_referencia);
                p.vr_encargos = diasAtraso * (vr_jurosMora / 100) * p.vr_principal;
                p.vr_encargos = Math.Round(p.vr_encargos.Value, 2);
            }
            #endregion

            return p;
        }

        public IEnumerable<OPRepo> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }

    public class _CalcularMultaAtrasoParcelaBI<OPRepo, OPERepo> : DWMContext<ApplicationContext>, IProcess<OPRepo, ApplicationContext>
        where OPRepo : OperacaoParcelaViewModel<OPERepo>
        where OPERepo : OperacaoParcelaEventoViewModel
    {
        public decimal? vr_multa { get; set; }
        public DateTime dt_referencia { get; set; }

        #region Constructor
        public _CalcularMultaAtrasoParcelaBI() { }

        public _CalcularMultaAtrasoParcelaBI(ApplicationContext _db, SecurityContext _seguranca_db, decimal? vr_multa, DateTime? dt_referencia = null)
        {
            base.Create(_db, _seguranca_db);
            this.vr_multa = vr_multa;
            this.dt_referencia = dt_referencia ?? Funcoes.Brasilia().Date;
        }
        #endregion

        public OPRepo Run(Repository value)
        {
            if (!vr_multa.HasValue)
                return (OPRepo)value;

            OPRepo p = (OPRepo)value;

            #region Calcula o próximo dia útil em relação à data de vencimento
            ObterProximoDiaUtil obterProximoDiaUtil = new ObterProximoDiaUtil(this.db, this.seguranca_db, p.dt_vencimento);
            obterProximoDiaUtil.Run(new FeriadoViewModel());
            DateTime dt_proximo_diaUtil = obterProximoDiaUtil.dt_referencia;
            #endregion

            p.vr_encargos = 0;

            if (dt_referencia > dt_proximo_diaUtil && vr_multa.HasValue && vr_multa > 0)
            {
                p.vr_encargos += (vr_multa / 100) * p.vr_principal;
                p.vr_encargos = Math.Round(p.vr_encargos.Value, 2);
            }

            return p;
        }

        public IEnumerable<OPRepo> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }


    //#region Obsoleto
    //public class CalcularEncargosParcelaBI : DWMContext<ApplicationContext>, IProcess<ContaPagarParcelaViewModel, ApplicationContext>
    //{
    //    public decimal? vr_jurosMora { get; set; }
    //    public decimal? vr_multa { get; set; }
    //    public DateTime dt_referencia { get; set; }

    //    #region Constructor
    //    public CalcularEncargosParcelaBI() { }

    //    public CalcularEncargosParcelaBI(ApplicationContext _db, SecurityContext _seguranca_db, decimal? vr_jurosMora, decimal? vr_multa, DateTime? dt_referencia = null)
    //    {
    //        base.Create(_db, _seguranca_db);
    //        this.vr_jurosMora = vr_jurosMora;
    //        this.vr_multa = vr_multa;
    //        this.dt_referencia = dt_referencia ?? Funcoes.Brasilia().Date;
    //    }
    //    #endregion

    //    public ContaPagarParcelaViewModel Run(Repository value)
    //    {
    //        if (!vr_jurosMora.HasValue && !vr_multa.HasValue)
    //            return (ContaPagarParcelaViewModel)value;

    //        ContaPagarParcelaViewModel p = (ContaPagarParcelaViewModel)value;

    //        #region Calcula o próximo dia útil em relação à data de vencimento
    //        ObterProximoDiaUtil obterProximoDiaUtil = new ObterProximoDiaUtil(this.db, this.seguranca_db, p.dt_vencimento);
    //        obterProximoDiaUtil.Run(new FeriadoViewModel());
    //        DateTime dt_proximo_diaUtil = obterProximoDiaUtil.dt_referencia;
    //        #endregion

    //        p.vr_encargos = 0;

    //        #region Calcula o total de encargos (mora e os juros se a parcela estiver atrasada)
    //        if (dt_referencia > dt_proximo_diaUtil && vr_jurosMora.HasValue && vr_jurosMora > 0)
    //        {
    //            TimeSpan ts = dt_referencia - p.dt_vencimento;
    //            int diasAtraso = Funcoes.DateDiff(p.dt_vencimento, dt_referencia);
    //            p.vr_encargos = diasAtraso * (vr_jurosMora / 100) * p.vr_principal;
    //        }

    //        if (dt_referencia > dt_proximo_diaUtil && vr_multa.HasValue && vr_multa > 0)
    //        {
    //            p.vr_encargos += (vr_multa / 100) * p.vr_principal;
    //            p.vr_encargos = Math.Round(p.vr_encargos.Value, 2);
    //        }
    //        #endregion

    //        return p;
    //    }

    //    public IEnumerable<ContaPagarParcelaViewModel> List(params object[] param)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class CalcularJurosMoraParcelaBI : DWMContext<ApplicationContext>, IProcess<ContaPagarParcelaViewModel, ApplicationContext>
    //{
    //    public decimal? vr_jurosMora { get; set; }
    //    public DateTime dt_referencia { get; set; }

    //    #region Constructor
    //    public CalcularJurosMoraParcelaBI() { }

    //    public CalcularJurosMoraParcelaBI(ApplicationContext _db, SecurityContext _seguranca_db, decimal? vr_jurosMora, DateTime? dt_referencia = null)
    //    {
    //        base.Create(_db, _seguranca_db);
    //        this.vr_jurosMora = vr_jurosMora;
    //        this.dt_referencia = dt_referencia ?? Funcoes.Brasilia().Date;
    //    }
    //    #endregion

    //    public ContaPagarParcelaViewModel Run(Repository value)
    //    {
    //        if (!vr_jurosMora.HasValue)
    //            return (ContaPagarParcelaViewModel)value;

    //        ContaPagarParcelaViewModel p = (ContaPagarParcelaViewModel)value;

    //        #region Calcula o próximo dia útil em relação à data de vencimento
    //        ObterProximoDiaUtil obterProximoDiaUtil = new ObterProximoDiaUtil(this.db, this.seguranca_db, p.dt_vencimento);
    //        obterProximoDiaUtil.Run(new FeriadoViewModel());
    //        DateTime dt_proximo_diaUtil = obterProximoDiaUtil.dt_referencia;
    //        #endregion

    //        p.vr_encargos = 0;

    //        #region Calcula o total de encargos (mora e os juros se a parcela estiver atrasada)
    //        if (dt_referencia > dt_proximo_diaUtil && vr_jurosMora.HasValue && vr_jurosMora > 0)
    //        {
    //            TimeSpan ts = dt_referencia - p.dt_vencimento;
    //            int diasAtraso = Funcoes.DateDiff(p.dt_vencimento, dt_referencia);
    //            p.vr_encargos = diasAtraso * (vr_jurosMora / 100) * p.vr_principal;
    //            p.vr_encargos = Math.Round(p.vr_encargos.Value, 2);
    //        }
    //        #endregion

    //        return p;
    //    }

    //    public IEnumerable<ContaPagarParcelaViewModel> List(params object[] param)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class CalcularMultaAtrasoParcelaBI : DWMContext<ApplicationContext>, IProcess<ContaPagarParcelaViewModel, ApplicationContext>
    //{
    //    public decimal? vr_multa { get; set; }
    //    public DateTime dt_referencia { get; set; }

    //    #region Constructor
    //    public CalcularMultaAtrasoParcelaBI() { }

    //    public CalcularMultaAtrasoParcelaBI(ApplicationContext _db, SecurityContext _seguranca_db, decimal? vr_multa, DateTime? dt_referencia = null)
    //    {
    //        base.Create(_db, _seguranca_db);
    //        this.vr_multa = vr_multa;
    //        this.dt_referencia = dt_referencia ?? Funcoes.Brasilia().Date;
    //    }
    //    #endregion

    //    public ContaPagarParcelaViewModel Run(Repository value)
    //    {
    //        if (!vr_multa.HasValue)
    //            return (ContaPagarParcelaViewModel)value;

    //        ContaPagarParcelaViewModel p = (ContaPagarParcelaViewModel)value;

    //        #region Calcula o próximo dia útil em relação à data de vencimento
    //        ObterProximoDiaUtil obterProximoDiaUtil = new ObterProximoDiaUtil(this.db, this.seguranca_db, p.dt_vencimento);
    //        obterProximoDiaUtil.Run(new FeriadoViewModel());
    //        DateTime dt_proximo_diaUtil = obterProximoDiaUtil.dt_referencia;
    //        #endregion

    //        p.vr_encargos = 0;

    //        if (dt_referencia > dt_proximo_diaUtil && vr_multa.HasValue && vr_multa > 0)
    //        {
    //            p.vr_encargos += (vr_multa / 100) * p.vr_principal;
    //            p.vr_encargos = Math.Round(p.vr_encargos.Value, 2);
    //        }

    //        return p;
    //    }

    //    public IEnumerable<ContaPagarParcelaViewModel> List(params object[] param)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    //#endregion

}