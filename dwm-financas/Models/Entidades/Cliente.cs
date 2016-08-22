using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWM.Models.Entidades
{
    [Table("Cliente")]
    public class Cliente
    {
        [Key]
        [DisplayName("ID")]
        public int clienteId { get; set; }

        [DisplayName("EmpresaID")]
        public int empresaId { get; set; }

        [DisplayName("Grupo")]
        public Nullable<int> grupoClienteId { get; set; }

        [DisplayName("Nome")]
        public string nome { get; set; }

        [DisplayName("Tipo_Pessoa")]
        public string ind_tipo_pessoa { get; set; }

        [DisplayName("CPF_CNPJ")]
        public string cpf_cnpj { get; set; }

        [DisplayName("Dt_Inclusao")]
        public DateTime dt_inclusao { get; set; }

        [DisplayName("Dt_Altercao")]
        public DateTime dt_alteracao { get; set; }

        [DisplayName("Codigo")]
        public string codigo { get; set; }

        [DisplayName("Endereco")]
        public string endereco { get; set; }

        [DisplayName("Copmlemento")]
        public string complemento { get; set; }

        [DisplayName("Cidade")]
        public string cidade { get; set; }

        [DisplayName("UF")]
        public string uf { get; set; }

        [DisplayName("CEP")]
        public string cep { get; set; }

        [DisplayName("Bairro")]
        public string bairro { get; set; }

        [DisplayName("Celular_1")]
        public string fone1 { get; set; }

        [DisplayName("Celular_2")]
        public string fone2 { get; set; }

        [DisplayName("Celular_3")]
        public string fone3 { get; set; }

        [DisplayName("Email")]
        public string email { get; set; }

        [DisplayName("Sexo")]
        public string sexo { get; set; }

        [DisplayName("Dt_Nascimento")]
        public Nullable<DateTime> dt_nascimento { get; set; }

        [DisplayName("Observacao")]
        public string observacao { get; set; }
    }
}