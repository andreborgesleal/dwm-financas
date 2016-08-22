using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;

namespace DWM.Models.Repositories
{
    public class ContaReceberViewModel : OperacaoViewModel<ContaReceberParcelaViewModel, ContaReceberParcelaEventoViewModel>
    {
        [DisplayName("Cliente")]
        [Required(ErrorMessage = "Campo obrigatório: Cliente")]
        public int clienteId { get; set; }

        public string nome_cliente { get; set; }
        public string descricao_grupoCliente { get; set; }
    }
}