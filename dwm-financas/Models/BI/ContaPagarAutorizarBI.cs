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
    public class ContaPagarAutorizarBI : DWMContext<ApplicationContext>, IProcessAPI<ContaPagarParcelaViewModel, ApplicationContext>
    {
        #region Constructor
        public ContaPagarAutorizarBI() { }

        public ContaPagarAutorizarBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual ContaPagarParcelaViewModel Run(Repository value)
        {
            ContaPagarParcelaViewModel r = (ContaPagarParcelaViewModel)value;
            try
            {
                ContaPagarParcelaCrudModel model = new ContaPagarParcelaCrudModel();
                model.Create(this.db, this.seguranca_db);
                ContaPagarParcelaViewModel p = model.getObject(r);
                p.ind_autorizacao = r.ind_autorizacao;
                p.uri = r.uri;
                p = model.Update(p);
                if (p.mensagem.Code > 0)
                    throw new Exception(p.mensagem.Message);
            }
            catch (Exception ex)
            {
                r.mensagem = new Validate() { Code = 999, MessageBase = ex.Message, Message = "Ocorreu um erro na autorização/revogação dos dados" };
            }
            return r;
        }

        public IEnumerable<ContaPagarParcelaViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            ListViewContaPagarAutorizarBI list = new ListViewContaPagarAutorizarBI(this.db, this.seguranca_db);
            return list.PagedList(index, pageSize);
        }
    }
}