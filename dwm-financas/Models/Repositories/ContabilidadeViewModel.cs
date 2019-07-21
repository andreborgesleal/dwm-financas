using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;
using App_Dominio.Contratos;
using System.Linq;


namespace DWM.Models.Repositories
{
    public class ContabilidadeViewModel : Repository, IMasterRepository<ContabilidadeItemViewModel>
    {
        [DisplayName("ID")]
        public int contabilidadeId { get; set; }

        [DisplayName("Exercício")]
        [Required(ErrorMessage = "Campo obrigatório: Exercício contábil")]
        public int exercicio { get; set; }

        [DisplayName("Dt.Lançamento")]
        [Required(ErrorMessage = "Campo obrigatório: Dt.Lançamento")]
        public DateTime dt_lancamento { get; set; }

        [DisplayName("Documento")]
        [StringLength(20, ErrorMessage = "Documento deve ter no máximo 20 caracteres")]
        public string documento { get; set; }

        public ContabilidadeItemViewModel ContabilidadeItem { get; set; }

        public IEnumerable<ContabilidadeItemViewModel> ContabilidadeItems { get; set; }

        public int? operacaoId { get; set; }
        public int? parcelaId { get; set; }
        public string dt_evento { get; set; }
        public string natureza { get; set; }

        #region métodos da interface IMasterRepository
        public void CreateItem()
        {
            if (GetItems().Count() == 0)
                ContabilidadeItem = new ContabilidadeItemViewModel();
            else
                ContabilidadeItem = new ContabilidadeItemViewModel()
                {
                    centroCustoId = GetItems().Last().centroCustoId,
                    descricao_centroCusto = GetItems().Last().descricao_centroCusto,
                    historicoId = GetItems().Last().historicoId,
                    descricao_historico = GetItems().Last().descricao_historico,
                    complementoHist = GetItems().Last().complementoHist,
                    DocumentoURL = GetItems().Last().DocumentoURL
                };
        }

        public IEnumerable<ContabilidadeItemViewModel> GetItems()
        {
            return ContabilidadeItems;
        }

        public ContabilidadeItemViewModel GetItem()
        {
            return ContabilidadeItem;
        }

        public void SetItems(IEnumerable<ContabilidadeItemViewModel> value)
        {
            ContabilidadeItems = value;
        }

        public void SetItem(ContabilidadeItemViewModel value)
        {
            ContabilidadeItem = value;
        }
        #endregion
    }
}