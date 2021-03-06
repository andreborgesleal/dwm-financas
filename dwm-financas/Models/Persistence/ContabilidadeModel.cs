﻿using System;
using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using System.Web.Mvc;
using App_Dominio.Models;
using System.Data.Entity;

namespace DWM.Models.Persistence
{
    public class ContabilidadeModel : CrudModel<Contabilidade, ContabilidadeViewModel, ApplicationContext>, IProcess<ContabilidadeViewModel, ApplicationContext>
    {
        #region Constructor
        public ContabilidadeModel() { }
        public ContabilidadeModel(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        private int? operacaoId { get; set; }
        private int? parcelaId { get; set; }
        private string dt_evento { get; set; }
        private string natureza { get; set; }

        #region Métodos da classe CrudContext
        public override ContabilidadeViewModel AfterInsert(ContabilidadeViewModel value)
        {
            if (operacaoId.HasValue && natureza=="P")
            {
                ContaPagarParcelaEvento entity = db.ContaPagarParcelaEventos.Find(operacaoId, parcelaId, Convert.ToDateTime(dt_evento));
                if (entity != null)
                {
                    entity.contabilidadeId = value.contabilidadeId;
                    db.Entry(entity).State = EntityState.Modified;
                }
            }
            else if (operacaoId.HasValue && natureza == "R")
            {
                ContaReceberParcelaEvento entity = db.ContaReceberParcelaEventos.Find(operacaoId, parcelaId, Convert.ToDateTime(dt_evento));
                if (entity != null)
                {
                    entity.contabilidadeId = value.contabilidadeId;
                    db.Entry(entity).State = EntityState.Modified;
                }
            }

            return base.AfterInsert(value);
        }

        public override Contabilidade MapToEntity(ContabilidadeViewModel value)
        {
            Contabilidade c = Find(value);

            if (c != null)
            {
                c.contabilidadeId = value.contabilidadeId;
                c.empresaId = value.empresaId;
                c.dt_lancamento = value.dt_lancamento;
                c.documento= value.documento;
                c.exercicio = value.exercicio;
                c.ContabilidadeItems.Clear();
            }
            else
                c = new Contabilidade()
                {
                    contabilidadeId = value.contabilidadeId,
                    empresaId = value.empresaId,
                    dt_lancamento = value.dt_lancamento,
                    documento = value.documento,
                    exercicio = value.exercicio,
                    ContabilidadeItems = new List<ContabilidadeItem>()
                };

            foreach (ContabilidadeItemViewModel i in value.ContabilidadeItems)
            {
                ContabilidadeItem x = new ContabilidadeItem()
                {
                    contabilidadeId = value.contabilidadeId,
                    sequencial = i.sequencial,
                    centroCustoId = i.centroCustoId,
                    planoContaId = i.planoContaId,
                    historicoId = i.historicoId,
                    complementoHist = i.complementoHist,
                    tipoLancamento = i.tipoLancamento,
                    valor = i.valor,
                    DocumentoURL = i.DocumentoURL
                };

                c.ContabilidadeItems.Add(x);
            }

            if (value.operacaoId.HasValue)
            {
                operacaoId = value.operacaoId;
                parcelaId = value.parcelaId;
                dt_evento = value.dt_evento;
                natureza = value.natureza;
            };

            return c;
        }

        public override ContabilidadeViewModel MapToRepository(Contabilidade entity)
        {
            ContabilidadeViewModel r = new ContabilidadeViewModel()
            {
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS },
                contabilidadeId = entity.contabilidadeId,
                empresaId = entity.empresaId,
                dt_lancamento = entity.dt_lancamento,
                documento = entity.documento,
                exercicio = entity.exercicio,
                ContabilidadeItem = new ContabilidadeItemViewModel(),
                ContabilidadeItems = new List<ContabilidadeItemViewModel>()
            };

            string _codigoPleno = "";

            foreach (ContabilidadeItem i in entity.ContabilidadeItems)
            {
                _codigoPleno = db.PlanoContas.Find(i.planoContaId).codigoPleno;

                ContabilidadeItemViewModel x = new ContabilidadeItemViewModel()
                {
                    mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS },
                    contabilidadeId = r.contabilidadeId,
                    sequencial = i.sequencial,
                    centroCustoId = i.centroCustoId,
                    descricao_centroCusto = i.centroCustoId != null ? db.CentroCustos.Find(i.centroCustoId.Value).descricao : "",
                    planoContaId = i.planoContaId,
                    codigoPleno = _codigoPleno,
                    codigoReduzido = db.PlanoContas.Find(i.planoContaId).codigoReduzido,
                    descricao_planoConta = db.PlanoContas.Find(i.planoContaId).descricao,
                    historicoId = i.historicoId,
                    descricao_historico = db.Historicos.Find(i.historicoId).descricao,
                    complementoHist = i.complementoHist,
                    tipoLancamento = i.tipoLancamento,
                    valor = i.valor,
                    DocumentoURL = i.DocumentoURL
                };

                ((List<ContabilidadeItemViewModel>)r.ContabilidadeItems).Add(x);
            }

