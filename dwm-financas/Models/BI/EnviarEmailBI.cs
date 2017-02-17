using System;
using System.Collections.Generic;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using DWM.Models.Entidades;
using System.Web.Mvc;
using App_Dominio.Enumeracoes;
using App_Dominio.Security;
using App_Dominio.Models;
using System.Net.Mail;
using DWM.Models.Repositories;
using System.Linq;

namespace DWM.Models.BI
{
    public class EnviarEmailBI : DWMContextLocal, IProcess<UsuarioViewModel, ApplicationContext>
    {
        #region Constructor
        public EnviarEmailBI() { }

        public EnviarEmailBI(ApplicationContext _db, SecurityContext _segurancaDb)
        {
            this.Create(_db, _segurancaDb);
        }

        #endregion

        public string Tag { get; set; }

        public UsuarioViewModel Run(Repository value)
        {
            UsuarioViewModel rec = (UsuarioViewModel)value;
            string habilitaEmail = db.Parametros.Find(rec.empresaId, (int)Enumeracoes.Enumeradores.Param.HABILITA_EMAIL).valor;
            if (habilitaEmail == "S")
            {
                SendEmail sendMail = new SendEmail();

                MailAddress sender = new MailAddress("DWM Sistemas <andre@dwmsistemas.com>");
                List<string> recipients = new List<string>();

                recipients.Add(rec.nome + "<" + rec.login + ">");

                string Subject = "Esqueci minha senha - DWM-Finanças"; 
                string Text = "<p>DWM-Finanças</p>";
                string Html = "<p><span style=\"font-family: Verdana; font-size: larger; color: #656464\">DWM-Finanças - Sistema de Gestão Financeira</span></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: xx-large; color: #0094ff\">" + rec.nome.ToUpper() + "</span></p>" +
                              "<p><span style=\"font-family: Verdana; font-size: small;\">Conforme solicitado essa é uma mensagem de recuperação de sua senha de cadastro no sistema. </span></p>";

                Html += "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Seu Login de acesso é: </span></p>" +
                        "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: xx-large; color: #0094ff\">" + rec.login + "</span></p>" +
                        "<p></p>";

                Html += "<p></p>" +
                        "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Ao acessar o link @esqueci_minha_senha você será orientado a realizar a troca de senha e pode escolher uma de sua preferência. </span></p>" +
                        "<p></p>" +
                        "<p></p>" +
                        "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">- Observação: Este link estará disponível para ativação por 24h.</span></p>" +
                        "<p></p>" +
                        "<p></p>" +
                        "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Importante: esta é uma mensagem automática e não deve ser respondida. Para enviar a sua sugestão ou solicitação para a DWM Sistemas, por favor, envie mensagem em nosso porta http://www.dwmsistemas.com. </span></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Política de Segurança: A DWM Sistemas nunca envia arquivos executáveis ou solicitação de dados pessoais. Para sua segurança, ao receber uma comunicação digital, acesse nosso portal e confirme a autenticidade do comunicado. Além disso, mantenha atualizado o antivírus do seu computador. </span></p>" +
                        "<hr />" +
                        "<p></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Além desses recursos, estaremos implementando outras novidades. Aguarde !</span></p>" +
                        "<p>&nbsp;</p>" +
                        "<p>&nbsp;</p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Atenciosamente,</span></p>" +
                        "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Assistente Virtual DWM Sistemas</span></p>";

                Html = Html.Replace("@esqueci_minha_senha", Tag);

                Validate result = sendMail.Send(sender, recipients, Html, Subject, Text);
                if (result.Code > 0)
                {
                    result.MessageBase = "Solicitação de renovação de senha realizada com sucesso, mas por falhas de comunicação não foi possível enviar seu e-mail de confirmação. Favor entrar em contato com faleconosco@dwmsisteamas.com e solicite seu e-mail de ativação.";
                    throw new App_DominioException(result);
                }

            }
            rec.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };
            return rec; 
        }

        public IEnumerable<UsuarioViewModel> List(params object[] param)
        {
            throw new NotImplementedException();
        }

        public IPagedList PagedList(int? index, int pageSize = 50, params object[] param)
        {
            throw new NotImplementedException();
        }

    }
}