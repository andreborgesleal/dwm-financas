using System;
using System.Collections.Generic;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using DWM.Models.Entidades;
using System.Web.Mvc;
using App_Dominio.Enumeracoes;
using App_Dominio.Security;
using System.Linq;
using System.Data.Entity.Infrastructure;
using App_Dominio.Models;
using App_Dominio.Repositories;
using System.Data.Entity;

namespace DWM.Models.BI
{
    public class CodigoAtivacaoBI : DWMContextLocal, IProcess<UsuarioRepository, ApplicationContext>
    {
        #region Constructor
        public CodigoAtivacaoBI() { }

        public void Create(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            this.db = _db;
            this.seguranca_db = _seguranca_db;
        }

        public CodigoAtivacaoBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            Create(_db, _seguranca_db);
        }
        #endregion

        public UsuarioRepository Run(Repository value)
        {
            UsuarioRepository r = (UsuarioRepository)value;
            try
            {
                // verifica se a chave ínformada é igual a chave gravada no banco para o respectivo usuário
                Usuario u = seguranca_db.Usuarios.Find(r.usuarioId);
                if (u == null)
                    throw new ArgumentException("Usuário inválido");

                if (u.dt_keyword < Funcoes.Brasilia())
                    throw new ArgumentException("Chave de ativação expirada");

                if (u.keyword != r.keyword)
                    throw new ArgumentException("Chave de ativação inválida");

                if (r.senha == null || r.senha.Trim() == "")
                    throw new ArgumentException("Senha deve ser informada");

                if (r.senha != r.confirmacaoSenha)
                    throw new ArgumentException("Senha e confirmação de senha não conferem");

                EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();

                // Ativar o usuário
                u.keyword = null;
                u.dt_keyword = null;
                u.situacao = "A";
                u.senha = security.Criptografar(r.senha);

                seguranca_db.Entry(u).State = EntityState.Modified;

                seguranca_db.SaveChanges();

                r.mensagem = new Validate() { Code = -1 };
            }
            catch (DbUpdateException ex)
            {
                r.mensagem.MessageBase = ex.InnerException.InnerException.Message ?? ex.Message;
                if (r.mensagem.MessageBase.ToUpper().Contains("REFERENCE"))
                {
                    if (r.mensagem.MessageBase.ToUpper().Contains("DELETE"))
                    {
                        r.mensagem.Code = 16;
                        r.mensagem.Message = MensagemPadrao.Message(16).ToString();
                        r.mensagem.MessageType = MsgType.ERROR;
                    }
                    else
                    {
                        r.mensagem.Code = 28;
                        r.mensagem.Message = MensagemPadrao.Message(28).ToString();
                        r.mensagem.MessageType = MsgType.ERROR;
                    }
                }
                else if (r.mensagem.MessageBase.ToUpper().Contains("PRIMARY"))
                {
                    r.mensagem.Code = 37;
                    r.mensagem.Message = MensagemPadrao.Message(37).ToString();
                    r.mensagem.MessageType = MsgType.WARNING;
                }
                else if (r.mensagem.MessageBase.ToUpper().Contains("UNIQUE KEY"))
                {
                    r.mensagem.Code = 54;
                    r.mensagem.Message = MensagemPadrao.Message(54).ToString();
                    r.mensagem.MessageType = MsgType.WARNING;
                }
                else
                {
                    r.mensagem.Code = 44;
                    r.mensagem.Message = MensagemPadrao.Message(44).ToString();
                    r.mensagem.MessageType = MsgType.ERROR;
                }
            }
            catch (ArgumentException ex)
            {
                App_DominioException.saveError(ex, "ERROR");
                r.mensagem = new Validate() { Code = 997, Message = MensagemPadrao.Message(997).ToString(), MessageBase = ex.Message };
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                r.mensagem = new Validate() { Code = 42, Message = MensagemPadrao.Message(42).ToString(), MessageBase = ex.EntityValidationErrors.Select(m => m.ValidationErrors.First().ErrorMessage).First() };
            }
            catch (Exception ex)
            {
                r.mensagem.Code = 17;
                r.mensagem.Message = MensagemPadrao.Message(17).ToString();
                r.mensagem.MessageBase = new App_DominioException(ex.InnerException.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                r.mensagem.MessageType = MsgType.ERROR;
            }
            return r;
        }

        public IEnumerable<UsuarioRepository> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }

