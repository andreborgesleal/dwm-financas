using System;
using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using System.Web;
using App_Dominio.Models;
using App_Dominio.Security;

namespace DWM.Models.Persistence
{
    public class ExercicioModel : CrudContext<Exercicio, ExercicioViewModel, ApplicationContext>
    {
        #region Métodos da classe CrudContext
        public override Exercicio MapToEntity(ExercicioViewModel value)
        {
            Exercicio entity = Find(value);

            if (entity == null)
                entity = new Exercicio();

            entity.empresaId = value.empresaId;
            entity.exercicio = value.exercicio;
            entity.dt_inicio = value.dt_inicio;
            entity.dt_fim = value.dt_fim;
            entity.dt_lancamento_inicio = value.dt_lancamento_inicio;
            entity.dt_lancamento_fim = value.dt_lancamento_fim;
            entity.mascaraPc = value.mascaraPc;
            entity.encerrado = value.encerrado;

            return entity;
        }

        public override ExercicioViewModel MapToRepository(Exercicio entity)
        {
            return new ExercicioViewModel()
            {
                empresaId = entity.empresaId,
                exercicio = entity.exercicio,
                dt_inicio = entity.dt_inicio,
                dt_fim = entity.dt_fim,
                dt_lancamento_inicio = entity.dt_lancamento_inicio,
                dt_lancamento_fim = entity.dt_lancamento_fim,
                mascaraPc = entity.mascaraPc,
                encerrado = entity.encerrado,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Exercicio Find(ExercicioViewModel key)
        {
            return db.Exercicios.Find(key.empresaId, key.exercicio);
        }

        public override Validate Validate(ExercicioViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };

            if (value.empresaId == 0)
            {
                value.mensagem.Code = 35;
                value.mensagem.Message = MensagemPadrao.Message(35).ToString();
                value.mensagem.MessageBase = "Sua sessão expirou. Faça um novo login no sistema";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.exercicio < 2016 || value.exercicio > 2050)
            {
                value.mensagem.Code = 4;
                value.mensagem.Message = MensagemPadrao.Message(4, "Exercício contábil", value.exercicio.ToString()).ToString();
                value.mensagem.MessageBase = "Exerício contábil informado está fora da faixa de valores permitido (2016 à 2050)";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            #region Verificar se já existe o exercício cadastrado na inclusão
            if (operation == Crud.INCLUIR &&
                (from ex in db.Exercicios
                 where ex.empresaId == value.empresaId
                        && ex.exercicio == value.exercicio
                 select ex).Count() > 0)
            {
                value.mensagem.Code = 19;
                value.mensagem.Message = MensagemPadrao.Message(19).ToString();
                value.mensagem.MessageBase = "Exercício já cadastrado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            #endregion

            if (operation != Crud.EXCLUIR)
            {
                if (value.dt_inicio == null || value.dt_inicio <= Convert.ToDateTime("1980-01-01"))
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Data de início do exercício").ToString();
                    value.mensagem.MessageBase = "Data de início do exercício deve ser preenchida com uma data válida";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.dt_inicio == null || value.dt_inicio <= Convert.ToDateTime("1980-01-01"))
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Data de início do exercício").ToString();
                    value.mensagem.MessageBase = "Data de início do exercício deve ser preenchida com uma data válida";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.dt_fim == null)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Data fim do exercício").ToString();
                    value.mensagem.MessageBase = "Data fim do exercício deve ser preenchida com uma data válida e deve ser maior que a data de início";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.dt_fim <= value.dt_inicio)
                {
                    value.mensagem.Code = 11;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Data fim do exercício", "Data início do exercício").ToString();
                    value.mensagem.MessageBase = "Data fim do exercício deve ser preenchida com uma data válida e deve ser maior que a data de início";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if ((value.dt_lancamento_inicio != null && value.dt_lancamento_fim == null) || 
                    (value.dt_lancamento_inicio == null && value.dt_lancamento_fim != null))
                {
                    value.mensagem.Code = 55;
                    value.mensagem.Message = MensagemPadrao.Message(55).ToString();
                    value.mensagem.MessageBase = "Se a data de lançamento inicial for informada, a data de lançamento final também deverá ser preenchida e vice versa.";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.dt_lancamento_fim <= value.dt_lancamento_inicio)
                {
                    value.mensagem.Code = 11;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Data fim do lançamento", "Data início do lançamento").ToString();
                    value.mensagem.MessageBase = "Data fim do lançamento deve ser preenchida com uma data válida e deve ser maior que a data de início do lançamento";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }


                if ((value.dt_lancamento_inicio != null && (value.dt_lancamento_inicio < value.dt_inicio || value.dt_lancamento_inicio > value.dt_fim)) ||
                    (value.dt_lancamento_fim != null && (value.dt_lancamento_fim < value.dt_inicio || value.dt_lancamento_fim > value.dt_fim)))
                {
                    value.mensagem.Code = 48;
                    value.mensagem.Message = MensagemPadrao.Message(48, "Data lançamento inicial e Data lancamento Final",value.dt_inicio.ToString("dd/MM/yyyy"), value.dt_fim.ToString("dd/MM/yyyy")).ToString();
                    value.mensagem.MessageBase = "Data de lançamento inicial ou Data de lançamento final inválidas";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (String.IsNullOrEmpty(value.mascaraPc))
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Máscara do plano de contas").ToString();
                    value.mensagem.MessageBase = "Máscara do plano de contas deve ser informada";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
            }
            else if ((from con in db.Contabilidades where con.empresaId == value.empresaId && con.exercicio == value.exercicio select con).Count() > 0)
            {
                value.mensagem.Code = 16;
                value.mensagem.Message = MensagemPadrao.Message(16).ToString();
                value.mensagem.MessageBase = "Existem lançamentos contábeis vinculados a este exercício.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            return value.mensagem;
        }

        public override ExercicioViewModel CreateRepository(HttpRequestBase Request = null)
        {
            EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();
            ExercicioViewModel value = base.CreateRepository(Request);
            value.empresaId = security.getEmpresa().empresaId;
            value.exercicio = Funcoes.Brasilia().Date.Year;
            value.dt_inicio = new DateTime(Funcoes.Brasilia().Date.Year, 1, 1);
            value.dt_fim = new DateTime(Funcoes.Brasilia().Date.Year, 12, 31);
            value.dt_lancamento_inicio = new DateTime(Funcoes.Brasilia().Date.Year, 1, 1);
            value.dt_lancamento_fim = new DateTime(Funcoes.Brasilia().Date.Year, 1, 31);
            value.encerrado = "N";
            return value;
        }

        #endregion
    }

    public class ListViewExercicio : ListViewRepository<ExercicioViewModel, ApplicationContext>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<ExercicioViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            //string _descricao = param != null && param.Count() > 0 && param[0] != null ? param[0].ToString() : null;
            int? _exercicio = null;
            if (param != null && param.Count() > 0 && param [0] != null)
                _exercicio = int.Parse(param[0].ToString());

            return (from ex in db.Exercicios
                    where ex.empresaId.Equals(sessaoCorrente.empresaId) &&
                          (_exercicio == null || ex.exercicio == _exercicio)
                    orderby ex.exercicio descending
                    select new ExercicioViewModel()
                    {
                        empresaId = sessaoCorrente.empresaId,
                        exercicio = ex.exercicio,
                        dt_inicio = ex.dt_inicio,
                        dt_fim = ex.dt_fim,
                        dt_lancamento_inicio = ex.dt_lancamento_inicio,
                        dt_lancamento_fim = ex.dt_lancamento_fim,
                        mascaraPc = ex.mascaraPc,
                        encerrado = ex.encerrado,
                        PageSize = pageSize,
                        TotalCount = (from ex1 in db.Exercicios
                                      where ex1.empresaId.Equals(sessaoCorrente.empresaId) &&
                                            (_exercicio == null || ex1.exercicio == _exercicio)
                                      orderby ex1.exercicio descending
                                      select ex1).Count()
                    }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new ExercicioModel().getObject((ExercicioViewModel)id);
        }
        #endregion
    }

}