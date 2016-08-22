using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Models;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DWM.Models.BI
{
    public abstract class OperacaoEditarBI<EORepo, EOPERepo> : DWMContext<ApplicationContext>, IProcess<EORepo, ApplicationContext>
        where EORepo : EditarOperacaoViewModel<EOPERepo>
        where EOPERepo : EditarOperacaoParcelaEventoViewModel
    {
        #region Constructor
        public OperacaoEditarBI() { }

        public OperacaoEditarBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public abstract EORepo Run(Repository value);

        public IEnumerable<EORepo> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }
}