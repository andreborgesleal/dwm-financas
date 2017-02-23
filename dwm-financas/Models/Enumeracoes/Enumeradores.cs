using App_Dominio.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DWM.Models.Enumeracoes
{
    public static class Enumeradores
    {
        public enum Param
        {
            //GRUPO_USUARIO = 1,
            SISTEMA = 3,
            //EMPRESA = 3,
            HABILITA_EMAIL = 6,
            FUSO_HORARIO = 7,
            EXERCICIO_CONTABIL = 15
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
            [StringDescription("Cartão")]
            [StringValue("3")]
            CARTAO,
            [StringDescription("Boleto")]
            [StringValue("4")]
            BOLETO,
            [StringDescription("TED/DOC")]
            [StringValue("5")]
            TED_DOC,
            [StringDescription("Pag. Eletrônico")]
            [StringValue("6")]
            PAG_SEGURO,
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

        public enum EmailTipo
        {
            [StringDescription("Informativo")]
            [StringValue("1")]
            INFORMATIVO = 1,

            [StringDescription("Cadastro Condômino")]
            [StringValue("2")]
            CADASTRO_CONDOMINO = 2,

            [StringDescription("Cadastro Convite (token)")]
            [StringValue("3")]
            CADASTRO_CONVITE = 3,

            [StringDescription("Cadastro Proprietário")]
            [StringValue("4")]
            CADASTRO_PROPRIETARIO = 4,

            [StringDescription("Cadastro Credenciado")]
            [StringValue("5")]
            CADASTRO_CREDENCIADO = 5,

            [StringDescription("Chamado")]
            [StringValue("6")]
            CHAMADO = 6,

            [StringDescription("Esqueci minha senha")]
            [StringValue("7")]
            FORGOT = 7,

            [StringDescription("Outros")]
            [StringValue("9")]
            OUTROS = 9,

        }
    }
}