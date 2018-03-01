using System;
using System.Collections.Generic;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using System.Web.Mvc;
using System.Linq;
using App_Dominio.Pattern;
using App_Dominio.Models;

namespace DWM.Models.BI
{
    public class HomeBI : DWMContext<ApplicationContext>, IProcess<HomeViewModel, ApplicationContext>
    {
        #region Constructor
        public HomeBI() { }

        public HomeBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public HomeViewModel Run(Repository value)
        {
            HomeViewModel r = (HomeViewModel)value;
            try
            {
                Factory<ExercicioViewModel, ApplicationContext> facade = new Factory<ExercicioViewModel, ApplicationContext>();
                ExercicioViewModel e = facade.Execute(new ExercicioBI(), new ExercicioViewModel());

                DateTime data1;
                DateTime data2;

                if (e.mensagem.Code == 0 && e.dt_lancamento_inicio.HasValue)
                {
                    data1 = e.dt_lancamento_inicio.Value;
                    data2 = e.dt_lancamento_fim.Value;
                }
                else
                {
                    data1 = Convert.ToDateTime(DateTime.Today.ToString("yyyy-MM-") + "01");
                    data2 = Convert.ToDateTime(DateTime.Today.AddMonths(1).ToString("yyyy-MM-") + "01").AddDays(-1);
                }

                ListViewContaReceberDemonstrativoBI listCob = new ListViewContaReceberDemonstrativoBI();
                //Facade<ContaReceberDemonstrativoViewModel, ContaReceberModel, ApplicationContext> facadeCob = new Facade<ContaReceberDemonstrativoViewModel, ContaReceberModel, ApplicationContext>();

                Factory<ContaReceberDemonstrativoViewModel, ApplicationContext> facadeCob = new Factory<ContaReceberDemonstrativoViewModel, ApplicationContext>();

                r.Cobranca = facadeCob.PagedList(listCob, 0, 15, true,
                                                new DateTime(data1.AddMonths(-2).Year, data1.AddMonths(-2).Month, 1),
                                                data1.Date.AddDays(-1),
                                                true,
                                                data1,
                                                data2,
                                                true,
                                                true,
                                                true,
                                                false,
                                                data1,
                                                data2,
                                                null,
                                                null,
                                                null,
                                                null,
                                                null,
                                                null);


                ListViewContaPagar listPag = new ListViewContaPagar();
                Facade<ContaPagarViewModel, ContaPagarModel, ApplicationContext> facadePag = new Facade<ContaPagarViewModel, ContaPagarModel, ApplicationContext>();
                r.Pagamentos = facadePag.getPagedList(listPag, 0, 15,
                                                true,
                                                new DateTime(data1.AddMonths(-2).Year, data1.AddMonths(-2).Month, 1),
                                                data1.Date.AddDays(-1),
                                                true,
                                                data1,
                                                data2,
                                                true,
                                                true,
                                                true,
                                                false,
                                                data1,
                                                data2,
                                                null,
                                                null,
                                                null,
                                                null,
                                                null,
                                                null);

                r.nome_empresa = db.EmpresaInternas.Find(sessaoCorrente.empresaId).nome.ToString();

                /*
                #region Resumo da Venda
                ListViewResumoVenda modelResumoVenda = new ListViewResumoVenda(this.db, this.seguranca_db);
                r.ResumoVenda = modelResumoVenda.Bind(0, 1000, null, null, null);
                #endregion
                */
            }
            catch (Exception ex)
            {
                r.mensagem = new Validate() { Code = 999, MessageBase = ex.Message, Message = "Ocorreu um erro na recuperação dos dados" };
            }
            return r;
        }

        public IEnumerable<HomeViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }
}