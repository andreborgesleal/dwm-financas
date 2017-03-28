using System;
using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.App_Start;
using System.Data.Entity.SqlServer;

namespace DWM.Models.Persistence
{
    public class PlanoContaModel : CrudContext<PlanoConta, PlanoContaViewModel, ApplicationContext>
    {
        #region Métodos da classe CrudContext
        public override PlanoConta MapToEntity(PlanoContaViewModel value)
        {
            return new PlanoConta()
            {
                planoContaId = value.planoContaId,
                descricao = value.descricao,
                exercicio = value.exercicio,
                codigoPleno = value.codigoPleno,
                codigoReduzido = value.codigoReduzido,
                planoContaId_pai = value.planoContaId_pai,
                tipoConta = value.tipoConta,
                vr_saldo_inicial = value.tipoConta == "A" ? value.vr_saldo_inicial : null,
                empresaId = value.empresaId
            };
        }

        public override PlanoContaViewModel MapToRepository(PlanoConta entity)
        {
            PlanoConta planoContaPai = new PlanoConta();
            if (entity.planoContaId_pai.HasValue)
                planoContaPai = db.PlanoContas.Find(entity.planoContaId_pai);

            //int _exercicio = int.Parse(sessaoCorrente.value1);
            int _exercicio = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EXERCICIO_CONTABIL, sessaoCorrente.empresaId).valor);

