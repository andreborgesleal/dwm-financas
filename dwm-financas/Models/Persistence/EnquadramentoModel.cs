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

namespace DWM.Models.Persistence
{
    public class EnquadramentoModel : CrudModel<Enquadramento, EnquadramentoViewModel, ApplicationContext>
    {
       #region Constructor
        public EnquadramentoModel() { }

       public EnquadramentoModel(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            Create(_db, _seguranca_db);
        }
        #endregion

       #region Métodos da classe CrudModel
       public override Enquadramento MapToEntity(EnquadramentoViewModel value)
        {
            Enquadramento e = Find(value);

            if (e != null)
            {
                e.enquadramentoId = value.enquadramentoId;
                e.empresaId = value.empresaId;
                e.descricao = value.descricao;
                e.exercicio = value.exercicio;
                e.EnquadramentoItems.Clear();
            }
            else
                e = new Enquadramento()
                {
                    enquadramentoId = value.enquadramentoId,
                    empresaId = value.empresaId,
                    descricao = value.descricao,
                    exercicio = value.exercicio,
                    EnquadramentoItems = new List<EnquadramentoItem>()
                };

            foreach (EnquadramentoItemViewModel i in value.EnquadramentoItems)
            {
                EnquadramentoItem x = new EnquadramentoItem()
                {
                    enquadramentoId = value.enquadramentoId,
                    sequencial = i.sequencial,
                    centroCustoId = i.centroCustoId,
                    planoContaId = i.planoContaId,
                    historicoId = i.historicoId,
                    complementoHist = i.complementoHist,
                    tipoLancamento = i.tipoLancamento,
                    valor = i.valor
                };

                e.EnquadramentoItems.Add(x);
            }

            return e;
        }

        public override EnquadramentoViewModel MapToRepository(Enquadramento entity)
        {
            EnquadramentoViewModel r = new EnquadramentoViewModel()
            {
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS },
                enquadramentoId = entity.enquadramentoId,
                empresaId = entity.empresaId,
                descricao = entity.descricao,
                exercicio = entity.exercicio,
                EnquadramentoItem = new EnquadramentoItemViewModel(),
                EnquadramentoItems = new List<EnquadramentoItemViewModel>()
            };

            string _codigoPleno = "";

            foreach (EnquadramentoItem i in entity.EnquadramentoItems)
            {
                _codigoPleno = ((ApplicationContext)db).PlanoContas.Find(i.planoContaId).codigoPleno;

                EnquadramentoItemViewModel x = new EnquadramentoItemViewModel()
                {
                    enquadramentoId = r.enquadramentoId,
                    sequencial = i.sequencial,
                    centroCustoId = i.centroCustoId,
                    descricao_centroCusto = i.centroCustoId != null ? db.CentroCustos.Find(i.centroCustoId).descricao : "",
                    planoContaId = i.planoContaId,
                    codigoPleno = _codigoPleno,
                    descricao_planoConta = i.planoContaId != 0 ? db.PlanoContas.Find(i.planoContaId).descricao : "",
                    historicoId = i.historicoId,
                    descricao_historico = i.historicoId != 0 ? db.Historicos.Find(i.historicoId).descricao : "",
                    complementoHist = i.complementoHist,
                    tipoLancamento = i.tipoLancamento,
                    valor = i.valor,
                    mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
                };

                ((List<EnquadramentoItemViewModel>)r.EnquadramentoItems).Add(x);
            }

            return r;
        }

        public override Enquadramento Find(EnquadramentoViewModel key)
        {
            int _exercicio = int.Parse(sessaoCorrente.value1);
            Enquadramento entity = db.Enquadramentos.Find(key.enquadramentoId);
            if (entity != null && (entity.EnquadramentoItems.Count() == 0 || entity.empresaId != sessaoCorrente.empresaId || entity.exercicio != _exercicio))
                return null;

            return entity;
        }

        public override Validate Validate(EnquadramentoViewModel value, Crud operation)
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

            if (value.descricao.Trim().Length == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.MessageBase = MensagemPadrao.Message(5, "Descricao do enquadramento").ToString();
                value.mensagem.Message = "Campo Descrição é de preenchimento obrigatório";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.EnquadramentoItems.Count() == 0)
            {
                value.mensagem.Code = 46;
                value.mensagem.MessageBase = MensagemPadrao.Message(46, "enquadramento").ToString();
                value.mensagem.Message = "Inclua pelo menos um item de enquadramento na lista.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            return value.mensagem;
        }