            #region Verifica se o registro contábil está vinculado a um movimento financeiro
            if (db.ContaRecebers.Where(info => info.empresaId == sessaoCorrente.empresaId && info.contabilidadeId == entity.contabilidadeId).Count() > 0)
            {
                r.operacaoId = db.ContaRecebers.Where(info => info.empresaId == sessaoCorrente.empresaId && info.contabilidadeId == entity.contabilidadeId).Select(info => info.operacaoId).FirstOrDefault();
                r.parcelaId = 1;
                r.natureza = "R"; // contas a receber
            }
            else if (db.ContaReceberParcelaEventos.Where(info => info.contabilidadeId == entity.contabilidadeId).Count() > 0)
            {
                r.operacaoId = db.ContaReceberParcelaEventos.Where(info => info.contabilidadeId == entity.contabilidadeId).Select(info => info.operacaoId).FirstOrDefault();
                r.parcelaId = db.ContaReceberParcelaEventos.Where(info => info.contabilidadeId == entity.contabilidadeId).Select(info => info.parcelaId).FirstOrDefault();
                r.natureza = "R"; // contas a receber
            }
            else if (db.ContaPagars.Where(info => info.empresaId == sessaoCorrente.empresaId && info.contabilidadeId == entity.contabilidadeId).Count() > 0)
            {
                r.operacaoId = db.ContaPagars.Where(info => info.empresaId == sessaoCorrente.empresaId && info.contabilidadeId == entity.contabilidadeId).Select(info => info.operacaoId).FirstOrDefault();
                r.parcelaId = 1;
                r.natureza = "P";
            }
            else if (db.ContaPagarParcelaEventos.Where(info => info.contabilidadeId == entity.contabilidadeId).Count() > 0)
            {
                r.operacaoId = db.ContaPagarParcelaEventos.Where(info => info.contabilidadeId == entity.contabilidadeId).Select(info => info.operacaoId).FirstOrDefault();
                r.parcelaId = db.ContaPagarParcelaEventos.Where(info => info.contabilidadeId == entity.contabilidadeId).Select(info => info.parcelaId).FirstOrDefault();
                r.natureza = "P";
            }
            #endregion

            return r;
        }

