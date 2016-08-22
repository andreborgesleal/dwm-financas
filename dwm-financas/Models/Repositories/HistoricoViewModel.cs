using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;
using DWM.Models.Entidades;

namespace DWM.Models.Repositories
{
    public class HistoricoViewModel : Repository
    {
        [Key]
        [DisplayName("Histórico ID")]
        public int historicoId { get; set; }

        [DisplayName("Descrição")]
        [Required(ErrorMessage = "Por favor, informe a descrição do histórico")]
        [StringLength(60, ErrorMessage = "A descrição deve ter no máximo 60 caracteres")]
        public string descricao { get; set; }

        [DisplayName("Tipo")]
        [Required(ErrorMessage = "Por favor, informe o tipo do histórico")]
        public string ind_tipoHistorico { get; set; }
    }
}