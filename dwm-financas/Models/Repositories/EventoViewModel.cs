using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;
using DWM.Models.Entidades;

namespace DWM.Models.Repositories
{
    public class EventoViewModel : Repository
    {
        [DisplayName("Evento ID")]
        public int eventoId { get; set; }

        [DisplayName("Descrição")]
        [Required(ErrorMessage = "Descrição do evento deve ser informada")]
        [StringLength(50, ErrorMessage = "A descrição deve ter no máximo 50 caracteres")]
        public string descricao { get; set; }

        public int codigo { get; set; }

        [DisplayName("Tipo Evento")]
        public string ind_tipoEvento { get; set; }

        [DisplayName("Evento Fixo")]
        public string ind_eventoFixo { get; set; }

        [DisplayName("Operação")]
        [Required(ErrorMessage = "Operação deve ser informada")]
        public string ind_operacao{ get; set; }

        [DisplayName("Modalidade")]
        [Required(ErrorMessage = "Modalidade deve ser informada")]
        public string ind_modalidade { get; set; }

        public string ind_eventoFixoExtenso
        {
            get
            {
                switch (ind_tipoEvento[0])
                {
                    case '0':
                        return "Inclusão de Operação";
                    case '1':
                        return "Encargos";
                    case '2':
                        return "Desconto";
                    case '3':
                        return "Amortização";
                    case '4':
                        return "Baixa por motivo de liquidação";
                    case '5':
                        return "Baixa por motivo de cancelamento";
                    case '6':
                        return "Baixa por motivo de renegociação";
                    case '7':
                        return "Baixa por motivo de desconto";
                    case '8':
                        return "Alteração de título";
                    case '9':
                        return "Estorno de lançamento";
                    default:
                        return ind_tipoEvento;
                }
            }
        }
    }
}