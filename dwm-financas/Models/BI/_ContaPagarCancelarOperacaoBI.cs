using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Models;
using App_Dominio.Security;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace DWM.Models.BI
{
    public class _ContaPagarCancelarOperacaoBI : DWMContext<ApplicationContext>, IProcess<ContaPagarViewModel, ApplicationContext>
    {
        #region Constructor
        public _ContaPagarCancelarOperacaoBI() { }

        public _ContaPagarCancelarOperacaoBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual ContaPagarViewModel Run(Repository value)
        {
            ContaPagarViewModel r = (ContaPagarViewModel)value;
            ContaPagarViewModel repository = new ContaPagarViewModel()
            {
                operacaoId = r.operacaoId,
                mensagem = new Validate() { Code = 0, Message = "Registro cancelado com sucesso" }
            };

            try
            {
                ContaPagarModel model = new ContaPagarModel(this.db, this.seguranca_db);
                ContaPagarParcelaEventoModel contaPagarParcelaEventoModel = new ContaPagarParcelaEventoModel(this.db, this.seguranca_db);

                repository = model.getObject(repository);

                int seconds = 0;
                int i = 0;
                Evento eve = db.Eventos.Where(info => info.codigo == 5 && info.empresaId == sessaoCorrente.empresaId).FirstOrDefault();

                foreach (ContaPagarParcelaViewModel par in repository.ContaPagarParcelas)
                {
                    if (!"4|5".Contains(par.ind_baixa ?? " "))
                    {
                        ContaPagarParcelaEventoViewModel contaPagarParcelaEvento = new ContaPagarParcelaEventoViewModel()
                        {
                            operacaoId = repository.operacaoId,
                            parcelaId = par.parcelaId,
                            dt_evento = Funcoes.Brasilia().AddSeconds(seconds),
                            eventoId = eve.eventoId,
                            dt_ocorrencia = Funcoes.Brasilia().Date,
                            valor = par.vr_saldo_devedor.Value,
                            ind_operacao = eve.ind_operacao,
                            ind_estorno = "N",
                            ind_tipoEvento = eve.ind_tipoEvento,
                            uri = r.uri,
                        };

                        contaPagarParcelaEvento = contaPagarParcelaEventoModel.Insert(contaPagarParcelaEvento);
                        if (contaPagarParcelaEvento.mensagem.Code > 0)
                            throw new Exception(contaPagarParcelaEvento.mensagem.MessageBase);

                        seconds += 2;
                    }

                    i++;
                }

                if (seconds == 0)
                    repository.mensagem = new Validate() { Code = 20, Message = "Operação já havia sido cancelada" };
                else
                    repository.mensagem = new Validate() { Code = 0, Message = "Operação cancelada com sucesso !!!" };
            }
            catch (App_DominioException ex)
            {
                repository.mensagem = ex.Result;

                if (ex.InnerException != null)
                    repository.mensagem.MessageBase = new App_DominioException(ex.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                else
                    repository.mensagem.MessageBase = new App_DominioException(ex.Result.Message, GetType().FullName).Message;
            }
            catch (Exception ex)
            {
                repository.mensagem = new Validate() { Code = 999, MessageBase = ex.Message, Message = "Erro de processamento da operação." };

                if (ex.InnerException != null)
                    repository.mensagem.MessageBase = new App_DominioException(ex.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                else
                    repository.mensagem.MessageBase = new App_DominioException(ex.Message, GetType().FullName).Message;
            }

            return repository;
        }

        public IEnumerable<ContaPagarViewModel> List(params object[] param)
        {
            ContaPagarViewModel repository = new ContaPagarViewModel()
            {
                operacaoId = (int)param[0],
                mensagem = new Validate() { Code = 0, Message = "Registro cancelado com sucesso" }
            };

            ContaPagarModel model = new ContaPagarModel(this.db, this.seguranca_db);
            repository = model.getObject(repository);

            int i = 0;

            while (i <= repository.ContaPagarParcelas.Count() -1)
                repository.ContaPagarParcelas.ElementAt(i++).vr_saldo_devedor = 0;

            repository.ContaPagarParcela.vr_saldo_devedor = 0;

            IList<ContaPagarViewModel> list = new List<ContaPagarViewModel>();
            list.Add(repository);

            return list.ToList();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }
}