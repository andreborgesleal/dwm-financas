using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;

namespace DWM.Models.Repositories
{
    public class FeriadoViewModel : Repository
    {
        [DisplayName("ID")]
        public int feriadoId { get; set; }

        [DisplayName("CidadeID")]
        public Nullable<int> cidadeId { get; set; }

        [DisplayName("Data do Feriado")]
        [Required(ErrorMessage = "Campo obrigatório: Dt.Lançamento")]
        public DateTime dt_feriado { get; set; }

        public string descricao { get; set; }
    }
}