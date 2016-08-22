using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;
using DWM.Models.Entidades;


namespace DWM.Models.Repositories
{
    public class CidadeViewModel : Repository
    {
        [Key]
        [DisplayName("C.Custo ID")]
        public int cidadeId { get; set; }

        [DisplayName("Descrição")]
        [Required(ErrorMessage = "Por favor, informe o nome da cidade")]
        [StringLength(25, ErrorMessage = "O nome da cidade deve ter no máximo 25 caracteres")]
        public string nome { get; set; }
    }
}