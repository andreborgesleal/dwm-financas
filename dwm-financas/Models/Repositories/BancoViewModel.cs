using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;

namespace DWM.Models.Repositories
{
    public class BancoViewModel : Repository
    {
        [DisplayName("ID")]
        public int bancoId { get; set; }
        
        [DisplayName("Nome")]
        [Required(ErrorMessage = "Nome do banco deve ser informado")]
        public string nome { get; set; }

        [DisplayName("Classificação")]
        public string classificacao { get; set; }

        [DisplayName("Sigla")]
        public string sigla { get; set; }

        [DisplayName("Número")]
        public string numero { get; set; }

    }
}