using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;

namespace DWM.Models.Repositories
{
    public class GrupoCobrancaViewModel : Repository
    {
        [DisplayName("ID")]
        public int grupoCobrancaId { get; set; }

        [DisplayName("Descrição")]
        [Required(ErrorMessage = "Descrição do grupo de cobrança deve ser informado")]
        [StringLength(60, ErrorMessage = "Descrição do grupo de cobrança deve ter no máximo 60 caracteres")]
        public string descricao { get; set; }
    }
}