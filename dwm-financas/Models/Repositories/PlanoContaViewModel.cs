using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;
using DWM.Models.Entidades;

namespace DWM.Models.Repositories
{
    public class PlanoContaViewModel : Repository
    {
        [Key]
        [DisplayName("ID")]
        public int planoContaId { get; set; }

        [DisplayName("Exercício")]
        [Required(ErrorMessage = "Por favor, informe o exercício")]
        public int exercicio { get; set; }

        [DisplayName("Código Pleno")]
        [Required(ErrorMessage = "Por favor, informe o código pleno")]
        [StringLength(20, ErrorMessage = "O código pleno deve ter no máximo 20 caracteres")]
        public string codigoPleno { get; set; }

        [DisplayName("Código Reduzido")]
        [Required(ErrorMessage = "Por favor, informe o código reduzido")]
        public int codigoReduzido { get; set; }

        [DisplayName("Plano de Conta Pai")]
        public int? planoContaId_pai { get; set; }

        public string descricao_pai { get; set; }

        [DisplayName("Descrição")]
        [Required(ErrorMessage = "Por favor, informe a descrição do plano de conta")]
        [StringLength(60, ErrorMessage = "A descrição deve ter no máximo 60 caracteres")]
        public string descricao { get; set; }

        [DisplayName("Tipo de Conta")]
        public string tipoConta { get; set; }

        [DisplayName("Saldo Inicial")]
        public System.Nullable<decimal> vr_saldo_inicial { get; set; }

        public string mascaraPC { get; set; }

        public int grauPc
        {
            get
            {
                int grau = 1;
                if (codigoPleno != null)
                {
                    string[] id = codigoPleno.Replace("-","").Split('.');
                    for (int i = 1; i <= id.Length - 1; i++)
                    {
                        if (int.Parse(id[i]) == 0)
                            break;
                        grau++;
                    }
                    return grau;
                }
                else
                    return -1;
            }
        }

        public IEnumerable<PlanoConta> lastThree { get; set; }
    }
}