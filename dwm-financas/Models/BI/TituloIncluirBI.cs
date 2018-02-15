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
using System.Linq;
using System.Data.SqlClient;
using System.Data;

namespace DWM.Models.BI
{
    public class TituloIncluirBI : DWMContext<ApplicationContext>, IProcess<TituloIncluirViewModel, ApplicationContext>
    {
        #region Constructor
        public TituloIncluirBI() { }

        public TituloIncluirBI(ApplicationContext _db, SecurityContext _seguranca_db)
        {
            base.Create(_db, _seguranca_db);
        }
        #endregion

        public virtual TituloIncluirViewModel Run(Repository value)
        {
            TituloIncluirViewModel r = (TituloIncluirViewModel)value;
            try
            {

                TituloModel model = new TituloModel();
                model.Create(this.db, this.seguranca_db);
                TituloViewModel t = model.BeforeInsert(r);

                r.empresaId = t.empresaId;
                r.SequenciaID = t.SequenciaID;
                r.IndAtivo = t.IndAtivo;
                r.DataEmissao = t.DataEmissao;
                r.OcorrenciaID = t.OcorrenciaID;
                r.NossoNumero = t.NossoNumero;
                r.NossoNumeroDV = t.NossoNumeroDV;

                Validate v = model.Validate(r, App_Dominio.Enumeracoes.Crud.INCLUIR);
                if (v.Code > 0)
                {
                    r.mensagem = v;
                    return r;
                }

                #region Parâmetros
                SqlParameter operacaoIdParam = new SqlParameter("@pOperacaoId", SqlDbType.Int);
                SqlParameter parcelaIdParam = new SqlParameter("@pParcelaId", SqlDbType.Int);
                SqlParameter SequenciaIDParam = new SqlParameter("@pSequenciaID", SqlDbType.Int);
                SqlParameter BancoIDParam = new SqlParameter("@pBancoID", SqlDbType.NChar, 3);
                SqlParameter ConvenioIDParam = new SqlParameter("@pConvenioID", SqlDbType.NVarChar, 15);
                SqlParameter empresaIdParam = new SqlParameter("@pEmpresaId", SqlDbType.Int);
                SqlParameter clienteIdParam = new SqlParameter("@pClienteId", SqlDbType.Int);
                SqlParameter TituloIDParam = new SqlParameter("@pTituloID", SqlDbType.NVarChar, 25);
                SqlParameter OcorrenciaIDParam = new SqlParameter("@pOcorrenciaID", SqlDbType.NChar, 2);
                SqlParameter NossoNumeroParam = new SqlParameter("@pNossoNumero", SqlDbType.NChar, 8);
                SqlParameter NossoNumeroDVParam = new SqlParameter("@pNossoNumeroDV", SqlDbType.NChar, 1);
                SqlParameter SeuNumeroParam = new SqlParameter("@pSeuNumero", SqlDbType.NVarChar, 10);
                SqlParameter DataVencimentoParam = new SqlParameter("@pDataVencimento", SqlDbType.SmallDateTime);
                SqlParameter ValorPrincipalParam = new SqlParameter("@pValorPrincipal", SqlDbType.Decimal);
                SqlParameter EspecieParam = new SqlParameter("@pEspecie", SqlDbType.NChar, 2);
                SqlParameter AceiteParam = new SqlParameter("@pAceite", SqlDbType.NChar, 1);
                SqlParameter DataEmissaoParam = new SqlParameter("@pDataEmissao", SqlDbType.SmallDateTime);
                SqlParameter DataJurosParam = new SqlParameter("@pDataJuros", SqlDbType.SmallDateTime);
                SqlParameter ValorJurosParam = new SqlParameter("@pValorJuros", SqlDbType.Decimal);
                SqlParameter DataDesconto1Param = new SqlParameter("@pDataDesconto1", SqlDbType.SmallDateTime);
                SqlParameter ValorDesconto1Param = new SqlParameter("@pValorDesconto1", SqlDbType.Decimal);
                SqlParameter DataDesconto2Param = new SqlParameter("@pDataDesconto2", SqlDbType.SmallDateTime);
                SqlParameter ValorDesconto2Param = new SqlParameter("@pValorDesconto2", SqlDbType.Decimal);
                SqlParameter DataDesconto3Param = new SqlParameter("@pDataDesconto3", SqlDbType.SmallDateTime);
                SqlParameter ValorDesconto3Param = new SqlParameter("@pValorDesconto3", SqlDbType.Decimal);
                SqlParameter ValorIOFParam = new SqlParameter("@pValorIOF", SqlDbType.Decimal);
                SqlParameter ValorAbatimentoParam = new SqlParameter("@pValorAbatimento", SqlDbType.Decimal);
                SqlParameter NumDiasDevolucaoParam = new SqlParameter("@pNumDiasDevolucao", SqlDbType.NChar, 2);
                SqlParameter MultaIDParam = new SqlParameter("@pMultaID", SqlDbType.NChar, 1);
                SqlParameter DataMultaParam = new SqlParameter("@pDataMulta", SqlDbType.SmallDateTime);
                SqlParameter ValorMultaParam = new SqlParameter("@pValorMulta", SqlDbType.Decimal);
                SqlParameter InstrucaoRodapeParam = new SqlParameter("@pInstrucaoRodape", SqlDbType.NVarChar, 40);
                SqlParameter InstrucaoPagamento1Param = new SqlParameter("@pInstrucaoPagamento1", SqlDbType.NVarChar, 40);
                SqlParameter InstrucaoPagamento2Param = new SqlParameter("@pInstrucaoPagamento2", SqlDbType.NVarChar, 40);
                SqlParameter InstrucaoPagamento3Param = new SqlParameter("@pInstrucaoPagamento3", SqlDbType.NVarChar, 40);
                SqlParameter InstrucaoPagamento4Param = new SqlParameter("@pInstrucaoPagamento4", SqlDbType.NVarChar, 40);
                SqlParameter IndAtivoParam = new SqlParameter("@pIndAtivo", SqlDbType.Bit);
                SqlParameter historicoIdParam = new SqlParameter("@pHistoricoId", SqlDbType.Int);
                SqlParameter complementoHistParam = new SqlParameter("@pComplementoHist", SqlDbType.NVarChar, 300);
                SqlParameter centroCustoIdParam = new SqlParameter("@pCentroCustoId", SqlDbType.Int);
                SqlParameter enquadramentoIdParam = new SqlParameter("@pEnquadramentoId", SqlDbType.Int);
                SqlParameter documentoParam = new SqlParameter("@pDocumento", SqlDbType.NVarChar, 12);
                SqlParameter Cod_erroParam = new SqlParameter("@pCod_erro", SqlDbType.Int);
                SqlParameter Desc_erroParam = new SqlParameter("@pDesc_erro", SqlDbType.NVarChar, 400);

                // Parâmetros do título
                operacaoIdParam.Direction = ParameterDirection.Output;
                operacaoIdParam.Value = r.operacaoId;
                parcelaIdParam.Value = r.parcelaId;
                SequenciaIDParam.Value = r.SequenciaID;
                BancoIDParam.Value = r.BancoID;
                ConvenioIDParam.Value = r.ConvenioID;
                empresaIdParam.Value = sessaoCorrente.empresaId;
                clienteIdParam.Value = r.clienteId;
                TituloIDParam.Direction = ParameterDirection.Output;
                TituloIDParam.Value = r.TituloID;
                OcorrenciaIDParam.Value = r.OcorrenciaID;
                NossoNumeroParam.Value = r.NossoNumero;
                NossoNumeroDVParam.Value = r.NossoNumeroDV;
                SeuNumeroParam.Value = r.SeuNumero;
                DataVencimentoParam.Value = r.DataVencimento;
                ValorPrincipalParam.Value = r.ValorPrincipal;
                EspecieParam.Value = r.Especie;
                AceiteParam.Value = r.Aceite;
                DataEmissaoParam.Value = r.DataEmissao;
                DataJurosParam.Value = r.DataJuros;
                ValorJurosParam.Value = r.ValorJuros;
                DataDesconto1Param.Value = r.DataDesconto1;
                ValorDesconto1Param.Value = r.ValorDesconto1;
                DataDesconto2Param.Value = r.DataDesconto2;
                ValorDesconto2Param.Value = r.ValorDesconto2;
                DataDesconto3Param.Value = r.DataDesconto3;
                ValorDesconto3Param.Value = r.ValorDesconto3;
                ValorIOFParam.Value = r.ValorIOF;
                ValorAbatimentoParam.Value = r.ValorAbatimento;
                NumDiasDevolucaoParam.Value = r.NumDiasDevolucao;
                MultaIDParam.Value = r.MultaID;
                DataMultaParam.Value = r.DataMulta;
                ValorMultaParam.Value = r.ValorMulta;
                InstrucaoRodapeParam.Value = r.InstrucaoRodape;
                InstrucaoPagamento1Param.Value = r.InstrucaoPagamento1;
                InstrucaoPagamento2Param.Value = r.InstrucaoPagamento2;
                InstrucaoPagamento3Param.Value = r.InstrucaoPagamento3;
                InstrucaoPagamento4Param.Value = r.InstrucaoPagamento4;
                IndAtivoParam.Value = r.IndAtivo;
                // Parâmetros do Contas a Receber
                historicoIdParam.Value = r.historicoId;
                complementoHistParam.Value = r.complementoHist;
                centroCustoIdParam.Value = r.centroCustoId;
                enquadramentoIdParam.Value = r.enquadramentoId;
                documentoParam.Value = r.Documento;

                Cod_erroParam.Direction = ParameterDirection.Output;
                Cod_erroParam.Value = 0;
                Desc_erroParam.Direction = ParameterDirection.Output;
                Desc_erroParam.Value = "";
                #endregion

                int result = db.Database.ExecuteSqlCommand("spr_titulo_incluir", operacaoIdParam,
                                                                                 parcelaIdParam,
                                                                                 SequenciaIDParam,
                                                                                 BancoIDParam,
                                                                                 ConvenioIDParam,
                                                                                 empresaIdParam,
                                                                                 clienteIdParam,
                                                                                 TituloIDParam,
                                                                                 OcorrenciaIDParam,
                                                                                 NossoNumeroParam,
                                                                                 NossoNumeroDVParam,
                                                                                 SeuNumeroParam,
                                                                                 DataVencimentoParam,
                                                                                 ValorPrincipalParam,
                                                                                 EspecieParam,
                                                                                 AceiteParam,
                                                                                 DataEmissaoParam,
                                                                                 DataJurosParam,
                                                                                 ValorJurosParam,
                                                                                 DataDesconto1Param,
                                                                                 ValorDesconto1Param,
                                                                                 DataDesconto2Param,
                                                                                 ValorDesconto2Param,
                                                                                 DataDesconto3Param,
                                                                                 ValorDesconto3Param,
                                                                                 ValorIOFParam,
                                                                                 ValorAbatimentoParam,
                                                                                 NumDiasDevolucaoParam,
                                                                                 MultaIDParam,
                                                                                 DataMultaParam,
                                                                                 ValorMultaParam,
                                                                                 InstrucaoRodapeParam,
                                                                                 InstrucaoPagamento1Param,
                                                                                 InstrucaoPagamento2Param,
                                                                                 InstrucaoPagamento3Param,
                                                                                 InstrucaoPagamento4Param,
                                                                                 IndAtivoParam,
                                                                                 historicoIdParam,
                                                                                 complementoHistParam,
                                                                                 centroCustoIdParam,
                                                                                 enquadramentoIdParam,
                                                                                 documentoParam,
                                                                                 Cod_erroParam,
                                                                                 Desc_erroParam);

                r.mensagem = new Validate() { Code = (int)Cod_erroParam.Value, Message = Desc_erroParam.Value.ToString() };
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


        public IEnumerable<TituloIncluirViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }
    }
}