        public override EnquadramentoViewModel CreateRepository(System.Web.HttpRequestBase Request = null)
        {
            EnquadramentoViewModel r = new EnquadramentoViewModel()
            {
                exercicio = int.Parse(sessaoCorrente.value1),
                empresaId = sessaoCorrente.empresaId,
                EnquadramentoItem = new EnquadramentoItemViewModel(),
                EnquadramentoItems = new List<EnquadramentoItemViewModel>()
            };

            return r;
        }
        #endregion
    }

    public class ListViewEnquadramento : ListViewModel<EnquadramentoViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewEnquadramento() { }
        public ListViewEnquadramento(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe ListViewRepository
        public override IEnumerable<EnquadramentoViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _descricao = param != null && param.Count() > 0 && param[0] != null ? param[0].ToString() : null;
            int _exercicio = int.Parse(sessaoCorrente.value1);
            var q = (from c in db.Enquadramentos
                     where c.empresaId.Equals(sessaoCorrente.empresaId)
                            && c.exercicio == _exercicio
                            && (_descricao == null || String.IsNullOrEmpty(_descricao) || c.descricao.StartsWith(_descricao.Trim()))
                     orderby c.descricao
                     select new EnquadramentoViewModel
                     {
                         enquadramentoId = c.enquadramentoId,
                         empresaId = c.empresaId,
                         exercicio = c.exercicio,
                         descricao = c.descricao,
                         EnquadramentoItems = from i in db.EnquadramentoItems join pc in db.PlanoContas on i.planoContaId equals pc.planoContaId
                                              where i.enquadramentoId == c.enquadramentoId
                                              select new EnquadramentoItemViewModel()
                                              {
                                                  enquadramentoId = i.enquadramentoId,
                                                  sequencial = i.sequencial,
                                                  centroCustoId = i.centroCustoId,
                                                  planoContaId = i.planoContaId,
                                                  codigoPleno = pc.codigoPleno,
                                                  historicoId = i.historicoId,
                                                  complementoHist = i.complementoHist,
                                                  tipoLancamento = i.tipoLancamento,
                                                  valor = i.valor
                                              },
                         PageSize = pageSize,
                         TotalCount = (from c1 in db.Enquadramentos
                                       where c1.empresaId.Equals(sessaoCorrente.empresaId)
                                             && c1.exercicio == _exercicio
                                             && (_descricao == null || String.IsNullOrEmpty(_descricao) || c1.descricao.StartsWith(_descricao.Trim()))
                                       select c1).Count()
                     }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();

            return q;
        }

        public override Repository getRepository(Object id)
        {
            return new EnquadramentoModel(this.db, this.seguranca_db).getObject((EnquadramentoViewModel)id);
        }
        #endregion
    }


    //***************************************************************************************************************************************************************************
    // Este método foi criado para diferenciar o EnquadramentoModal do Contas a pagar e receber do Enquandramento Modal usado na inclusão da contabilidade.
    // A única diferença está no método MOVE do javascript:
    // Na Contabilidade quando o usuário seleciona o enquadramento desejado, o sistema recupera todos os itens do respectivo enquadramento e MOVE para os itens da contabilidade
    // No contas a pagar e receber, o enquadramento será usado exclusivamente para o TYPEAHEAD e quando o usuário selecionar o formulário modal e mandar MOVER (javascript) 
    // a linha selecionada, o sistema irá apenas mover a descrição do enquadramento para o textbox do typeahead
    //***************************************************************************************************************************************************************************


    #region LookupEnquadramento usado na contabilidade
    public class LookupEnquadramentoModel : ListViewEnquadramento
    {
        public override string action()
        {
            return "../Enquadramentos/ListEnquadramentoModal";
        }

        public override string DivId()
        {
            return "div-enq";
        }
    }

    public class LookupEnquadramentoFiltroModel : ListViewEnquadramento
    {
        public override string action()
        {
            return "../Enquadramentos/_ListEnquadramentoModal";
        }

        public override string DivId()
        {
            return "div-enq";
        }
    }
    #endregion

    #region LookupEnquadramento usado no contas a pagar e receber - Inclusão da operação
    public class LookupEnquadramentoOperacaoModel : ListViewEnquadramento
    {
        public override string action()
        {
            return "../Enquadramentos/ListEnquadramentoOperacaoModal";
        }

        public override string DivId()
        {
            return "div-enq";
        }
    }

    public class LookupEnquadramentoOperacaoFiltroModel : ListViewEnquadramento
    {
        public override string action()
        {
            return "../Enquadramentos/_ListEnquadramentoOperacaoModal";
        }

        public override string DivId()
        {
            return "div-enq";
        }
    }
    #endregion

    #region LookupEnquadramento usado no contas a pagar e receber - Amortização
    public class LookupEnquadramentoOperacaoAmortizacaoModel : ListViewEnquadramento
    {
        public override string action()
        {
            return "../Enquadramentos/ListEnquadramentoOperacaoAmortizacaoModal";
        }

        public override string DivId()
        {
            return "div-amort";
        }
    }

    public class LookupEnquadramentoOperacaoAmortizacaoFiltroModel : ListViewEnquadramento
    {
        public override string action()
        {
            return "../Enquadramentos/_ListEnquadramentoOperacaoAmortizacaoModal";
        }

        public override string DivId()
        {
            return "div-amort";
        }
    }
    #endregion
}