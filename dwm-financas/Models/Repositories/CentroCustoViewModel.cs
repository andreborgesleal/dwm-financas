using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;
using DWM.Models.Entidades;

namespace DWM.Models.Repositories
{
    public class CentroCustoViewModel : Repository
    {
        [Key]
        [DisplayName("C.Custo ID")]
        public int centroCustoId { get; set; }

        [DisplayName("Descrição")]
        [Required(ErrorMessage = "Por favor, informe o nome do centro de custo")]
        [StringLength(30, ErrorMessage = "O nome do centro de custo deve ter no máximo 30 caracteres")]
        public string descricao { get; set; }
    }
}