using App_Dominio.Component;
using App_Dominio.Contratos;
using DWM.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DWM.Models.Repositories
{
    public abstract class ReportViewModel<R> : ListViewRepository<R, ApplicationContext>, IListReportRepository<R> where R : Repository
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
}