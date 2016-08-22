using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;
using App_Dominio.Contratos;
using System.Linq;

namespace DWM.Models.Repositories
{
    public class EnquadramentoViewModel : Repository, IMasterRepository<EnquadramentoItemViewModel>
    {
        [DisplayName("ID")]
        public int enquadramentoId { get; set; }

        [DisplayName("Exercício")]
        [Required(ErrorMessage = "Por favor, informe o exercício contábil")]
        public int exercicio { get; set; }

        [DisplayName("Descrição")]
        [Required(ErrorMessage = "Campo obrigatório: Descrição")]
        [StringLength(50, ErrorMessage = "A descrição do enquadramento deve ter no máximo 50 caracteres")]
        public string descricao { get; set; }

        public EnquadramentoItemViewModel EnquadramentoItem { get; set; }

        public virtual IEnumerable<EnquadramentoItemViewModel> EnquadramentoItems { get; set; }

        #region métodos da interface IMasterRepository
        public void CreateItem()
        {
            if (GetItems().Count() == 0)
                EnquadramentoItem = new EnquadramentoItemViewModel();
            else
                EnquadramentoItem = new EnquadramentoItemViewModel()
                {
                    centroCustoId = GetItems().Last().centroCustoId,
                    descricao_centroCusto = GetItems().Last().descricao_centroCusto,
                    historicoId = GetItems().Last().historicoId,
                    descricao_historico = GetItems().Last().descricao_historico,
                    complementoHist = GetItems().Last().complementoHist
                };
        }

        public IEnumerable<EnquadramentoItemViewModel> GetItems()
        {
            return EnquadramentoItems;
        }

        public EnquadramentoItemViewModel GetItem()
        {
            return EnquadramentoItem;
        }

        public void SetItems(IEnumerable<EnquadramentoItemViewModel> value)
        {
            EnquadramentoItems = value;
        }

        public void SetItem(EnquadramentoItemViewModel value)
        {
            EnquadramentoItem = value;
        }
        #endregion
    }
}