﻿using App_Dominio.Component;
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

namespace DWM.Models.BI
{
    public class ContaPagarLiquidarOperacaoBI : DWMContext<ApplicationContext>, IProcess<ContaPagarViewModel, ApplicationContext>
    {
        #region Constructor
        public ContaPagarLiquidarOperacaoBI() { }

        public ContaPagarLiquidarOperacaoBI(ApplicationContext _db, SecurityContext _seguranca_db)
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
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso" }
            };

            try
            {
                var _pOperacaoId = new SqlParameter { ParameterName = "pOperacaoId", DbType = System.Data.DbType.Int16, Value = r.operacaoId };
                var _pBancoId = new SqlParameter { ParameterName = "pBancoId", DbType = System.Data.DbType.Int16, Value = r.ContaPagarParcela.bancoId.Value };
                var _pInd_forma_pagamento = new SqlParameter { ParameterName = "pInd_forma_pagamento", DbType = System.Data.DbType.String, Value = r.ContaPagarParcela.ind_forma_pagamento };
                var _pDt_vencimento = new SqlParameter { ParameterName = "pDt_vencimento", DbType = System.Data.DbType.Date, Value = r.ContaPagarParcela.dt_baixa };
                var _pDt_pagamento = new SqlParameter { ParameterName = "pDt_pagamento", DbType = System.Data.DbType.Date, Value = r.ContaPagarParcela.dt_baixa };
                var _pDt_movto = new SqlParameter { ParameterName = "pDt_movto", DbType = System.Data.DbType.Date, Value = r.dt_movto };
                var _pCheque_banco = new SqlParameter { ParameterName = "pCheque_banco", DbType = System.Data.DbType.Int16, Value = r.ContaPagarParcela.cheque_banco ?? 0 };
                var _pCheque_agencia = new SqlParameter { ParameterName = "pCheque_agencia", DbType = System.Data.DbType.String, Value = r.ContaPagarParcela.cheque_agencia ?? ""};
                var _pCheque_numero = new SqlParameter { ParameterName = "pCheque_numero", DbType = System.Data.DbType.String, Value = r.ContaPagarParcela.cheque_numero ?? "" };
                var _pVr_desconto = new SqlParameter { ParameterName = "pVr_desconto", DbType = System.Data.DbType.Decimal, Value = r.ContaPagarParcela.vr_desconto ?? 0 };
                var _pVr_multa_atraso = new SqlParameter { ParameterName = "pVr_multa_atraso", DbType = System.Data.DbType.Decimal, Value = r.vr_multa ?? 0 };
                var _pVr_juros_mora = new SqlParameter { ParameterName = "pVr_juros_mora", DbType = System.Data.DbType.Decimal, Value = r.vr_jurosMora ?? 0 };
                var _pEnquadramentoId = new SqlParameter { ParameterName = "pEnquadramentoId", DbType = System.Data.DbType.Int16, Value = r.enquadramentoId ?? 0 };
                var _pArquivo = new SqlParameter { ParameterName = "pArquivo", DbType = System.Data.DbType.String, Value = r.fileComprovante };

                var _error_code = new SqlParameter
                {
                    ParameterName = "pERROR_CODE",
                    DbType = System.Data.DbType.Int16,
                    Direction = System.Data.ParameterDirection.Output
                };

                var _error_desc = new SqlParameter
                {
                    ParameterName = "pERROR_DESC",
                    DbType = System.Data.DbType.String,
                    Size = 1000,
                    Direction = System.Data.ParameterDirection.Output
                };

                #region stored procedure
                int result = db.Database.ExecuteSqlCommand("spr_liquidar_conta_pagar @pOperacaoId, " + 
                                                           "                         @pBancoId, " + 
                                                           "                         @pInd_forma_pagamento, " + 
                                                           "                         @pDt_vencimento, " + 
                                                           "                         @pDt_pagamento, " + 
                                                           "                         @pDt_movto, " + 
                                                           "                         @pCheque_banco, " + 
                                                           "                         @pCheque_agencia, " + 
                                                           "                         @pCheque_numero, " +
                                                           "                         @pVr_desconto, " +
                                                           "                         @pVr_multa_atraso, " + 
                                                           "                         @pVr_juros_mora, " +
                                                           "                         @pEnquadramentoId, " +
                                                           "                         @pArquivo, " + 
                                                           "                         @pERROR_CODE out, " + 
                                                           "                         @pERROR_DESC out",
                                                           _pOperacaoId,
                                                           _pBancoId,
                                                           _pInd_forma_pagamento,
                                                           _pDt_vencimento,
                                                           _pDt_pagamento,
                                                           _pDt_movto, 
                                                           _pCheque_banco, 
                                                           _pCheque_agencia, 
                                                           _pCheque_numero, 
                                                           _pVr_desconto, 
                                                           _pVr_multa_atraso, 
                                                           _pVr_juros_mora, 
                                                           _pEnquadramentoId, 
                                                           _pArquivo, 
                                                           _error_code,
                                                           _error_desc);
                #endregion

                if (_error_code.Value.ToString() != "0")
                    r.mensagem = new Validate() { Code = int.Parse(_error_code.Value.ToString()), Message = _error_desc.Value.ToString() };
                else
                    r.mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso" };
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

            return new ContaPagarViewModel()
            {
                mensagem = r.mensagem
            };
        }

        public IEnumerable<ContaPagarViewModel> List(params object[] param)
        {
            ContaPagarViewModel repository = new ContaPagarViewModel()
            {
                operacaoId = (int)param[0],
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso" }
            };

            ContaPagarModel model = new ContaPagarModel(this.db, this.seguranca_db);
            repository = model.getObject(repository);

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