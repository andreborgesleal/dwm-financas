using App_Dominio.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App_Dominio.Entidades;
//using System.Data.Objects.SqlClient;
using DWM.Models.Entidades;
using App_Dominio.Security;
using App_Dominio.Models;
using App_Dominio.App_Start;

namespace DWM.Models.Enumeracoes
{
    public class BindDropDownList
    {
        public IEnumerable<SelectListItem> GrupoFornecedores(params object[] param)
        {
            // params[0] -> cabeçalho (Selecione..., Todos...)
            // params[1] -> SelectedValue
            string cabecalho = param[0].ToString();
            string selectedValue = param[1].ToString();

            EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();

            using (ApplicationContext db = new ApplicationContext())
            {
                Sessao sessao = security.getSessaoCorrente();

                IList<SelectListItem> q = new List<SelectListItem>();

                if (cabecalho != "")
                    q.Add(new SelectListItem() { Value = "", Text = cabecalho });

                q = q.Union(from e in db.GrupoCredores.AsEnumerable()
                            where e.empresaId == sessao.empresaId
                            orderby e.nome
                            select new SelectListItem()
                            {
                                Value = e.grupoCredorId.ToString(),
                                Text = e.nome,
                                Selected = (selectedValue != "" ? e.nome.Equals(selectedValue) : false)
                            }).ToList();

                return q;
            }


        }

        public IEnumerable<SelectListItem> GrupoClientes(params object[] param)
        {
            // params[0] -> cabeçalho (Selecione..., Todos...)
            // params[1] -> SelectedValue
            string cabecalho = param[0].ToString();
            string selectedValue = param[1].ToString();

            EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();

            using (ApplicationContext db = new ApplicationContext())
            {
                Sessao sessao = security.getSessaoCorrente();
                
                IList<SelectListItem> q = new List<SelectListItem>();

                if (cabecalho != "")
                    q.Add(new SelectListItem() { Value = "", Text = cabecalho });

                q = q.Union(from e in db.GrupoClientes.AsEnumerable()
                            where e.empresaId == sessao.empresaId
                            orderby e.nome
                            select new SelectListItem()
                            {
                                Value = e.grupoClienteId.ToString(),
                                Text = e.nome,
                                Selected = (selectedValue != "" ? e.nome.Equals(selectedValue) : false)
                            }).ToList();

                return q;
            }
        }

        public IEnumerable<SelectListItem> GrupoClientesParaCobranca(params object[] param)
        {
            // params[] -> SelectedValue
            string selectedValue = param[0].ToString();

            EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();

            using (ApplicationContext db = new ApplicationContext())
            {
                Sessao sessao = security.getSessaoCorrente();

                IList<SelectListItem> q = new List<SelectListItem>();

                q.Add(new SelectListItem() { Value = "-1", Text = "Nenhum pagador..." });
                q.Add(new SelectListItem() { Value = "0", Text = "Todos os pagadores..." });

                q = q.Union(from e in db.GrupoClientes.AsEnumerable()
                            where e.empresaId == sessao.empresaId
                            orderby e.nome
                            select new SelectListItem()
                            {
                                Value = e.grupoClienteId.ToString(),
                                Text = e.nome,
                                Selected = (selectedValue != "" ? e.nome.Equals(selectedValue) : false)
                            }).ToList();

                return q;
            }
        }

        public IEnumerable<SelectListItem> GrupoCobrancas(params object[] param)
        {
            // params[0] -> cabeçalho (Selecione..., Todos...)
            // params[1] -> SelectedValue
            string cabecalho = param[0].ToString();
            string selectedValue = param[1].ToString();

            EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();

            using (ApplicationContext db = new ApplicationContext())
            {
                Sessao sessao = security.getSessaoCorrente();

                IList<SelectListItem> q = new List<SelectListItem>();

                if (cabecalho != "")
                    q.Add(new SelectListItem() { Value = "", Text = cabecalho });

                q = q.Union(from e in db.GrupoCobrancas.AsEnumerable()
                            where e.empresaId == sessao.empresaId
                            orderby e.descricao
                            select new SelectListItem()
                            {
                                Value = e.grupoCobrancaId.ToString(),
                                Text = e.descricao,
                                Selected = (selectedValue != "" ? e.descricao.Equals(selectedValue) : false)
                            }).ToList();

                return q;
            }
        }

