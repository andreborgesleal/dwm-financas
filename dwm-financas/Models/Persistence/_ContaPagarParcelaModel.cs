using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using System.Web;
using System;
using App_Dominio.Component;
using App_Dominio.Models;
using System.Web.Mvc;

namespace DWM.Models.Persistence
{
    public class _ContaPagarParcelaModel : CrudItem<ContaPagarParcelaViewModel, ApplicationContext>
    {
        public _ContaPagarParcelaModel()
            : base()
        {

        }

        public _ContaPagarParcelaModel(IList<ContaPagarParcelaViewModel> list)
            : base(list)
        {

        }


        #region Métodos da classe CrudItem
        public override ContaPagarParcelaViewModel Find(ContaPagarParcelaViewModel key)
        {
            return this.ListAll().Where(info => info.parcelaId == key.parcelaId).First();
        }

        public override int Indexof(ContaPagarParcelaViewModel key)
        {
            return this.ListAll().ToList().FindIndex(info => info.parcelaId == key.parcelaId);
        }

        public override ContaPagarParcelaViewModel CreateRepository(HttpRequestBase Request = null)
        {
            ContaPagarParcelaViewModel r = base.CreateRepository();

            return SetKey(r);
        }

        public override ContaPagarParcelaViewModel SetKey(ContaPagarParcelaViewModel r)
        {
            r.parcelaId = this.ListAll().Count() > 0 ? this.ListAll().Last().parcelaId + 1 : 1;

            return r;
        }

        public override Validate Validate(ContaPagarParcelaViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = "Item incluído com sucesso", MessageType = MsgType.SUCCESS };

            if (operation != Crud.EXCLUIR)
            {
                if (value.dt_vencimento <= Convert.ToDateTime("1980-01-01"))
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Dt.Vencimento").ToString();
                    value.mensagem.MessageBase = "Campo obrigatório: Dt. Vencimento";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.vr_principal <= 0)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Vr.Principal").ToString();
                    value.mensagem.MessageBase = "Campo obrigatório: Valor Principal";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (!"1|2|3|4|5|6|9".Contains(value.ind_forma_pagamento))
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Vr.Principal").ToString();
                    value.mensagem.MessageBase = "Campo obrigatório: Forma de pagamento";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
            }

            return value.mensagem;
        }

        public override void AfterDelete()
        {
            // Reindexa o sequencial
            for (int i = 0; i <= this.ListAll().Count() - 1; i++)
            {
                ContaPagarParcelaViewModel item = this.ListAll().ToList()[i];
                item.parcelaId = i + 1;
                this.ListAll().ToList()[i] = item;
            }
        }
        #endregion

    }
}

