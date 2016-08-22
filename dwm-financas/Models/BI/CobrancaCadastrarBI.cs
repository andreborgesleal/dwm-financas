using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Security;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DWM.Models.BI
{
    public class CobrancaCadastrarBI : DWMContext<ApplicationContext>, IProcess<CobrancaViewModel, ApplicationContext>
    {
        #region Constructor
        public CobrancaCadastrarBI() { }

        public CobrancaCadastrarBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual CobrancaViewModel Run(Repository value)
        {
            CobrancaViewModel r = (CobrancaViewModel)value;
            try
            {
                #region Incluir os clientes na lista de cobrança
                ListViewModelCliente listViewCliente = new ListViewModelCliente();
                listViewCliente.Create(this.db, this.seguranca_db);

                IEnumerable<ClienteViewModel> listClientes = null;

                // se grupoClienteId == null (não inclui nenhum cliente na lista)
                if (r.grupoClienteId > 0) // clientes do grupo
                    listClientes = listViewCliente.Bind(0, 10000, db.GrupoClientes.Find(r.grupoClienteId).nome);
                else if (r.grupoClienteId == 0) // todos os clientes
                    listClientes = listViewCliente.Bind(0, 10000);

                r.CobrancaClientes = new List<CobrancaClienteViewModel>();

                foreach (ClienteViewModel cli in listClientes)
                {
                    CobrancaClienteViewModel cobCli = new CobrancaClienteViewModel()
                    {
                        clienteId = cli.clienteId,
                        empresaId = sessaoCorrente.empresaId,
                        dia_vencimento = 0,
                        mes_dia = 0,
                        valor = 0,
                    };
                    ((List<CobrancaClienteViewModel>)r.CobrancaClientes).Add(cobCli);
                }
                #endregion

                #region Cadastra a cobrança
                r.empresaId = sessaoCorrente.empresaId;
                CobrancaModel cobrancaModel = new CobrancaModel(this.db, this.seguranca_db);

                r = cobrancaModel.Insert(r);
                if (r.mensagem.Code > 0)
                    throw new Exception(r.mensagem.MessageBase);

                db.SaveChanges();
                seguranca_db.SaveChanges();
                #endregion
            }
            catch (App_DominioException ex)
            {
                r.mensagem = ex.Result;

                if (ex.InnerException != null)
                    r.mensagem.MessageBase = new App_DominioException(ex.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                else
                    r.mensagem.MessageBase = new App_DominioException(ex.Result.Message, GetType().FullName).Message;
            }
            catch (Exception ex)
            {
                r.mensagem = new Validate() { Code = 999, MessageBase = ex.Message, Message = "Erro de processamento da operação." };

                if (ex.InnerException != null)
                    r.mensagem.MessageBase = new App_DominioException(ex.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                else
                    r.mensagem.MessageBase = new App_DominioException(ex.Message, GetType().FullName).Message;
            }

            return r;
        }

        public IEnumerable<CobrancaViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }
}