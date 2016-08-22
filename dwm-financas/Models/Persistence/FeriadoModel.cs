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
using System.Web.Mvc;


namespace DWM.Models.Persistence
{
    public class FeriadoModel : CrudContext<Feriado, FeriadoViewModel, ApplicationContext>
    {
        #region Métodos da classe CrudContext
        public override Feriado MapToEntity(FeriadoViewModel value)
        {
            return new Feriado()
            {
                feriadoId = value.feriadoId,
                descricao = value.descricao,
                dt_feriado= value.dt_feriado,
                cidadeId = value.cidadeId
            };
        }

        public override FeriadoViewModel MapToRepository(Feriado entity)
        {
            return new FeriadoViewModel()
            {
                feriadoId = entity.feriadoId,
                descricao = entity.descricao,
                dt_feriado = entity.dt_feriado,
                cidadeId = entity.cidadeId,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Feriado Find(FeriadoViewModel key)
        {
            return db.Feriados.Find(key.feriadoId);
        }

        public override Validate Validate(FeriadoViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };

            if (value.descricao.Trim().Length == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Nome").ToString();
                value.mensagem.MessageBase = "Campo Descrição do Feriado deve ser informado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            else if (operation == Crud.INCLUIR)
            {
                // Verifica se o feriado já foi cadastrado com o mesmo descricao
                if (db.Feriados.Where(info => info.descricao == value.descricao).Count() > 0)
                {
                    value.mensagem.Code = 19;
                    value.mensagem.Message = MensagemPadrao.Message(19).ToString();
                    value.mensagem.MessageBase = "Campo Nome do Feriado já existe";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
            }
            else if (operation == Crud.ALTERAR)
                // Verifica se o feriado já foi cadastrado com o mesmo descricao
                if (db.Feriados.Where(info => info.descricao == value.descricao && info.feriadoId != value.feriadoId).Count() > 0)
                {
                    value.mensagem.Code = 19;
                    value.mensagem.Message = MensagemPadrao.Message(19).ToString();
                    value.mensagem.MessageBase = "Campo Nome do Feriado já existe";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

            return value.mensagem;
        }
        #endregion
    }

    public class ObterProximoDiaUtil : DWMContext<ApplicationContext>, IProcess<FeriadoViewModel, ApplicationContext>
    {
        public DateTime dt_referencia { get; set; }
        private int cidadeId { get; set; }

        #region Constructor
        public ObterProximoDiaUtil() { }

        public ObterProximoDiaUtil(ApplicationContext _db, SecurityContext _seguranca_db, DateTime? dt_referencia = null)
        {
            base.Create(_db, _seguranca_db);
            this.dt_referencia = dt_referencia ?? Funcoes.Brasilia().Date;
            this.cidadeId = db.EmpresaInternas.Find(sessaoCorrente.empresaId).cidadeId;
        }
        #endregion

        public FeriadoViewModel Run(Repository value)
        {
            FeriadoViewModel fer = (FeriadoViewModel)value;

            // verifica se é sábado ou domingo
            if (this.dt_referencia.DayOfWeek == DayOfWeek.Saturday)
                this.dt_referencia = this.dt_referencia.AddDays(2);
            else if (this.dt_referencia.DayOfWeek == DayOfWeek.Sunday)
                this.dt_referencia = this.dt_referencia.AddDays(1);

            if (db.Feriados.Where(info => info.cidadeId == cidadeId && info.dt_feriado == this.dt_referencia).Count() == 1)
            {
                this.dt_referencia = this.dt_referencia.AddDays(1);
                Run(fer);
            }

            return fer;
        }

        public IEnumerable<FeriadoViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }

    public class ListViewFeriado : ListViewRepository<FeriadoViewModel, ApplicationContext>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<FeriadoViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _descricao = param != null && param.Count() > 0 && param[0] != null ? param[0].ToString() : null;
            return (from cre in db.Feriados
                    where (_descricao == null || String.IsNullOrEmpty(_descricao) || cre.descricao.Contains(_descricao.Trim()))
                    orderby cre.descricao
                    select new FeriadoViewModel
                    {
                        feriadoId = cre.feriadoId,
                        descricao = cre.descricao,
                        cidadeId = cre.cidadeId,
                        dt_feriado = cre.dt_feriado,
                        PageSize = pageSize,
                        TotalCount = (from cre1 in db.Feriados
                                      where (_descricao == null || String.IsNullOrEmpty(_descricao) || cre1.descricao.Contains(_descricao.Trim()))
                                      select cre1.feriadoId).Count()
                    }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new FeriadoModel().getObject((FeriadoViewModel)id);
        }
        #endregion
    }
}