            return new PlanoContaViewModel()
            {
                planoContaId = entity.planoContaId,
                exercicio = entity.exercicio,
                codigoPleno = entity.codigoPleno,
                codigoReduzido = entity.codigoReduzido,
                planoContaId_pai = entity.planoContaId_pai,
                descricao = entity.descricao,
                descricao_pai = planoContaPai.descricao,
                tipoConta = entity.tipoConta,
                vr_saldo_inicial = entity.vr_saldo_inicial,
                empresaId = entity.empresaId,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS },
                mascaraPC = db.Exercicios.Where(m => m.empresaId.Equals(sessaoCorrente.empresaId) && m.exercicio == _exercicio).Count() > 0 ?
                                                 db.Exercicios.Where(m => m.empresaId.Equals(sessaoCorrente.empresaId) &&
                                                                          m.exercicio == _exercicio).Select(code => code.mascaraPc).First() : "9.99.999.999"
            };
        }

        public override PlanoConta Find(PlanoContaViewModel key)
        {
            return db.PlanoContas.Find(key.planoContaId);
        }

        public override Validate Validate(PlanoContaViewModel value, Crud operation)
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

            // Verifica se o código pleno está dentro do formato adequado
            if (int.Parse(value.codigoPleno.Split('.')[0]) == 0)
            {
                value.mensagem.Code = 4;
                value.mensagem.MessageBase = MensagemPadrao.Message(4, "Código Pleno", value.codigoPleno).ToString();
                value.mensagem.Message = "Código pleno não está no formato correto";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.descricao.Trim().Length == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.MessageBase = MensagemPadrao.Message(5, "Descricao da conta").ToString();
                value.mensagem.Message = "Campo Descrição é de preenchimento obrigatório";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.tipoConta.Equals("A") && !value.planoContaId_pai.HasValue)
            {
                value.mensagem.Code = 5;
                value.mensagem.MessageBase = MensagemPadrao.Message(5, "Plano Conta Pai").ToString();
                value.mensagem.Message = MensagemPadrao.Message(33).ToString();
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (operation == Crud.INCLUIR)
            {
                int codigoPleno = (from c in db.PlanoContas
                                   where c.empresaId.Equals(value.empresaId)
                                         && c.exercicio == value.exercicio
                                         && c.codigoPleno.Equals(value.codigoPleno)
                                   select c.codigoPleno).Count();
                if (codigoPleno > 0)
                {
                    value.mensagem.Code = 40;
                    value.mensagem.MessageBase = MensagemPadrao.Message(40, "Código Pleno").ToString();
                    value.mensagem.Message = "Código pleno já existente";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }


                int codigoReduz = (from c in db.PlanoContas
                                   where c.empresaId.Equals(value.empresaId)
                                         && c.exercicio == value.exercicio
                                         && c.codigoReduzido.Equals(value.codigoReduzido)
                                   select c.codigoReduzido).Count();
                if (codigoReduz > 0)
                {
                    value.mensagem.Code = 40;
                    value.mensagem.MessageBase = MensagemPadrao.Message(40, "Código Reduzido").ToString();
                    value.mensagem.Message = "Código reduzido já existente";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                int nomePlanoConta = (from c in db.PlanoContas
                                      where c.empresaId.Equals(value.empresaId)
                                            && c.exercicio.Equals(value.exercicio)
                                            && c.descricao.Equals(value.descricao)
                                      select c.descricao).Count();
                if (nomePlanoConta > 0)
                {
                    value.mensagem.Code = 40;
                    value.mensagem.MessageBase = MensagemPadrao.Message(40, "Descrição").ToString();
                    value.mensagem.Message = "Descrição da conta já existente";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
            }
            else if (operation == Crud.ALTERAR)
            {
                int codigoPleno = (from c in db.PlanoContas
                                   where c.planoContaId != value.planoContaId
                                         && c.empresaId.Equals(value.empresaId)
                                         && c.exercicio == value.exercicio
                                         && c.codigoPleno.Equals(value.codigoPleno)
                                   select c.codigoPleno).Count();
                if (codigoPleno > 0)
                {
                    value.mensagem.Code = 40;
                    value.mensagem.MessageBase = MensagemPadrao.Message(40, "Código Pleno").ToString();
                    value.mensagem.Message = "Código pleno já existente";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }


                int codigoReduz = (from c in db.PlanoContas
                                   where c.planoContaId != value.planoContaId
                                         && c.empresaId.Equals(value.empresaId)
                                         && c.exercicio == value.exercicio
                                         && c.codigoReduzido.Equals(value.codigoReduzido)
                                   select c.codigoReduzido).Count();
                if (codigoReduz > 0)
                {
                    value.mensagem.Code = 40;
                    value.mensagem.MessageBase = MensagemPadrao.Message(40, "Descrição").ToString();
                    value.mensagem.Message = "Código reduzido já existente";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                int nomePlanoConta = (from c in db.PlanoContas
                                      where c.planoContaId != value.planoContaId
                                            && c.empresaId.Equals(value.empresaId)
                                            && c.exercicio == value.exercicio
                                            && c.descricao.Equals(value.descricao)
                                      select c.descricao).Count();
                if (nomePlanoConta > 0)
                {
                    value.mensagem.Code = 40;
                    value.mensagem.MessageBase = MensagemPadrao.Message(40, "Descrição").ToString();
                    value.mensagem.Message = "Descrição da conta já existente";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.planoContaId == value.planoContaId_pai)
                {
                    value.mensagem.Code = 4;
                    value.mensagem.MessageBase = MensagemPadrao.Message(4, "Plano Conta Pai", "Uma conta não pode estar vinculada a ela mesmo. Por favor, selecione outro Plano Conta Pai").ToString();
                    value.mensagem.Message = "Plano Conta Pai inválido. Por favor, escolha outra conta";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
            }
            return value.mensagem;
        }

        public override PlanoContaViewModel CreateRepository(System.Web.HttpRequestBase Request = null)
        {
            using (ApplicationContext db = this.Create())
            {
                //int _exercicio = int.Parse(sessaoCorrente.value1);
                int _exercicio = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EXERCICIO_CONTABIL, sessaoCorrente.empresaId).valor);
                PlanoContaViewModel r = new PlanoContaViewModel()
                {
                    exercicio = _exercicio,
                    lastThree = db.PlanoContas.Where(p => p.empresaId.Equals(sessaoCorrente.empresaId) && p.exercicio == _exercicio).OrderByDescending(info => info.planoContaId).Take(6).OrderBy(info => info.codigoPleno).ToList(),
                    codigoReduzido = db.PlanoContas.Where(p => p.empresaId.Equals(sessaoCorrente.empresaId) && p.exercicio == _exercicio).Count() > 0 ?
                                                               db.PlanoContas.Where(m => m.empresaId.Equals(sessaoCorrente.empresaId) &&
                                                                                         m.exercicio == _exercicio).Select(code => code.codigoReduzido).Max() + 1 : 1,
                    mascaraPC = db.Exercicios.Where(m => m.empresaId.Equals(sessaoCorrente.empresaId) && m.exercicio.Equals(_exercicio)).Count() > 0 ?
                                                         db.Exercicios.Where(m => m.empresaId.Equals(sessaoCorrente.empresaId) &&
                                                                                  m.exercicio.Equals(_exercicio)).Select(code => code.mascaraPc).FirstOrDefault() : "9.99.999.999"
                };

                return r;
            }
        }
        #endregion
    }

    public class ListViewPlanoConta : ListViewRepository<PlanoContaViewModel, ApplicationContext>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<PlanoContaViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _descricao = param != null && param.Count() > 0 && param[0] != null ? param[0].ToString() : null;
            string sintetica = Enumeradores.TipoContaContabil.SINTETICA.GetStringDescription();
            string analitica = Enumeradores.TipoContaContabil.ANALITICA.GetStringDescription();
            //int _exercicio = int.Parse(sessaoCorrente.value1);

            int _exercicio = int.Parse(db.Parametros.Find((int)DWM.Models.Enumeracoes.Enumeradores.Param.EXERCICIO_CONTABIL, sessaoCorrente.empresaId).valor);

            return (from c in db.PlanoContas
                    where c.empresaId.Equals(sessaoCorrente.empresaId)
                           && c.exercicio == _exercicio
                           && (_descricao == null || String.IsNullOrEmpty(_descricao) || c.descricao.StartsWith(_descricao.Trim()) || c.codigoPleno.StartsWith(_descricao.Trim()))
                    orderby c.codigoPleno
                    select new PlanoContaViewModel
                    {
                        empresaId = c.empresaId,
                        planoContaId = c.planoContaId,
                        exercicio = c.exercicio,
                        codigoPleno = c.codigoPleno,
                        codigoReduzido = c.codigoReduzido,
                        planoContaId_pai = c.planoContaId_pai,
                        descricao = c.descricao,
                        tipoConta = c.tipoConta == "S" ? sintetica : analitica,
                        vr_saldo_inicial = c.vr_saldo_inicial,
                        //PageSize = pageSize,
                        //TotalCount = (from c1 in db.PlanoContas
                        //              where c1.empresaId.Equals(sessaoCorrente.empresaId)
                        //                    && c1.exercicio == _exercicio
                        //                    && (_descricao == null || String.IsNullOrEmpty(_descricao) || c1.descricao.StartsWith(_descricao.Trim()) || c1.codigoPleno.StartsWith(_descricao.Trim()))
                        //              select c1).Count()
                    }).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new PlanoContaModel().getObject((PlanoContaViewModel)id);
        }
        #endregion
    }

    public class LookupPlanoContaModel : ListViewPlanoConta
    {
        public override string action()
        {
            return "../PlanoContas/ListPlanoContaModal";
        }

        public override string DivId()
        {
            return "div-pc";
        }
    }

    public class LookupPlanoContaFiltroModel : ListViewPlanoConta
    {
        public override string action()
        {
            return "../PlanoContas/_ListPlanoContaModal";
        }

        public override string DivId()
        {
            return "div-pc";
        }
    }

    public class LookupPlanoContaPaiModel : ListViewPlanoConta
    {
        public override string action()
        {
            return "../PlanoContas/ListPlanoContaPaiModal";
        }

        public override string DivId()
        {
            return "div-pc";
        }
    }

    public class LookupPlanoContaPaiFiltroModel : ListViewPlanoConta
    {
        public override string action()
        {
            return "../PlanoContas/_ListPlanoContaPaiModal";
        }

        public override string DivId()
        {
            return "div-pc";
        }
    }

}