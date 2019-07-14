﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;
using App_Dominio.Contratos;
using DWM.Models.Entidades;
using System.Linq;

namespace DWM.Models.Repositories
{
    public class ContabilidadeItemViewModel : Repository
    {
        [DisplayName("ID")]
        [Required(ErrorMessage = "Campo obrigatório: Contabilidade ID")]
        public int contabilidadeId { get; set; }

        [DisplayName("Sequencial")]
        [Required(ErrorMessage = "Campo obrigatório: Sequencial")]
        public int sequencial { get; set; }

        [DisplayName("C.Custo")]
        public System.Nullable<int> centroCustoId { get; set; }

        public string descricao_centroCusto { get; set; }

        [DisplayName("Conta Contábil")]
        public int planoContaId { get; set; }

        public string codigoPleno { get; set; } 

        public int codigoReduzido { get; set; }

        public string descricao_planoConta { get; set; }

        public string GetDescricaoContaMaster()
        {
            using (ApplicationContext db = new ApplicationContext())
                return db.PlanoContas.Where(info => info.empresaId == empresaId && info.codigoPleno.Substring(0, 1) == codigoPleno.Substring(0, 1)).Select(info => info.descricao).FirstOrDefault();
        }

        [DisplayName("Histórico")]
        public int historicoId { get; set; }

        public string descricao_historico { get; set; }

        [DisplayName("Complemento Histórico")]
        [StringLength(300, ErrorMessage = "O complemento do histórico deve ter no máximo 300 caracteres")]
        public string complementoHist { get; set; }

        [DisplayName("Tipo")]
        public string tipoLancamento { get; set; }

        [DisplayName("Valor")]
        public decimal valor { get; set; }

        public string operacao { get; set; }

        [DisplayName("Documento")]
        public string DocumentoURL { get; set; }

        public ContabilidadeItemViewModel()
        {
            mensagem = new Validate() { Code = 0, Message = "Item incluído com sucesso" };
        }
    }
}