using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using System.Web;
using System;
using App_Dominio.Component;
using App_Dominio.Models;

namespace DWM.Models.Persistence
{
    public class MovtoBancarioModel : CrudModel<MovtoBancario, MovtoBancarioViewModel, ApplicationContext>
    {
        #region Constructor
        public MovtoBancarioModel() { }
        public MovtoBancarioModel(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe CrudModel
        public override MovtoBancario MapToEntity(MovtoBancarioViewModel value)
        {
            MovtoBancario m = Find(value);
            if (m != null)
            {
                m.movtoBancarioId = value.movtoBancarioId;
                m.historicoId = value.historicoId;
                m.bancoId = value.bancoId;
                m.empresaId = value.empresaId;
                m.complementoHist = value.complementoHist;
                m.dt_movto = value.dt_movto;
                m.valor = value.valor;
                m.documento = value.documento;
                m.tipoMovto = value.tipoMovto;
            }
            else
                m = new MovtoBancario()
                {
                    movtoBancarioId = value.movtoBancarioId,
                    historicoId = value.historicoId,
                    bancoId = value.bancoId,
                    empresaId = value.empresaId,
                    complementoHist = value.complementoHist,
                    dt_movto = value.dt_movto,
                    valor = value.valor,
                    documento = value.documento,
                    tipoMovto = value.tipoMovto
                };

            return m; 
        }

        public override MovtoBancarioViewModel MapToRepository(MovtoBancario entity)
        {
            return new MovtoBancarioViewModel()
            {
                movtoBancarioId = entity.movtoBancarioId,
                historicoId = entity.historicoId,
                descricao_historico = db.Historicos.Find(entity.historicoId).descricao,
                empresaId = entity.empresaId,
                bancoId = entity.bancoId,
                nome_banco = db.Bancos.Find(entity.bancoId).nome,
                complementoHist = entity.complementoHist,
                dt_movto = entity.dt_movto,
                valor = entity.valor,
                documento = entity.documento,
                tipoMovto = entity.tipoMovto,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override MovtoBancario Find(MovtoBancarioViewModel key)
        {
            return db.MovtoBancarios.Find(key.movtoBancarioId);
        }

        public override Validate Validate(MovtoBancarioViewModel value, Crud operation)
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

            if (value.bancoId == null || value.bancoId == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Banco").ToString();
                value.mensagem.MessageBase = "Banco deve ser preenchida";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.historicoId == null || value.historicoId == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Histórico").ToString();
                value.mensagem.MessageBase = "Histórico deve ser preenchida";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.dt_movto == null || value.dt_movto <= Convert.ToDateTime("1980-01-01"))
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Dt.Movimento").ToString();
                value.mensagem.MessageBase = "Dt. Movimento deve ser preenchida";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.valor <= 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Valor").ToString();
                value.mensagem.MessageBase = "Valor do movimento deve ser preenchido com um número maior que zero";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            return value.mensagem;
        }

        #endregion
    }

    public class ListViewMovtoBancario : ListViewModel<MovtoBancarioViewModel, ApplicationContext>
    {
        #region Constructor
        public ListViewMovtoBancario() { }
        public ListViewMovtoBancario(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion
        
        #region Métodos da classe ListViewRepository
        public override IEnumerable<MovtoBancarioViewModel> Bind(int? index, int pageSize = 50, params object[] param)
        {
            int? _bancoId = (int?)param[0];
            DateTime _data1 = (DateTime)param[1];
            DateTime _data2 = (DateTime)param[2];

            return (from m in db.MovtoBancarios join h in db.Historicos on m.historicoId equals h.historicoId join b in db.Bancos on m.bancoId equals b.bancoId
                    join p in db.ContaPagarParcelaEventos on m.movtoBancarioId equals p.movtoBancarioId into P
                    from p in P.DefaultIfEmpty()
                    join r in db.ContaReceberParcelaEventos on m.movtoBancarioId equals r.movtoBancarioId into R
                    from r in R.DefaultIfEmpty()
                    where m.empresaId.Equals(sessaoCorrente.empresaId) &&
                          (!_bancoId.HasValue || m.bancoId == _bancoId) &&
                          m.dt_movto >= _data1 && m.dt_movto <= _data2
                    orderby m.dt_movto
                    select new MovtoBancarioViewModel
                    {
                        movtoBancarioId = m.movtoBancarioId,
                        empresaId = sessaoCorrente.empresaId,
                        historicoId = m.historicoId,
                        descricao_historico = h.descricao,
                        bancoId = m.bancoId,
                        nome_banco = b.nome,
                        complementoHist = m.complementoHist,
                        dt_movto = m.dt_movto,
                        valor = m.valor,
                        documento = m.documento,
                        tipoMovto = m.tipoMovto,
                        HasOperacao = p.movtoBancarioId != null || r.movtoBancarioId != null ? "S" : "N",
                        PageSize = pageSize,
                        TotalCount = (from m1 in db.MovtoBancarios
                                      join h1 in db.Historicos on m1.historicoId equals h1.historicoId
                                      join b1 in db.Bancos on m1.bancoId equals b1.bancoId
                                      join p1 in db.ContaPagarParcelaEventos on m1.movtoBancarioId equals p1.movtoBancarioId into P1
                                      from p1 in P1.DefaultIfEmpty()
                                      join r1 in db.ContaReceberParcelaEventos on m1.movtoBancarioId equals r1.movtoBancarioId into R1
                                      from r1 in R1.DefaultIfEmpty()
                                      where m1.empresaId.Equals(sessaoCorrente.empresaId) &&
                                            (!_bancoId.HasValue || m1.bancoId == _bancoId) &&
                                            m1.dt_movto >= _data1 && m1.dt_movto <= _data2
                                      orderby m1.dt_movto
                                      select m1).Count()
                    }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new MovtoBancarioModel(this.db, this.seguranca_db).getObject((MovtoBancarioViewModel)id);
        }
        #endregion
    }
}