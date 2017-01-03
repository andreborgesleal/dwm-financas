using System.ComponentModel;
using App_Dominio.Component;

namespace DWM.Models.Repositories
{
    public class ExercicioViewModel : Repository
    {
        [DisplayName("Exercicio")]
        public int exercicio { get; set; }

        [DisplayName("Dt_Inicio")]
        public System.DateTime dt_inicio { get; set; }

        [DisplayName("Dt_Fim")]
        public System.DateTime dt_fim { get; set; }

        [DisplayName("dt_lancamento_inicio")]
        public System.Nullable<System.DateTime> dt_lancamento_inicio { get; set; }

        [DisplayName("dt_lancamento_fim")]
        public System.Nullable<System.DateTime> dt_lancamento_fim { get; set; }

        [DisplayName("MascaraPC")]
        public string mascaraPc { get; set; }

        [DisplayName("Encerrado")]
        public string encerrado { get; set; }
    }
}