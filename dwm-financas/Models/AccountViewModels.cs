using App_Dominio.Component;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DWM.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Login")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha Atual")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "O campo {0} deve ter pelo menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)] 
        [Display(Name = "Confirmação da nova senha")]
        [Compare("Confirmação de senha", ErrorMessage = "As senhas não combinam.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage="O campo login é de preenhcimento obrigatório e deve ser um e-mail válido")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Informe um e-mail válido")]
        [EmailAddress(ErrorMessage="Informe o login com um formato de e-mail válido")]
        [Display(Name = "Login")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "O campo Senha é de preenhcimento obrigatório")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Display(Name = "Lembrar-me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel : Repository
    {
        public int usuarioId { get; set; }

        public int? associadoId { get; set; }

        [Required(ErrorMessage = "O nome do usuário deve ser informado.")]
        [StringLength(40, ErrorMessage = "O nome do Usuário deve possuir no mínimo 10 e no máximo 40 caracteres", MinimumLength = 10)]
        [Display(Name = "Nome")]
        public string nome { get; set; }

        [Required(ErrorMessage = "O campo login é de preenhcimento obrigatório e deve ser um e-mail válido")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Informe um e-mail válido")]
        [EmailAddress]
        [Display(Name = "Login")]
        public string login { get; set; }

        [Required(ErrorMessage = "O campo Senha é de preenhcimento obrigatório")]
        [StringLength(20, ErrorMessage = "O campo {0} deve ter pelo menos {2} caracteres e no máximo 20 caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string senha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmação de senha")]
        [Compare("senha", ErrorMessage = "As senhas não conferem.")]
        public string confirmacaoSenha { get; set; }

        [Required(ErrorMessage = "O campo CRM é de preenhcimento obrigatório")]
        [StringLength(6, ErrorMessage = "O campo CRM deve ter no máximo 6 dígitos numéricos.")]
        [Display(Name = "CRM")]
        public string CRM { get; set; }

        [Required(ErrorMessage = "O campo UF é de preenhcimento obrigatório")]
        [StringLength(2, ErrorMessage = "O campo UF do CRM deve possuir 2 caracteres.", MinimumLength = 2)]
        [Display(Name = "UF")]
        public string ufCRM { get; set; }

        [Required(ErrorMessage = "O campo CPF é de preenhcimento obrigatório")]
        [Display(Name = "CPF")]
        public string cpf { get; set; }
    }
}
