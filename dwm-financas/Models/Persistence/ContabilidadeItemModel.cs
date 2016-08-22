using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using DWM.Models.Repositories;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using System.Web;

namespace DWM.Models.Persistence
{
    public class ContabilidadeItemModel : CrudItem<ContabilidadeItemViewModel, ApplicationContext>
    {
        public ContabilidadeItemModel()
            : base()
        {

        }

        public ContabilidadeItemModel(IList<ContabilidadeItemViewModel> list)
            : base(list)
        {

        }


        #region Métodos da classe CrudItem
        public override ContabilidadeItemViewModel Find(ContabilidadeItemViewModel key)
        {
            return this.ListAll().Where(info => info.sequencial == key.sequencial).First();
        }

        public override int Indexof(ContabilidadeItemViewModel key)
        {
            return this.ListAll().ToList().FindIndex(info => info.sequencial == key.sequencial);
        }

        public override ContabilidadeItemViewModel CreateRepository(HttpRequestBase Request = null)
        {
            ContabilidadeItemViewModel r = base.CreateRepository();

            return SetKey(r);
        }

        public override ContabilidadeItemViewModel SetKey(ContabilidadeItemViewModel r)
        {
            r.sequencial = this.ListAll().Count() > 0 ? this.ListAll().Last().sequencial + 1 : 1;

            return r;
        }

        public override Validate Validate(ContabilidadeItemViewModel value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = "Item incluído com sucesso", MessageType = MsgType.SUCCESS };

            if (operation != Crud.EXCLUIR)
            {
                if (value.planoContaId == 0)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Conta Contábil").ToString();
                    value.mensagem.MessageBase = "Conta contábil deve ser informada";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.historicoId == 0)
                {
                    value.mensagem.Code = 5;
                    value.mensagem.Message = MensagemPadrao.Message(5, "Histórico").ToString();
                    value.mensagem.MessageBase = "Histórico deve ser informado";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                if (value.valor < 0)
                {
                    value.mensagem.Code = 47;
                    value.mensagem.Message = MensagemPadrao.Message(47, "Valor").ToString();
                    value.mensagem.MessageBase = "Valor inválido";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                // Verifica se a conta contábil é do tipo "Analítica"
                PlanoContaViewModel pc = new PlanoContaModel().getObject(new PlanoContaViewModel() { planoContaId = value.planoContaId });
                if (pc.tipoConta.Equals("S"))
                {
                    value.mensagem.Code = 21;
                    value.mensagem.Message = MensagemPadrao.Message(21).ToString();
                    value.mensagem.MessageBase = "Somente contas do tipo [Analítica] podem ser adicionadas à lista";
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
                ContabilidadeItemViewModel item = this.ListAll().ToList()[i];
                item.sequencial = i + 1;
                this.ListAll().ToList()[i] = item;
            }
        }
        #endregion
    }
}