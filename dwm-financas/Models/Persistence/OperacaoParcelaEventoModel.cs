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
    public abstract class OperacaoParcelaEventoModel<OPE, OPERepo> : CrudModel<OPE, OPERepo, ApplicationContext>
        where OPE : OperacaoParcelaEvento
        where OPERepo : OperacaoParcelaEventoViewModel
    {
        #region Constructor
        public OperacaoParcelaEventoModel() { }
        public OperacaoParcelaEventoModel(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        #region Métodos da classe CrudModel
        public override OPERepo BeforeInsert(OPERepo value)
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
                {
                    if (value.Contabilidade.ContabilidadeItems.ElementAt(contador).valor == 0)
                        value.Contabilidade.ContabilidadeItems.ElementAt(contador).valor = value.valor;
                    contador++;
                }

            };
            #endregion

            return value;
        }

        public override OPERepo AfterInsert(OPERepo value)
        {
            try
            {
                // FOI SUBSTITUÍDO POR UMA TRIGGER
                //**********************************
                //#region UC-043 Atualizar saldo da parcela de contas a receber
                //AtualizarSaldoContaReceber atu = new AtualizarSaldoContaReceber(this.db, this.seguranca_db);
                //ContaReceberParcelaCrudModel par = new ContaReceberParcelaCrudModel(this.db, this.seguranca_db);
                //ContaReceberParcelaViewModel r = par.getObject(new ContaReceberParcelaViewModel() { operacaoId = value.operacaoId, parcelaId = value.parcelaId });
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
                    if (f.Exists)
                        f.MoveTo(Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Users_Data/Empresas/" + sessaoCorrente.empresaId.ToString() + "/arquivos"), value.arquivo));
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

        public override OPE MapToEntity(OPERepo value)
        {
            OPE operacaoParcelaEvento = null;
            if (value.uri != null && value.uri != "")
            {
                operacaoParcelaEvento = Find(value);

                if (operacaoParcelaEvento == null)
                {
                    operacaoParcelaEvento = getEntityInstance();
                }
                else
                {
                    operacaoParcelaEvento.movtoBancarioId = null;
                    operacaoParcelaEvento.contabilidadeId = null;
                    operacaoParcelaEvento.Contabilidade = null;
                    operacaoParcelaEvento.MovtoBancario = null;
                }
            }
            else
                operacaoParcelaEvento = getEntityInstance();

            operacaoParcelaEvento.operacaoId = value.operacaoId;
            operacaoParcelaEvento.parcelaId = value.parcelaId;
            operacaoParcelaEvento.dt_evento = value.dt_evento;
            operacaoParcelaEvento.eventoId = value.eventoId;
            operacaoParcelaEvento.dt_ocorrencia = value.dt_ocorrencia;
            operacaoParcelaEvento.valor = value.valor;
            operacaoParcelaEvento.ind_operacao = value.ind_operacao;
            operacaoParcelaEvento.ind_estorno = value.ind_estorno;
            operacaoParcelaEvento.ind_tipoEvento = value.ind_tipoEvento;
            operacaoParcelaEvento.arquivo = value.arquivo;

            #region Mapping Movimentação bancária to Entity
            if (value.MovtoBancario != null) // Amortização ou Baixa por motivo de liquidação (gera MOVIMENTO BANCÁRIO)
            {
                MovtoBancarioModel movtoBancarioModel = new MovtoBancarioModel();
                operacaoParcelaEvento.MovtoBancario = movtoBancarioModel.MapToEntity(value.MovtoBancario);
            }
            #endregion

            #region Mapping contabilidade to Entity
            if (value.Contabilidade != null)
            {
                ContabilidadeModel contabilidadeModel = new ContabilidadeModel(this.db, this.seguranca_db);
                operacaoParcelaEvento.Contabilidade = contabilidadeModel.MapToEntity(value.Contabilidade);
            };
            #endregion

            return operacaoParcelaEvento;
        }

        public override OPERepo MapToRepository(OPE entity)
        {
            OPERepo r = base.CreateRepository();

            r.mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS };
            r.operacaoId = entity.operacaoId;
            r.parcelaId = entity.parcelaId;
            r.dt_evento = entity.dt_evento;
            r.eventoId = entity.eventoId;
            r.codigo = db.Eventos.Where(info => info.eventoId == entity.eventoId).Select(info => info.codigo).FirstOrDefault();
            r.descricao_evento = db.Eventos.Where(info => info.eventoId == entity.eventoId).Select(info => info.descricao).FirstOrDefault();
            r.dt_ocorrencia = entity.dt_ocorrencia;
            r.valor = entity.valor;
            r.arquivo = entity.arquivo;
            r.ind_operacao = entity.ind_operacao;
            r.ind_estorno = entity.ind_estorno;
            r.ind_tipoEvento = entity.ind_tipoEvento;

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

        public override Validate Validate(OPERepo value, Crud operation)
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

        public override OPERepo CreateRepository(System.Web.HttpRequestBase Request = null)
        {
            OPERepo operacaoParcelaEventoViewModel = base.CreateRepository();

            operacaoParcelaEventoViewModel.parcelaId = 1;
            operacaoParcelaEventoViewModel.dt_evento = Funcoes.Brasilia();
            operacaoParcelaEventoViewModel.dt_ocorrencia = Funcoes.Brasilia().Date;
            operacaoParcelaEventoViewModel.dt_movto = Funcoes.Brasilia().Date;

            return operacaoParcelaEventoViewModel;
        }
        #endregion

        #region Métodos Customizados

        #endregion
    }
}