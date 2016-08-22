using App_Dominio.Component;
using App_Dominio.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Threading.Tasks;
using App_Dominio.Contratos;
using DWM.Models.Entidades;
using App_Dominio.Security;
using App_Dominio.Entidades;
using Microsoft.Reporting.WebForms;
using DWM.Models.Repositories;

namespace DWM.Models.Repositories
{
    public abstract class ReportRepository<R> : ListViewRepository<R, ApplicationContext>, IListReportRepository<R> where R : Repository
    {
        public string totalizaColuna1 { get; set; }
        public string totalizaColuna2 { get; set; }

        public abstract IEnumerable<R> BindReport(params object[] param);

        public override IEnumerable<Repository> ListRepository(int? index, int pageSize = 50, params object[] param)
        {
            using (db = this.Create())
            {
                IEnumerable<R> r = Bind(index, pageSize, param);
                if (r.Count() > 0)
                    return LineBreak(r);
                else
                    return r;
            }
        }

        public IEnumerable<R> ListReportRepository(params object[] param)
        {
            using (db = this.Create())
            {
                IEnumerable<R> r = BindReport(param);
                return r;
            }
        }

        public IEnumerable<R> LineBreak(IEnumerable<R> repository)
        {
            int idx = 0;
            object value1 = "@";
            object value2 = "@";
            bool flag = true;

            IList<R> repo = new List<R>();
            repo = repository.ToList();

            foreach (IReportRepository<R> r in repository.ToList())
            {
                #region group
                if (value1.Equals(r.getValueColumn1()))
                    ((IEnumerable<IReportRepository<R>>)repo).ElementAt(idx).ClearColumn1();
                else if (!value1.Equals("@"))
                {
                    #region totaliza sub grupo
                    if (totalizaColuna2 == "S")
                    {
                        R subGroupKey = r.getKey(value1, value2);
                        R subGroup = r.Create(subGroupKey, repository);
                        repo.Insert(idx++, subGroup);
                    }

                    value2 = r.getValueColumn2();
                    #endregion

                    #region totaliza grupo
                    if (totalizaColuna1 == "S")
                    {
                        R groupKey = r.getKey(value1);
                        R group = r.Create(groupKey, repository);
                        repo.Insert(idx++, group);
                    }

                    value1 = r.getValueColumn1();
                    #endregion
                    flag = true;
                }
                else
                {
                    value1 = r.getValueColumn1();
                    value2 = r.getValueColumn2();
                }
                #endregion

                #region sub-group
                if (!flag)
                    if (value2.Equals(r.getValueColumn2()))
                        ((IEnumerable<IReportRepository<R>>)repo).ElementAt(idx).ClearColumn2();
                    else
                    {
                        if (totalizaColuna2 == "S")
                        {
                            R key = r.getKey(value1, value2);
                            R item = r.Create(key, repository);
                            repo.Insert(idx++, item);
                        }

                        value2 = r.getValueColumn2();
                    }

                flag = false;
                #endregion

                idx++;
            }

            #region totaliza sub grupo
            if (totalizaColuna2 == "S")
            {
                R geralSubGroupKey = ((IReportRepository<R>)repo.Last()).getKey(value1, value2);
                R geralSubGroup = ((IReportRepository<R>)repo.Last()).Create(geralSubGroupKey, repository);
                repo.Insert(idx++, geralSubGroup);
            }
            #endregion

            #region totaliza grupo
            if (totalizaColuna1 == "S")
            {
                R geralGroupKey = ((IReportRepository<R>)repo.Last()).getKey(value1);
                R geralGroup = ((IReportRepository<R>)repo.Last()).Create(geralGroupKey, repository);
                repo.Insert(idx++, geralGroup);
            }
            #endregion

            #region total geral
            R geralKey = ((IReportRepository<R>)repo.Last()).getKey();
            R geral = ((IReportRepository<R>)repo.Last()).Create(geralKey, repository);
            repo.Insert(idx++, geral);
            #endregion

            return repo;
        }

    }

    public class DiarioRepository : Repository, IReportRepository<DiarioRepository>
    {
        public int? contabilidadeId { get; set; }
        public DateTime? dt_lancamento { get; set; }
        public int _contabilidadeId { get; set; }
        public DateTime _dt_lancamento { get; set; }
        public string documento { get; set; }
        public int sequencial { get; set; }
        public System.Nullable<int> centroCustoId { get; set; }
        public string descricao_centroCusto { get; set; }
        public string codigoPleno { get; set; }
        public string descricao_planoConta { get; set; }
        public int historicoId { get; set; }
        public string descricao_historico { get; set; }
        public string complementoHist { get; set; }
        public System.Nullable<decimal> vr_debito { get; set; }
        public System.Nullable<decimal> vr_credito { get; set; }
        //public IEnumerable<Repository> r { get; set; }

        #region métodos da Interface
        public object getValueColumn1()
        {
            return dt_lancamento;
        }

        public object getValueColumn2()
        {
            return contabilidadeId;
        }

        public void ClearColumn1()
        {
            dt_lancamento = null;
        }

        public void ClearColumn2()
        {
            contabilidadeId = null;
        }

        public DiarioRepository getKey(object group = null, object subGroup = null)
        {
            return new DiarioRepository() { dt_lancamento = (DateTime?)group, contabilidadeId = (int?)subGroup };
        }

        public DiarioRepository Create(DiarioRepository key, IEnumerable<DiarioRepository> list)
        {
            DiarioRepository d = new DiarioRepository();

            if (key.contabilidadeId == null && key.dt_lancamento == null)
            {
                d.vr_debito = list.Sum(m => m.vr_debito);
                d.vr_credito = list.Sum(m => m.vr_credito);
                d.descricao_historico = "<b>Total geral:</b> ";
            }
            else if (key.contabilidadeId == null) // coluna 2 
            {
                d.vr_debito = list.Where(info => info._dt_lancamento.Equals(key.dt_lancamento)).Sum(m => m.vr_debito);
                d.vr_credito = list.Where(info => info._dt_lancamento.Equals(key.dt_lancamento)).Sum(m => m.vr_credito);
                d.descricao_historico = "<b>Total do dia: </b>"; // grupo
            }
            else if (key.dt_lancamento != null) // coluna 1 
            {
                d.vr_debito = list.Where(info => info._dt_lancamento.Equals(key.dt_lancamento) && info._contabilidadeId == key.contabilidadeId).Sum(m => m.vr_debito);
                d.vr_credito = list.Where(info => info._dt_lancamento.Equals(key.dt_lancamento) && info._contabilidadeId == key.contabilidadeId).Sum(m => m.vr_credito);
                d.descricao_historico = "<b>Total do lançamento:</b> "; // sub-grupo
            }

            return d;
        }

        #endregion
    }
}