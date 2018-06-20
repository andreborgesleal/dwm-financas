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
using System.Text;

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

                string dj = !r.DataJuros.HasValue ? "null" : "'" + r.DataJuros.Value.ToString("yyyyMMdd") + "'";
                string vj = !r.ValorJuros.HasValue ? "null" : r.ValorJuros.Value.ToString("########0.00").Replace(",",".");
                string d1 = !r.DataDesconto1.HasValue ? "null" : "'" + r.DataDesconto1.Value.ToString("yyyyMMdd") + "'";
                string vd1 = !r.ValorDesconto1.HasValue ? "null" : r.ValorDesconto1.Value.ToString("########0.00").Replace(",", ".");
                string d2 = !r.DataDesconto2.HasValue ? "null" : "'" + r.DataDesconto2.Value.ToString("yyyyMMdd") + "'";
                string vd2 = !r.ValorDesconto2.HasValue ? "null" : r.ValorDesconto2.Value.ToString("########0.00").Replace(",", ".");
                string d3 = !r.DataDesconto3.HasValue ? "null" : "'" + r.DataDesconto3.Value.ToString("yyyyMMdd") + "'";
                string vd3 = !r.ValorDesconto3.HasValue ? "null" : r.ValorDesconto3.Value.ToString("########0.00").Replace(",", ".");
                string iof = !r.ValorIOF.HasValue ? "null" : r.ValorIOF.Value.ToString("########0.00").Replace(",", ".");
                string aba = !r.ValorAbatimento.HasValue ? "null" : r.ValorAbatimento.Value.ToString("########0.00").Replace(",", ".");
                string dde = r.NumDiasDevolucao == null ? "null" : "'" + r.NumDiasDevolucao + "'";
                string mul = !r.ValorMulta.HasValue ? "null" : r.ValorMulta.Value.ToString("########0.00").Replace(",", ".");
                string dmu = !r.DataMulta.HasValue ? "null" : "'" + r.DataMulta.Value.ToString("yyyyMMdd") + "'";
                string ccu = !r.centroCustoId.HasValue ? "null" : r.centroCustoId.ToString();
                string enq = !r.enquadramentoId.HasValue ? "null" : r.enquadramentoId.ToString();
                string rod = r.InstrucaoRodape == null ? "null" : "'" + r.InstrucaoRodape + "'";
                string in1 = r.InstrucaoPagamento1 == null ? "null" : "'" + r.InstrucaoPagamento1 + "'";
                string in2 = r.InstrucaoPagamento1 == null ? "null" : "'" + r.InstrucaoPagamento2 + "'";
                string in3 = r.InstrucaoPagamento1 == null ? "null" : "'" + r.InstrucaoPagamento3 + "'";
                string in4 = r.InstrucaoPagamento1 == null ? "null" : "'" + r.InstrucaoPagamento4 + "'";

                StringBuilder sp = new StringBuilder();
                sp.Append("exec spr_titulo_incluir ");
                sp.Append("@pOperacaoId = " + r.operacaoId.ToString() + ", ");
                sp.Append("@pParcelaId  = " + r.parcelaId + ", ");
                sp.Append("@pSequenciaID = " + r.SequenciaID + ", ");
                sp.Append("@pBancoID = '" + r.BancoID + "', ");
                sp.Append("@pConvenioID = '" + r.ConvenioID + "', ");
                sp.Append("@pEmpresaId = " + r.empresaId + ", ");
                sp.Append("@pClienteId = " + r.clienteId + ", ");
                sp.Append("@pTituloID = '" + r.TituloID + "', ");
                sp.Append("@pOcorrenciaID = '" + r.OcorrenciaID + "', ");
                sp.Append("@pNossoNumero = '" + r.NossoNumero + "', ");
                sp.Append("@pNossoNumeroDV = '" + r.NossoNumeroDV + "', ");
                sp.Append("@pSeuNumero = '" + r.SeuNumero + "', ");
                sp.Append("@pDataVencimento = '" + r.DataVencimento.ToString("yyyyMMdd") + "', ");
                sp.Append("@pValorPrincipal = " + r.ValorPrincipal.ToString("######0.00").Replace(",", ".") + ", ");
                sp.Append("@pEspecie = '" + r.Especie + "', ");
                sp.Append("@pAceite = '" + r.Aceite + "', ");
                sp.Append("@pDataEmissao = '" + r.DataEmissao.ToString("yyyyMMdd") + "', ");
                sp.Append("@pDataJuros = " + dj + ", ");
                sp.Append("@pValorJuros = " + vj + ", ");
                sp.Append("@pDataDesconto1 = " + d1 + ", ");
                sp.Append("@pValorDesconto1 = " + vd1 + ", ");
                sp.Append("@pDataDesconto2 = " + d2 + ", ");
                sp.Append("@pValorDesconto2 = " + vd2 + ", ");
                sp.Append("@pDataDesconto3 = " + d3 + ", ");
                sp.Append("@pValorDesconto3 = " + vd3 + ", ");
                sp.Append("@pValorIOF = " + iof + ", ");
                sp.Append("@pValorAbatimento = " + aba + ", ");
                sp.Append("@pNumDiasDevolucao = " + dde + ", ");
                sp.Append("@pMultaID = '" + r.MultaID + "', ");
                sp.Append("@pDataMulta = " + dmu + ", ");
                sp.Append("@pValorMulta = " + mul + ", ");
                sp.Append("@pInstrucaoRodape = " + rod + ", ");
                sp.Append("@pInstrucaoPagamento1 = " + in1 + ", ");
                sp.Append("@pInstrucaoPagamento2 = " + in2 + ", ");
                sp.Append("@pInstrucaoPagamento3 = " + in3 + ", ");
                sp.Append("@pInstrucaoPagamento4 = " + in4 + ", ");
                sp.Append("@pIndAtivo = " + r.IndAtivo + ", ");
                sp.Append("@pHistoricoId = " + r.historicoId + ", ");
                sp.Append("@pComplementoHist = '" + r.complementoHist + "', ");
                sp.Append("@pCentroCustoId = " + ccu + ", ");
                sp.Append("@pEnquadramentoId = " + enq + ", ");
                sp.Append("@pDocumento = '" + r.documento + "', ");
                sp.Append("@pERROR_CODE = 0, ");
                sp.Append("@pERROR_DESC = ''");

                IEnumerable<TituloViewModel> result = db.Database.SqlQuery<TituloViewModel>(sp.ToString());

                r.mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso !!!", MessageBase = "Registro incluído com sucesso !!!" };

                if (result.FirstOrDefault().CodErro > 0)
                    throw new App_DominioException(new Validate() { Code = result.FirstOrDefault().CodErro, Message = result.FirstOrDefault().DescErro, MessageBase = result.FirstOrDefault().DescErro });

                r.operacaoId = result.FirstOrDefault().operacaoId;
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