        public override Contabilidade Find(ContabilidadeViewModel key)
        {
            //int _exercicio = int.Parse(sessaoCorrente.value1);
            int _exercicio = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EXERCICIO_CONTABIL, sessaoCorrente.empresaId).valor);

            Contabilidade entity = db.Contabilidades.Find(key.contabilidadeId);
            if (entity != null && (entity.ContabilidadeItems.Count() == 0 || entity.empresaId != sessaoCorrente.empresaId || entity.exercicio != _exercicio))
                return null;

            return entity;
        }

        public override Validate Validate(ContabilidadeViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };

            if (value.empresaId == 0)
            {
                value.mensagem.Code = 35;
                value.mensagem.MessageBase = MensagemPadrao.Message(35).ToString();
                value.mensagem.Message = "Sua sessão expirou. Faça um novo login no sistema";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.exercicio == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.MessageBase = MensagemPadrao.Message(5, "Exercício").ToString();
                value.mensagem.Message = "Sua sessão expirou. Faça um novo login no sistema";
                value.mensagem.MessageType = MsgType.WARNING;

                return value.mensagem;
            }

            // verifica se a data de lançamento está dentro da faixa permitida do exercício contábil
            string[] periodo = ValidaDataLancamento(value.dt_lancamento);
            if (periodo[0] != "")
            {
                value.mensagem.Code = 48;
                value.mensagem.MessageBase = MensagemPadrao.Message(48, "Dt.Lançamento", periodo[0], periodo[1]).ToString();
                value.mensagem.Message = "Dt. Lançamento fora do exercício contábil";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.ContabilidadeItems.Count() == 0)
            {
                value.mensagem.Code = 46;
                value.mensagem.MessageBase = MensagemPadrao.Message(46, "contabilidade").ToString();
                value.mensagem.Message = "Inclua pelo menos um item de contabilidade na lista.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            // verifica se há algum item com valor nulo
            foreach (ContabilidadeItemViewModel item in value.ContabilidadeItems)
                if (item.valor < 0)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.MessageBase = MensagemPadrao.Message(5, "Valor do Lançamento").ToString();
                    value.mensagem.Message = "Existem itens do lançamento contábil com valor negativo.";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

            return value.mensagem;
        }

        public override ContabilidadeViewModel CreateRepository(System.Web.HttpRequestBase Request = null)
        {
            //int _exercicio = int.Parse(sessaoCorrente.value1);
            int _exercicio = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EXERCICIO_CONTABIL, sessaoCorrente.empresaId).valor);

            DateTime d = DateTime.Today;
            if (db.Contabilidades.ToList().Where(info => info.empresaId.Equals(sessaoCorrente.empresaId) && info.exercicio == _exercicio).Count() > 0)
                d = db.Contabilidades.ToList().Where(info => info.empresaId.Equals(sessaoCorrente.empresaId) && info.exercicio == _exercicio).OrderByDescending(ord => ord.contabilidadeId).Take(1).Last().dt_lancamento;

            ContabilidadeViewModel r = new ContabilidadeViewModel()
            {
                exercicio = _exercicio,
                empresaId = sessaoCorrente.empresaId,
                dt_lancamento = d,
                ContabilidadeItem = new ContabilidadeItemViewModel(),
                ContabilidadeItems = new List<ContabilidadeItemViewModel>()
            };

            if (Request != null && Request["operacaoId"] != null)
            {
                r.natureza = Request["natureza"];
                r.operacaoId = int.Parse(Request["operacaoId"]);
                r.parcelaId = int.Parse(Request["parcelaId"]);
                r.dt_evento = Request["dt_evento"];
            }

            return r;
        }
        #endregion

        #region Métodos da Interface IProcess
        public ContabilidadeViewModel Run(Repository value)
        {
            return CreateRepositoryFromEnquadramento((EnquadramentoViewModel)value);
        }
        public IEnumerable<ContabilidadeViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }
        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Métodos Customizados
        public ContabilidadeViewModel CreateRepositoryFromEnquadramento(EnquadramentoViewModel id)
        {
            ContabilidadeViewModel value = CreateRepository();
            EnquadramentoViewModel enq = (EnquadramentoViewModel)new ListViewEnquadramento(this.db, this.seguranca_db).getRepository(id);
            foreach (EnquadramentoItemViewModel i in enq.EnquadramentoItems)
            {
                ContabilidadeItemViewModel x = new ContabilidadeItemViewModel()
                {
                    sequencial = i.sequencial,
                    centroCustoId = i.centroCustoId,
                    descricao_centroCusto = i.descricao_centroCusto,
                    planoContaId = i.planoContaId,
                    codigoPleno = i.codigoPleno,
                    descricao_planoConta = i.descricao_planoConta,
                    historicoId = i.historicoId,
                    descricao_historico = i.descricao_historico,
                    complementoHist = i.complementoHist,
                    tipoLancamento = i.tipoLancamento,
                    valor = i.valor ?? 0
                };

                ((List<ContabilidadeItemViewModel>)value.ContabilidadeItems).Add(x);
            }

            return value;
        }

        /// <summary>
        /// Valida a data do lançamento dentro do período do exercício contábil corrente
        /// </summary>
        /// <param name="data">Data a ser validada</param>
        /// <returns>Retorna null se a data estiver dentro de um período válido e caso a data esteja fora do período, retorna um vetor com a data inicial e data final do exercício corrente</returns>
        public string[] ValidaDataLancamento(DateTime? data)
        {
            string[] value = { "", "" };

            int _exercicio = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EXERCICIO_CONTABIL, sessaoCorrente.empresaId).valor);

            Exercicio exe = db.Exercicios.Find(sessaoCorrente.empresaId, _exercicio);

            if (exe == null)
                throw new Exception("Exercício contábil não encontrado");

            if (data < exe.dt_inicio || data > exe.dt_fim)
            {
                value[0] = exe.dt_inicio.ToString("dd/MM/yyyy");
                value[1] = exe.dt_fim.ToString("dd/MM/yyyy");
            }

            if (exe.dt_lancamento_inicio.HasValue && (data < exe.dt_lancamento_inicio.Value || data > exe.dt_lancamento_fim.Value))
            {
                value[0] = exe.dt_lancamento_inicio.Value.ToString("dd/MM/yyyy");
                value[1] = exe.dt_lancamento_fim.Value.ToString("dd/MM/yyyy");
            }

            return value;
        }

        #endregion
    }

    public class ListViewContabilidade : ListViewModel<ContabilidadeViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewContabilidade() { }
        public ListViewContabilidade(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe ListViewRepository
        public override IEnumerable<ContabilidadeViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            // param => periodo, data1, data2, planoContaId, centroCustoId, historicoId 
            int? planoContaId = (int?)param[2];
            int? centroCustoId = (int?)param[3];
            int? historicoId = (int?)param[4];
            int? contabilidadeId = (int?)param[5];
            DateTime dt1 = Convert.ToDateTime(param[0].ToString());
            DateTime dt2 = Convert.ToDateTime(param[1].ToString());

            //int _exercicio = int.Parse(sessaoCorrente.value1);
            int _exercicio = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EXERCICIO_CONTABIL, sessaoCorrente.empresaId).valor);

            #region LINQ
            var q = (from c in db.Contabilidades
                     where c.empresaId.Equals(sessaoCorrente.empresaId)
                            && c.exercicio == _exercicio
                            && (c.contabilidadeId == contabilidadeId ||
                                (contabilidadeId == null
                                 && c.dt_lancamento >= dt1 && c.dt_lancamento <= dt2
                                 && (from item in db.ContabilidadeItems
                                     where (item.contabilidadeId == c.contabilidadeId)
                                            && (planoContaId == null || item.planoContaId == planoContaId)
                                            && (centroCustoId == null || item.centroCustoId == centroCustoId)
                                            && (historicoId == null || item.historicoId == historicoId)
                                     select item).Count() > 0))
                     orderby c.dt_lancamento
                     select new ContabilidadeViewModel
                     {
                         contabilidadeId = c.contabilidadeId,
                         empresaId = c.empresaId,
                         exercicio = c.exercicio,
                         dt_lancamento = c.dt_lancamento,
                         documento = c.documento,
                         ContabilidadeItems = from i in db.ContabilidadeItems
                                              join cc in db.CentroCustos on i.centroCustoId equals cc.centroCustoId into CC
                                              from cc in CC.DefaultIfEmpty()
                                              join his in db.Historicos on i.historicoId equals his.historicoId
                                              join pc in db.PlanoContas on i.planoContaId equals pc.planoContaId
                                              where i.contabilidadeId == c.contabilidadeId
                                              select new ContabilidadeItemViewModel()
                                              {
                                                  contabilidadeId = i.contabilidadeId,
                                                  sequencial = i.sequencial,
                                                  centroCustoId = i.centroCustoId,
                                                  descricao_centroCusto =  cc.descricao,
                                                  planoContaId = i.planoContaId,
                                                  codigoPleno = pc.codigoPleno,
                                                  descricao_planoConta = pc.descricao,
                                                  historicoId = i.historicoId,
                                                  descricao_historico = his.descricao,
                                                  complementoHist = i.complementoHist,
                                                  tipoLancamento = i.tipoLancamento,
                                                  valor = i.valor,
                                                  DocumentoURL = i.DocumentoURL
                                              },
                         PageSize = pageSize,
                         TotalCount = 0
                         //TotalCount = (from c1 in db.Contabilidades
                         //              where c1.empresaId.Equals(sessaoCorrente.empresaId)
                         //                    && c1.exercicio == _exercicio
                         //                    && (c1.contabilidadeId == contabilidadeId ||
                         //                       (contabilidadeId == null
                         //                        && c1.dt_lancamento >= dt1 && c1.dt_lancamento <= dt2
                         //                        && (from item in db.ContabilidadeItems
                         //                            where (item.contabilidadeId == c1.contabilidadeId)
                         //                                   && (planoContaId == null || item.planoContaId == planoContaId)
                         //                                   && (centroCustoId == null || item.centroCustoId == centroCustoId)
                         //                                   && (historicoId == null || item.historicoId == historicoId)
                         //                            select item).Count() > 0))
                         //              select c1).Count()
                     }).ToList();
            #endregion

            return q;
        }

        public override Repository getRepository(Object id)
        {
            return new ContabilidadeModel(this.db, this.seguranca_db).getObject((ContabilidadeViewModel)id);
        }

        public override string action()
        {
            return "ListParam";
        }
        #endregion

    }
}
