using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using DWM.Models.Entidades;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DWM.Models.Enumeracoes;
using System.Data.Entity;

namespace DWM.Models.BI
{
    public class ExercicioAbrirBI : DWMContext<ApplicationContext>, IProcess<ExercicioViewModel, ApplicationContext>
    {
        #region Constructor
        public ExercicioAbrirBI() { }

        public ExercicioAbrirBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public ExercicioViewModel Run(Repository value)
        {
            ExercicioViewModel r = (ExercicioViewModel)value;
            try
            {
                Parametro param = db.Parametros.Find((int)Enumeradores.Param.EXERCICIO_CONTABIL, sessaoCorrente.empresaId);
                param.valor = r.exercicio.ToString();
                db.Entry(param).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                r.mensagem = new Validate() { Code = 999, MessageBase = ex.Message, Message = "Ocorreu um erro na recuperação dos dados" };
            }
            return r;
        }

        public IEnumerable<ExercicioViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }


    }
}