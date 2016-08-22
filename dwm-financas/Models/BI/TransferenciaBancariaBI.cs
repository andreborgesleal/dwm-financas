using System;
using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Models;
using System.IO;
using System.Web;
using DWM.Models.Persistence;
using System.Web.Mvc;

namespace DWM.Models.BI
{
    public class TransferenciaBancariaBI : DWMContext<ApplicationContext>, IProcess<TransferenciaBancariaViewModel, ApplicationContext>
    {

        #region Constructor
        public TransferenciaBancariaBI() { }

        public TransferenciaBancariaBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public TransferenciaBancariaViewModel Run(Repository value)
        {
            TransferenciaBancariaViewModel t = (TransferenciaBancariaViewModel)value;
            MovtoBancarioModel model = new MovtoBancarioModel(this.db, this.seguranca_db);

            #region Origem da transferência
            t.movtoBancarioOrigemViewModel = model.Insert(t.movtoBancarioOrigemViewModel);
            if (t.movtoBancarioOrigemViewModel.mensagem.Code > 0)
            {
              t.mensagem = t.movtoBancarioOrigemViewModel.mensagem;
              return t;
            }
            #endregion

            #region destino da transferência
            t.movtoBancarioDestinoViewModel = model.Insert(t.movtoBancarioDestinoViewModel);
            if (t.movtoBancarioDestinoViewModel.mensagem.Code > 0)
            {
              t.mensagem = t.movtoBancarioDestinoViewModel.mensagem;
              return t;
            }
            #endregion

            return t;
        }

        public IEnumerable<TransferenciaBancariaViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }
}