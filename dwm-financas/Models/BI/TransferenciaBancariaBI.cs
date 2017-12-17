using System;
using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using System.Web.Mvc;
using App_Dominio.Security;

namespace DWM.Models.BI
{
    public class TransferenciaBancariaBI : DWMContext<ApplicationContext>, IProcess<TransferenciaBancariaViewModel, ApplicationContext>
    {

        #region Constructor
        public TransferenciaBancariaBI() { }

        public TransferenciaBancariaBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public TransferenciaBancariaViewModel Run(Repository value)
        {
            TransferenciaBancariaViewModel t = (TransferenciaBancariaViewModel)value;
            MovtoBancarioModel model = new MovtoBancarioModel(this.db, this.seguranca_db);

            try
            {
                #region Origem da transferência
                t.movtoBancarioOrigemViewModel.empresaId = sessaoCorrente.empresaId;
                t.movtoBancarioOrigemViewModel = model.Insert(t.movtoBancarioOrigemViewModel);
                if (t.movtoBancarioOrigemViewModel.mensagem.Code > 0)
                {
                    t.mensagem = t.movtoBancarioOrigemViewModel.mensagem;
                    return t;
                }
                #endregion

                #region destino da transferência
                t.movtoBancarioDestinoViewModel.empresaId = sessaoCorrente.empresaId;
                t.movtoBancarioDestinoViewModel = model.Insert(t.movtoBancarioDestinoViewModel);
                if (t.movtoBancarioDestinoViewModel.mensagem.Code > 0)
                {
                    t.mensagem = t.movtoBancarioDestinoViewModel.mensagem;
                    return t;
                }
                #endregion

                #region Contabilidade
                if (t.enquadramentoId.HasValue && t.enquadramentoId > 0)
                {
                    #region Mapear enquadramento para contabilidade
                    ContabilidadeModel contabilidadeModel = new ContabilidadeModel(this.db, this.seguranca_db);
                    ContabilidadeViewModel contabilidadeViewModel = contabilidadeModel.CreateRepositoryFromEnquadramento(new EnquadramentoViewModel() { enquadramentoId = t.enquadramentoId.Value });
                    contabilidadeViewModel.dt_lancamento = t.movtoBancarioOrigemViewModel.dt_movto;
                    int contador = 0;
                    while (contador <= contabilidadeViewModel.ContabilidadeItems.Count() - 1)
                    {
                        if (contabilidadeViewModel.ContabilidadeItems.ElementAt(contador).valor == 0)
                            contabilidadeViewModel.ContabilidadeItems.ElementAt(contador).valor = t.movtoBancarioOrigemViewModel.valor;

                        contador++;
                    }

                    contabilidadeViewModel.empresaId = sessaoCorrente.empresaId;
                    contabilidadeViewModel = contabilidadeModel.Insert(contabilidadeViewModel);

                    if (contabilidadeViewModel.mensagem.Code > 0)
                    {
                        t.mensagem = contabilidadeViewModel.mensagem;
                        return t;
                    }

                    #endregion
                };
                #endregion
                this.db.SaveChanges();
                t.mensagem = new Validate() { Code = 0, Message = "Transferência realizada com sucesso" };
            }
            catch (App_DominioException ex)
            {
                t.mensagem = ex.Result;

                if (ex.InnerException != null)
                    t.mensagem.MessageBase = new App_DominioException(ex.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                else
                    t.mensagem.MessageBase = new App_DominioException(ex.Result.Message, GetType().FullName).Message;
            }
            catch (Exception ex)
            {
                t.mensagem = new Validate() { Code = 999, MessageBase = ex.Message, Message = "Erro de processamento da operação." };

                if (ex.InnerException != null)
                    t.mensagem.MessageBase = new App_DominioException(ex.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                else
                    t.mensagem.MessageBase = new App_DominioException(ex.Message, GetType().FullName).Message;
            }

            return t;
        }

        public IEnumerable<TransferenciaBancariaViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }
}