﻿using App_Dominio.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DWM.Models.Enumeracoes
{
    public class Enumeradores
    {
        public enum Param
        {
            //GRUPO_USUARIO = 1,
            //SISTEMA = 2,
            //EMPRESA = 3,
            HABILITA_EMAIL = 6,
            FUSO_HORARIO = 7
        }

        /// <summary>
        /// Tipo de Conta Contábil
        /// </summary>
        public enum TipoContaContabil
        {
            [StringDescription("Sintética")]
            [StringValue("S")]
            SINTETICA,
            [StringDescription("Analítica")]
            [StringValue("A")]
            ANALITICA
        }

        public enum TipoHistorico
        {
            [StringDescription("Contabilidade")]
            [StringValue("C")]
            CONTABILIDADE,
            [StringDescription("Contas a Pagar")]
            [StringValue("P")]
            PAGAR,
            [StringDescription("Contas a Receber")]
            [StringValue("R")]
            RECEBER,
        }

        public enum FormaPagamento
        {
            [StringDescription("Dinheiro")]
            [StringValue("1")]
            DINHEIRO,
            [StringDescription("Cheque")]
            [StringValue("2")]
            CHEQUE,
            [StringDescription("Cartão de crédito")]
            [StringValue("3")]
            CARTAO,
            [StringDescription("Boleto")]
            [StringValue("4")]
            BOLETO,
            [StringDescription("Nota promissória")]
            [StringValue("5")]
            NOTA_PROMISSORIA,
            [StringDescription("Nota fiscal")]
            [StringValue("6")]
            NOTA_FISCAL,
            [StringDescription("Outros")]
            [StringValue("9")]
            OUTROS,
        }

        public enum ClassificacaoBanco
        {
            [StringDescription("Conta/Corrente")]
            [StringValue("C/C")]
            CONTA_CORRENTE,
            [StringDescription("Poupança")]
            [StringValue("POU")]
            POUPANCA,
            [StringDescription("Investimento")]
            [StringValue("INV")]
            INVESTIMENTO,
        }

        public enum TipoEvento
        {
            [StringDescription("Inclusão de operação")]
            [StringValue("0")]
            INCLUSAO_OPERACAO,
            [StringDescription("Encargos (multa, juros, encargos, tributos, tarifa)")]
            [StringValue("1")]
            ENCARGOS,
            [StringDescription("Desconto")]
            [StringValue("2")]
            DESCONTO,
            [StringDescription("Amortização")]
            [StringValue("3")]
            AMORTIZACAO,
            [StringDescription("Baixa por motivo de liquidação")]
            [StringValue("4")]
            LIQUIDACAO,
            [StringDescription("Baixa por motivo de cancelamento")]
            [StringValue("5")]
            CANCELAMENTO,
            [StringDescription("Baixa por motivo de renegociação")]
            [StringValue("6")]
            RENEGOCIACAO,
            [StringDescription("Baixa por motivo de desconto")]
            [StringValue("7")]
            BAIXA_DESCONTO,
            [StringDescription("Alteração de título")]
            [StringValue("8")]
            ALTERACAO_TITULO,
            [StringDescription("Estorno de lancamento")]
            [StringValue("9")]
            ESTORNO,
        }

        public enum Operacao
        {
            [StringDescription("Débito")]
            [StringValue("D")]
            DEBITO,
            [StringDescription("Crédito")]
            [StringValue("C")]
            CREDITO,
        }

        public enum Modalidade
        {
            [StringDescription("Contas a pagar")]
            [StringValue("P")]
            PAGAR,
            [StringDescription("Contas a receber")]
            [StringValue("R")]
            RECEBER,
            [StringDescription("Evento de cobrança")]
            [StringValue("C")]
            COBRANCA,
        }
    }
}