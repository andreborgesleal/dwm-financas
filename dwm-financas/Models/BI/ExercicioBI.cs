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
    public class ExercicioBI : DWMContext<ApplicationContext>, IProcess<ExercicioViewModel, ApplicationContext>
    {
        #region Constructor
        public ExercicioBI() { }

        public ExercicioBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public ExercicioViewModel Run(Repository value)
        {
            ExercicioViewModel r = (ExercicioViewModel)value;
            try
            {
                Exercicio exercicio = db.Exercicios.Where(info => info.empresaId == sessaoCorrente.empresaId && info.encerrado == "N").FirstOrDefault();
                r = new ExercicioViewModel()
                {
                    empresaId = sessaoCorrente.empresaId,
                    exercicio = exercicio.exercicio,
                    dt_inicio = exercicio.dt_inicio,
                    dt_fim = exercicio.dt_fim,
                    dt_lancamento_inicio = exercicio.dt_lancamento_inicio,
                    dt_lancamento_fim = exercicio.dt_lancamento_fim,
                    mascaraPc = exercicio.mascaraPc,
                    encerrado = exercicio.encerrado,
                    mensagem = new Validate() { Code = 0, MessageBase = "Registro incluído com sucesso", Message = "Registro incluído com sucesso" }
                };
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