        #region DropDownList Periodo
        /// <summary>
        /// Retorna o período para processamento de datas
        /// </summary>
        /// <param name="selectedValue">Item da lista que receberá o foco inicial</param>
        /// <param name="header">Informar o cabeçalho do dropdownlist. Exemplo: "Selecione...". Observação: Se não informado o dropdownlist não terá cabeçalho.</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> Periodo(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = "30", Text = "Últimos 30 dias" }, 
                new SelectListItem() { Value = "3", Text = "Últimos 3 meses" }, 
                new SelectListItem() { Value = "6", Text = "Últimos 6 meses" },
                new SelectListItem() { Value = "12", Text = "Últimos 12 meses" }, 
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList TipoConta (Sintética ou Analítica)
        /// <summary>
        /// Retorna o tipo da Conta (Sintética ou Analítica)
        /// </summary>
        /// <param name="selectedValue">Item da lista que receberá o foco inicial</param>
        /// <param name="header">Informar o cabeçalho do dropdownlist. Exemplo: "Selecione...". Observação: Se não informado o dropdownlist não terá cabeçalho.</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> TipoConta(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = Enumeradores.TipoContaContabil.SINTETICA.GetStringValue(), Text = Enumeradores.TipoContaContabil.SINTETICA.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.TipoContaContabil.ANALITICA.GetStringValue(), Text = Enumeradores.TipoContaContabil.ANALITICA.GetStringDescription()  } 
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList Tipos de Historicos
        public IEnumerable<SelectListItem> TipoHistorico(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = Enumeradores.TipoHistorico.CONTABILIDADE.GetStringValue(), Text = Enumeradores.TipoHistorico.CONTABILIDADE.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.TipoHistorico.PAGAR.GetStringValue(), Text = Enumeradores.TipoHistorico.PAGAR.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.TipoHistorico.RECEBER.GetStringValue(), Text = Enumeradores.TipoHistorico.RECEBER.GetStringDescription()  }
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList Forma de pagamento
        public IEnumerable<SelectListItem> FormaPagamento(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = Enumeradores.FormaPagamento.DINHEIRO.GetStringValue(), Text = Enumeradores.FormaPagamento.DINHEIRO.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.FormaPagamento.CHEQUE.GetStringValue(), Text = Enumeradores.FormaPagamento.CHEQUE.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.FormaPagamento.CARTAO.GetStringValue(), Text = Enumeradores.FormaPagamento.CARTAO.GetStringDescription() },
                new SelectListItem() { Value = Enumeradores.FormaPagamento.BOLETO.GetStringValue(), Text = Enumeradores.FormaPagamento.BOLETO.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.FormaPagamento.TED_DOC.GetStringValue(), Text = Enumeradores.FormaPagamento.TED_DOC.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.FormaPagamento.PAG_SEGURO.GetStringValue(), Text = Enumeradores.FormaPagamento.PAG_SEGURO.GetStringDescription() },
                new SelectListItem() { Value = Enumeradores.FormaPagamento.OUTROS.GetStringValue(), Text = Enumeradores.FormaPagamento.OUTROS.GetStringDescription()  }
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList Cassificacao Banco
        public IEnumerable<SelectListItem> ClassificacaoBanco(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = Enumeradores.ClassificacaoBanco.CONTA_CORRENTE.GetStringValue(), Text = Enumeradores.ClassificacaoBanco.CONTA_CORRENTE.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.ClassificacaoBanco.POUPANCA.GetStringValue(), Text = Enumeradores.ClassificacaoBanco.POUPANCA.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.ClassificacaoBanco.INVESTIMENTO.GetStringValue(), Text = Enumeradores.ClassificacaoBanco.INVESTIMENTO.GetStringDescription()  }
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList Tipos de Eventos
        public IEnumerable<SelectListItem> TipoEvento(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = Enumeradores.TipoEvento.AMORTIZACAO.GetStringValue(), Text = Enumeradores.TipoEvento.AMORTIZACAO.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.TipoEvento.BAIXA_DESCONTO.GetStringValue(), Text = Enumeradores.TipoEvento.BAIXA_DESCONTO.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.TipoEvento.CANCELAMENTO.GetStringValue(), Text = Enumeradores.TipoEvento.CANCELAMENTO.GetStringDescription()  },
                new SelectListItem() { Value = Enumeradores.TipoEvento.DESCONTO.GetStringValue(), Text = Enumeradores.TipoEvento.DESCONTO.GetStringDescription()  },
                new SelectListItem() { Value = Enumeradores.TipoEvento.ENCARGOS.GetStringValue(), Text = Enumeradores.TipoEvento.ENCARGOS.GetStringDescription()  },
                new SelectListItem() { Value = Enumeradores.TipoEvento.ESTORNO.GetStringValue(), Text = Enumeradores.TipoEvento.ESTORNO.GetStringDescription()  },
                new SelectListItem() { Value = Enumeradores.TipoEvento.INCLUSAO_OPERACAO.GetStringValue(), Text = Enumeradores.TipoEvento.INCLUSAO_OPERACAO.GetStringDescription()  },
                new SelectListItem() { Value = Enumeradores.TipoEvento.LIQUIDACAO.GetStringValue(), Text = Enumeradores.TipoEvento.LIQUIDACAO.GetStringDescription()  },
                new SelectListItem() { Value = Enumeradores.TipoEvento.RENEGOCIACAO.GetStringValue(), Text = Enumeradores.TipoEvento.RENEGOCIACAO.GetStringDescription()  }
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList Modalidade
        public IEnumerable<SelectListItem> Modalidade(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = Enumeradores.Modalidade.PAGAR.GetStringValue(), Text = Enumeradores.Modalidade.PAGAR.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.Modalidade.RECEBER.GetStringValue(), Text = Enumeradores.Modalidade.RECEBER.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.Modalidade.COBRANCA.GetStringValue(), Text = Enumeradores.Modalidade.COBRANCA.GetStringDescription() } 
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList Exercicio
        public IEnumerable<SelectListItem> Exercicio(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>();

            for (int i=2016; i <= 2050; i++)
            {
                SelectListItem s = new SelectListItem()
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                };
                drp.Add(s);
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList Tipo Balancete Mensal
        public IEnumerable<SelectListItem> TipoBalanceteMensal(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() {
                new SelectListItem() { Value = Enumeradores.TipoBalanceteMensal.SALDO_MENSAL.GetStringValue(), Text = Enumeradores.TipoBalanceteMensal.SALDO_MENSAL.GetStringDescription() },
                new SelectListItem() { Value = Enumeradores.TipoBalanceteMensal.TOTAIS_DEB_CRED.GetStringValue(), Text = Enumeradores.TipoBalanceteMensal.TOTAIS_DEB_CRED.GetStringDescription()  }
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList Layout CNAB
        public IEnumerable<SelectListItem> LayoutCNAB(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() {
                new SelectListItem() { Value = Enumeradores.LayoutCNAB.CNAB240.GetStringValue(), Text = Enumeradores.LayoutCNAB.CNAB240.GetStringDescription() },
                new SelectListItem() { Value = Enumeradores.LayoutCNAB.CNAB400.GetStringValue(), Text = Enumeradores.LayoutCNAB.CNAB400.GetStringDescription()  }
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList Convenios
        public IEnumerable<SelectListItem> Convenios(params object[] param)
        {
            // params[] -> SelectedValue
            string _BancoID = param[0].ToString();
            string _ConvenioID = param[1].ToString();

            EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();

            using (ApplicationContext db = new ApplicationContext())
            {
                Sessao sessao = security.getSessaoCorrente();

                IList<SelectListItem> q = new List<SelectListItem>();

                q = (from conv in db.Convenios.AsEnumerable()
                     where conv.empresaId == sessao.empresaId
                     orderby conv.NomeBanco
                     select new SelectListItem()
                     {
                         Value = conv.BancoID + "|" + conv.ConvenioID,
                         Text = conv.BancoID + "-" + conv.NomeBanco + " Convênio: " + conv.ConvenioID,
                         Selected = (!String.IsNullOrEmpty(_BancoID) ? conv.BancoID.Equals(_BancoID) && conv.ConvenioID.Equals(_ConvenioID) : false)
                     }).ToList();

                return q;
            }
        }
        #endregion
    }
}