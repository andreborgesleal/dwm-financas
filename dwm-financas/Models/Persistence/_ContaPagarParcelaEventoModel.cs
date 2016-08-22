using System;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Models;
using System.IO;
using App_Dominio.Security;


namespace DWM.Models.Persistence
{
    public class _ContaPagarParcelaEventoModel : CrudModel<ContaPagarParcelaEvento, ContaPagarParcelaEventoViewModel, ApplicationContext>
    {
        #region Constructor
        public _ContaPagarParcelaEventoModel() { }
        public _ContaPagarParcelaEventoModel(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe CrudModel
        public override ContaPagarParcelaEventoViewModel BeforeInsert(ContaPagarParcelaEventoViewModel value)
        {
            #region Imports ContabilidadeViewModel From Enquadramento
            if (value.enquadramentoId != null && value.enquadramentoId > 0)
            {
                ContabilidadeModel contabilidadeModel = new ContabilidadeModel(this.db, this.seguranca_db);
                EnquadramentoViewModel enq = new EnquadramentoViewModel() { enquadramentoId = value.enquadramentoId.Value };
                value.Contabilidade = contabilidadeModel.CreateRepositoryFromEnquadramento(enq);

                value.Contabilidade.dt_lancamento = value.dt_ocorrencia;
                int contador = 0;
                while (contador <= value.Contabilidade.ContabilidadeItems.Count() - 1)
                    value.Contabilidade.ContabilidadeItems.ElementAt(contador++).valor = value.valor;
            };
            #endregion

            return value;
        }

        public override ContaPagarParcelaEventoViewModel AfterInsert(ContaPagarParcelaEventoViewModel value)
        {
            try
            {
                // FOI SUBSTITUÍDO POR UMA TRIGGER
                //**********************************
                //#region UC-043 Atualizar saldo da parcela de contas a pagar
                //AtualizarSaldoContaPagar atu = new AtualizarSaldoContaPagar(this.db, this.seguranca_db);
                //ContaPagarParcelaCrudModel par = new ContaPagarParcelaCrudModel(this.db, this.seguranca_db);
                //ContaPagarParcelaViewModel r = par.getObject(new ContaPagarParcelaViewModel() { operacaoId = value.operacaoId, parcelaId = value.parcelaId });
                //r = atu.Run(r);
                //if (r != null && r.mensagem.Code > 0)
                //    {
                //        value.mensagem = r.mensagem;
                //        return value;
                //    }                                        
                //#endregion

                #region Check if has file to transfer from Temp Folder to Users_Data Folder 
                if (value.arquivo != null && value.arquivo != "")
                {
                    #region Move the file from Temp Folder to Users_Data Folder
                    System.IO.FileInfo f = new System.IO.FileInfo(Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Temp"), value.arquivo));
                    f.MoveTo(Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Users_Data"), value.arquivo));
                    #endregion
                }
                #endregion
            }
            catch (DirectoryNotFoundException ex)
            {
                value.mensagem.Code = 17;
                value.mensagem.Message = MensagemPadrao.Message(17).ToString();
                value.mensagem.MessageBase = new App_DominioException(ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message, GetType().FullName).Message + ". Path de armazenamento do arquivo de boleto/comprovante não encontrado";
                value.mensagem.MessageType = MsgType.ERROR;
            }
            catch (FileNotFoundException ex)
            {
                value.mensagem.Code = 17;
                value.mensagem.Message = MensagemPadrao.Message(17).ToString();
                value.mensagem.MessageBase = new App_DominioException(ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message, GetType().FullName).Message + ". Arquivo de boleto/comprovante não encontrado";
                value.mensagem.MessageType = MsgType.ERROR;
            }
            catch (IOException ex)
            {
                value.mensagem.Code = 17;
                value.mensagem.Message = MensagemPadrao.Message(17).ToString();
                value.mensagem.MessageBase = new App_DominioException(ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message, GetType().FullName).Message + ". Erro referente ao arquivo de boleto/comprovante";
                value.mensagem.MessageType = MsgType.ERROR;
            }
            catch (Exception ex)
            {
                value.mensagem.Code = 17;
                value.mensagem.Message = MensagemPadrao.Message(17).ToString();
                value.mensagem.MessageBase = new App_DominioException(ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message, GetType().FullName).Message;
                value.mensagem.MessageType = MsgType.ERROR;
            }
            return value;
        }

        public override ContaPagarParcelaEvento MapToEntity(ContaPagarParcelaEventoViewModel value)
        {
            ContaPagarParcelaEvento contaPagarParcelaEvento = null;
            if (value.uri != null && value.uri != "")
            {
                contaPagarParcelaEvento = db.ContaPagarParcelaEventos.Find(value.operacaoId, value.parcelaId, value.dt_evento);

                if (contaPagarParcelaEvento == null)
                {
                    contaPagarParcelaEvento = new ContaPagarParcelaEvento();
                }
                else
                {
                    contaPagarParcelaEvento.movtoBancarioId = null;
                    contaPagarParcelaEvento.contabilidadeId = null;
                    contaPagarParcelaEvento.Contabilidade = null;
                    contaPagarParcelaEvento.MovtoBancario = null;
                }
            }
            else
                contaPagarParcelaEvento = new ContaPagarParcelaEvento();

            contaPagarParcelaEvento.operacaoId = value.operacaoId;
            contaPagarParcelaEvento.parcelaId = value.parcelaId;
            contaPagarParcelaEvento.dt_evento = value.dt_evento;
            contaPagarParcelaEvento.eventoId = value.eventoId;
            contaPagarParcelaEvento.dt_ocorrencia = value.dt_ocorrencia;
            contaPagarParcelaEvento.valor = value.valor;
            contaPagarParcelaEvento.ind_operacao = value.ind_operacao;
            contaPagarParcelaEvento.ind_estorno = value.ind_estorno;
            contaPagarParcelaEvento.ind_tipoEvento = value.ind_tipoEvento;
            contaPagarParcelaEvento.arquivo = value.arquivo;

            #region Mapping Movimentação bancária to Entity
            if (value.MovtoBancario != null) // Amortização ou Baixa por motivo de liquidação (gera MOVIMENTO BANCÁRIO)
            {
                MovtoBancarioModel movtoBancarioModel = new MovtoBancarioModel();
                contaPagarParcelaEvento.MovtoBancario = movtoBancarioModel.MapToEntity(value.MovtoBancario);
            }
            #endregion

            #region Mapping contabilidade to Entity
            if (value.Contabilidade != null)
            {
                ContabilidadeModel contabilidadeModel = new ContabilidadeModel(this.db, this.seguranca_db);
                contaPagarParcelaEvento.Contabilidade = contabilidadeModel.MapToEntity(value.Contabilidade);
            };
            #endregion

            return contaPagarParcelaEvento;
        }

        public override ContaPagarParcelaEventoViewModel MapToRepository(ContaPagarParcelaEvento entity)
        {
            ContaPagarParcelaEventoViewModel r = new ContaPagarParcelaEventoViewModel()
            {
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS },
                operacaoId = entity.operacaoId,
                parcelaId = entity.parcelaId,
                dt_evento = entity.dt_evento,
                eventoId = entity.eventoId,
                codigo = db.Eventos.Where(info => info.eventoId == entity.eventoId).Select(info => info.codigo).FirstOrDefault(),
                descricao_evento = db.Eventos.Where(info => info.eventoId == entity.eventoId).Select(info => info.descricao).FirstOrDefault(),
                dt_ocorrencia = entity.dt_ocorrencia,
                valor = entity.valor,
                arquivo = entity.arquivo,
                ind_operacao = entity.ind_operacao,
                ind_estorno = entity.ind_estorno,
                ind_tipoEvento = entity.ind_tipoEvento
            };

            if (entity.MovtoBancario != null)
            {
                MovtoBancarioModel movtoBancarioModel = new MovtoBancarioModel(this.db, this.seguranca_db);
                r.MovtoBancario = movtoBancarioModel.MapToRepository(entity.MovtoBancario);
                r.movtoBancarioId = r.MovtoBancario.movtoBancarioId;
            }

            if (entity.Contabilidade != null)
            {
                ContabilidadeModel contabilidadeModel = new ContabilidadeModel(this.db, this.seguranca_db);    
                r.Contabilidade = contabilidadeModel.MapToRepository(entity.Contabilidade);
                r.contabilidadeId = entity.Contabilidade.contabilidadeId;
            }

            return r;
        }

        public override ContaPagarParcelaEvento Find(ContaPagarParcelaEventoViewModel key)
        {
            return db.ContaPagarParcelaEventos.Find(key.operacaoId, key.parcelaId, key.dt_evento);
        }

        public override Validate Validate(ContaPagarParcelaEventoViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };

            if (value.parcelaId == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.MessageBase = MensagemPadrao.Message(5, "Parcela ID").ToString();
                value.mensagem.Message = "Campo obrigatório: Parcela ID";
                value.mensagem.MessageType = MsgType.WARNING;

                return value.mensagem;
            }

            if (value.dt_evento <= Convert.ToDateTime("1980-01-01"))
            {
                value.mensagem.Code = 5;
                value.mensagem.MessageBase = MensagemPadrao.Message(5, "Data do evento").ToString();
                value.mensagem.Message = "Campo obrigatório: Data do evento";
                value.mensagem.MessageType = MsgType.WARNING;

                return value.mensagem;
            }

            if (value.dt_ocorrencia <= Convert.ToDateTime("1980-01-01"))
            {
                value.mensagem.Code = 5;
                value.mensagem.MessageBase = MensagemPadrao.Message(5, "Data da Ocorrência").ToString();
                value.mensagem.Message = "Campo obrigatório: Data da Ocorrência";
                value.mensagem.MessageType = MsgType.WARNING;

                return value.mensagem;
            }            

            if (value.eventoId == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.MessageBase = MensagemPadrao.Message(5, "Evento ID").ToString();
                value.mensagem.Message = "Campo obrigatório: Evento ID";
                value.mensagem.MessageType = MsgType.WARNING;

                return value.mensagem;
            }

            // é permitido o valor igual a zero para poder liquidar a parcela 
            // situação em que foi amortizado todo o valor principal e está sendo pago apenas a multa/emcargos
            if (value.valor < 0) 
            {
                value.mensagem.Code = 5;
                value.mensagem.MessageBase = MensagemPadrao.Message(5, "Valor do evento " + value.ind_tipoEvento).ToString();
                value.mensagem.Message = "O valor do envento não pode ser nulo.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (!"DC".Contains(value.ind_operacao))
            {
                value.mensagem.Code = 5;
                value.mensagem.MessageBase = MensagemPadrao.Message(5, "Operação").ToString();
                value.mensagem.Message = "Campo obrigatório: Operação (D/C).";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }            

            if (value.ind_tipoEvento == "")
            {
                value.mensagem.Code = 5;
                value.mensagem.MessageBase = MensagemPadrao.Message(5, "Tipo do evento").ToString();
                value.mensagem.Message = "Campo obrigatório: Tipo do evento.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            // Validate Contabilidade
            if (value.Contabilidade != null)
            {
                ContabilidadeModel contabilidadeModel = new ContabilidadeModel(this.db, this.seguranca_db);
                value.mensagem = contabilidadeModel.Validate(value.Contabilidade, operation);
                if (value.mensagem.Code > 0)
                    return value.mensagem;
            }

            // Validate Movimento Bancário
            if (value.MovtoBancario != null)
            {
                MovtoBancarioModel movtoBancarioModel = new MovtoBancarioModel(this.db, this.seguranca_db);
                value.mensagem = movtoBancarioModel.Validate(value.MovtoBancario, operation);                
            }
            
            return value.mensagem;
        }

        public override ContaPagarParcelaEventoViewModel CreateRepository(System.Web.HttpRequestBase Request = null)
        {
            ContaPagarParcelaEventoViewModel contaPagarParcelaEventoViewModel = new ContaPagarParcelaEventoViewModel()
            {
                parcelaId = 1,
                dt_evento = Funcoes.Brasilia(),
                dt_ocorrencia = Funcoes.Brasilia().Date,
                dt_movto = Funcoes.Brasilia().Date
            };

            return contaPagarParcelaEventoViewModel;
        }
        #endregion

        #region Métodos Customizados

        #endregion
    }
}