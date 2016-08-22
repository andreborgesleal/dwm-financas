using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;
using App_Dominio.Contratos;
using System.Linq;
using App_Dominio.Models;

namespace DWM.Models.Repositories
{
    public class _ContaPagarParcelaViewModel : Repository, IMasterRepository<ContaPagarParcelaEventoViewModel>
    {
        [DisplayName("ID")]
        public int operacaoId { get; set; }

        [DisplayName("Parcela ID")]
        [Required(ErrorMessage = "Campo obrigatório: Parcela ID")]
        public int parcelaId { get; set; }

        [DisplayName("Banco")]
        public System.Nullable<int> bancoId { get; set; }

        public string nome_banco { get; set; }

        [DisplayName("Nº Titulo")]
        [StringLength(20, ErrorMessage="Número do título deve possuir no máximo 20 caracteres")]
        public string num_titulo { get; set; }

        [DisplayName("Vencimento")]
        [Required(ErrorMessage = "Campo Obrigatório: Dt. Vencimento")]
        public DateTime dt_vencimento { get; set; }

        public int? atraso { 
            get 
            {
                int perm = 0;
                if (ind_baixa == null)
                    perm = Funcoes.DateDiff(dt_vencimento, Funcoes.Brasilia().Date);
                else if(ind_baixa == "4")
                    perm = Funcoes.DateDiff(dt_vencimento, dt_ultima_amortizacao.Value);
                if (perm <= 0)
                    return null;
                else
                    return perm;
            } 
        }

        [DisplayName("Valor Principal")]
        [Required(ErrorMessage = "Campo Obrigatório: Valor Principal")]
        public decimal vr_principal { get; set; }

        [DisplayName("Amortização")]
        public System.Nullable<decimal> vr_amortizacao { get; set; }

        [DisplayName("Desconto")]
        public System.Nullable<decimal> vr_desconto { get; set; }

        [DisplayName("Encargos")]
        public System.Nullable<decimal> vr_encargos { get; set; }

        [DisplayName("Total Pago")]
        public System.Nullable<decimal> vr_total_pago { get; set; }

        [DisplayName("Saldo Devedor")]
        public System.Nullable<decimal> vr_saldo_devedor { get; set; }

        [DisplayName("Forma Pagamento")]
        public string ind_forma_pagamento { get; set; }

        public string descricao_forma_pagamento { 
            get 
            {
                switch (ind_forma_pagamento)
                {
                    case "1": 
                        return "Dinheiro";
                    case "2":
                        return "Cheque";
                    case "3":
                        return "Cartão";
                    case "4":
                        return "Boleto";
                    case "5":
                        return "Nota promissória";
                    case "6":
                        return "Nota fiscal";
                    case "9":
                        return "Outros";
                    default:
                        return "Outors";
                }
            } 
        }

        [DisplayName("Codigo Barras")]
        public string codigo_barras { get; set; }

        [DisplayName("Última Amortizaçãoo")]
        public System.Nullable<DateTime> dt_ultima_amortizacao { get; set; }

        [DisplayName("Baixa")]
        public string ind_baixa { get; set; }

        public string situacao { 
            get
            {
                switch (ind_baixa)
                {
                    case "4":
                        return "Baixa por liquidação";
                    case "5":
                        return "Baixa por cancelamento";
                    case "6":
                        return "Baixa por renegociação";
                    case "7":
                        return "Baixa por desconto";
                    default:
                        return "Em aberto";
                }
            }
        }

        [DisplayName("Dt. Baixa")]
        public System.Nullable<DateTime> dt_baixa { get; set; }

        [DisplayName("Cheque")]
        public System.Nullable<int> cheque_banco { get; set; }

        [DisplayName("Agência")]
        public string cheque_agencia { get; set; }

        [DisplayName("Nº Cheque")]
        public string cheque_numero { get; set; }

        public ContaPagarParcelaEventoViewModel ContaPagarParcelaEvento { get; set; }

        public IEnumerable<ContaPagarParcelaEventoViewModel> ContaPagarParcelaEventos { get; set; }

        public string operacao { get; set; }

        public ContaPagarParcelaViewModel()
        {
            mensagem = new Validate() { Code = 0, Message = "Item incluído com sucesso" };
        }

        #region métodos da interface IMasterRepository
        public void CreateItem()
        {
            ContaPagarParcelaEvento = new ContaPagarParcelaEventoViewModel();
        }

        public IEnumerable<ContaPagarParcelaEventoViewModel> GetItems()
        {
            return ContaPagarParcelaEventos;
        }

        public ContaPagarParcelaEventoViewModel GetItem()
        {
            return ContaPagarParcelaEvento;
        }

        public void SetItems(IEnumerable<ContaPagarParcelaEventoViewModel> value)
        {
            ContaPagarParcelaEventos = value;
        }

        public void SetItem(ContaPagarParcelaEventoViewModel value)
        {
            ContaPagarParcelaEvento = value;
        }
        #endregion

    }
}