    public class CodigoValidacaoBI : DWMContextLocal, IProcess<UsuarioRepository, ApplicationContext>
    {
        #region Constructor
        public CodigoValidacaoBI() { }

        public void Create(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            this.db = _db;
            this.seguranca_db = _seguranca_db;
        }

        public CodigoValidacaoBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            Create(_db, _seguranca_db);
        }
        #endregion

        public UsuarioRepository Run(Repository value)
        {
            UsuarioRepository r = (UsuarioRepository)value;
            try
            {
                // verifica se a chave ínformada é igual a chave gravada no banco para o respectivo usuário
                Usuario u = seguranca_db.Usuarios.Find(r.usuarioId);
                if (u == null)
                    throw new ArgumentException("Usuário inválido");

                if (u.dt_keyword < Funcoes.Brasilia())
                    throw new ArgumentException("Chave de ativação expirada");

                if (u.keyword != r.keyword)
                    throw new ArgumentException("Chave de ativação inválida");

                r.nome = u.nome;
                r.login = u.login;
                r.isAdmin = "N";
                r.situacao = "D";

                r.mensagem = new Validate() { Code = -1 };
            }
            catch (DbUpdateException ex)
            {
                r.mensagem.MessageBase = ex.InnerException.InnerException.Message ?? ex.Message;
                if (r.mensagem.MessageBase.ToUpper().Contains("REFERENCE"))
                {
                    if (r.mensagem.MessageBase.ToUpper().Contains("DELETE"))
                    {
                        r.mensagem.Code = 16;
                        r.mensagem.Message = MensagemPadrao.Message(16).ToString();
                        r.mensagem.MessageType = MsgType.ERROR;
                    }
                    else
                    {
                        r.mensagem.Code = 28;
                        r.mensagem.Message = MensagemPadrao.Message(28).ToString();
                        r.mensagem.MessageType = MsgType.ERROR;
                    }
                }
                else if (r.mensagem.MessageBase.ToUpper().Contains("PRIMARY"))
                {
                    r.mensagem.Code = 37;
                    r.mensagem.Message = MensagemPadrao.Message(37).ToString();
                    r.mensagem.MessageType = MsgType.WARNING;
                }
                else if (r.mensagem.MessageBase.ToUpper().Contains("UNIQUE KEY"))
                {
                    r.mensagem.Code = 54;
                    r.mensagem.Message = MensagemPadrao.Message(54).ToString();
                    r.mensagem.MessageType = MsgType.WARNING;
                }
                else
                {
                    r.mensagem.Code = 44;
                    r.mensagem.Message = MensagemPadrao.Message(44).ToString();
                    r.mensagem.MessageType = MsgType.ERROR;
                }
            }
            catch (ArgumentException ex)
            {
                r.mensagem = new Validate() { Code = 20, Message = MensagemPadrao.Message(20).ToString(), MessageBase = ex.Message };
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                r.mensagem = new Validate() { Code = 42, Message = MensagemPadrao.Message(42).ToString(), MessageBase = ex.EntityValidationErrors.Select(m => m.ValidationErrors.First().ErrorMessage).First() };
            }
            catch (Exception ex)
            {
                r.mensagem.Code = 17;
                r.mensagem.Message = MensagemPadrao.Message(17).ToString();
                r.mensagem.MessageBase = new App_DominioException(ex.InnerException.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                r.mensagem.MessageType = MsgType.ERROR;
            }
            return r;
        }

        public IEnumerable<UsuarioRepository> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }

}