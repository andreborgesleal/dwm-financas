using System;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using DWM.Models.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Security;
using App_Dominio.Repositories;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using App_Dominio.Models;


namespace DWM.Models.Persistence
{
    public class AccountModel : ProcessContext<Medico, RegisterViewModel, ApplicationContext>
    {
        private ChamadoViewModel chamadoViewModel { get; set; }

        #region Métodos da classe CrudContext
        public override Medico ExecProcess(RegisterViewModel value, Crud operation)
        {
            EmpresaSecurity<SecurityContext> empresaSecurity = new EmpresaSecurity<SecurityContext>();

            int _empresaId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["empresaId"]);
            int _sistemaId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["sistemaId"]);
            int _areaAtendimentoId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["secretariaId"]);

            #region Incluir o usuário
            UsuarioRepository usuarioRepository = new UsuarioRepository()
            {
                login = value.login.ToLower(),
                nome = value.nome.ToUpper(),
                empresaId = _empresaId,
                dt_cadastro = DateTime.Now,
                situacao = "D",
                isAdmin = "N",
                senha = value.senha,
                uri = value.uri,
                confirmacaoSenha = value.confirmacaoSenha
            };

            usuarioRepository = empresaSecurity.SetUsuario(usuarioRepository);
            if (usuarioRepository.mensagem.Code > 0)
                throw new ArgumentException(usuarioRepository.mensagem.Message);
            #endregion

            #region Vincular médico ao usuário
            value.cpf = value.cpf.Replace(".", "").Replace("-", "");
            value.ufCRM = value.ufCRM.ToUpper();
            value.login = value.login.ToLower();
            MedicoViewModel medicoViewModel = (from med in db.Medicos
                                               where (med.CRM == value.CRM && med.ufCRM == value.ufCRM) ||
                                                      med.cpf == value.cpf ||
                                                      med.email1 == value.login
                                               select new MedicoViewModel()
                                               {
                                                   associadoId = med.associadoId,
                                                   CRM = med.CRM,
                                                   ufCRM = med.ufCRM,
                                                   cpf = med.cpf,
                                                   nome = med.nome
                                               }).FirstOrDefault();

            value.associadoId = medicoViewModel.associadoId;

            Medico medico = Find(value);

            medico.CRM = value.CRM;
            medico.ufCRM = value.ufCRM;
            medico.cpf = value.cpf;
            medico.usuarioId = usuarioRepository.usuarioId;
            medico.email1 = value.login;

            db.Entry(medico).State = EntityState.Modified;
            #endregion

            #region Insere o chamado para a secretaria
            chamadoViewModel = new ChamadoViewModel()
            {
                associadoId = value.associadoId.Value,
                areaAtendimentoId = _areaAtendimentoId,
                dt_chamado = DateTime.Now,
                assunto = "Solicitação de ativação do usuário " + (usuarioRepository.nome.Length >= 15 ? usuarioRepository.nome.Substring(0, 15) : usuarioRepository.nome.Substring(0, usuarioRepository.nome.Length)),
                situacao = "A"
            };
            chamadoViewModel.mensagemOriginal = "<h4>Liberação de acesso ao sistema para um novo usuário</h4>";
            chamadoViewModel.mensagemOriginal += "<hr>";
            chamadoViewModel.mensagemOriginal += "<p><b>Nome do Usuário: </b>" + value.nome + "</p>";
            chamadoViewModel.mensagemOriginal += "<p><b>Nome Associado: </b>" + medicoViewModel.nome + "</p>";
            chamadoViewModel.mensagemOriginal += "<p><b>Login: </b>" + value.login + "</p>";
            chamadoViewModel.mensagemOriginal += "<p><b>CRM: </b>" + value.CRM + "</p>";
            chamadoViewModel.mensagemOriginal += "<p><b>UF CRM: </b>" + value.ufCRM + "</p>";
            chamadoViewModel.mensagemOriginal += "<p><b>CPF: </b>" + value.cpf + "</p>";
            chamadoViewModel.mensagemOriginal += "<hr>";
            chamadoViewModel.mensagemOriginal += "<p><a href=\"../Associado/Edit?associadoId=" + value.associadoId.ToString() + "\">Clique aqui</a> para acessar o cadastro do respectivo associado</p>";
            chamadoViewModel.uri = value.uri;

            ChamadoModel chamadoModel = new ChamadoModel();
            chamadoModel.db = db;
            Chamado chamado = chamadoModel.ExecProcess(chamadoViewModel, Crud.INCLUIR);
            #endregion

            return medico;
        }

        public override Validate AfterInsert(RegisterViewModel value)
        {
            EmpresaSecurity<SecurityContext> empresaSecurity = new EmpresaSecurity<SecurityContext>();

            int _sistemaId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["sistemaId"]);
            int _areaAtendimentoId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["secretariaId"]);

            // Inserir os alertas para os usuários da secretaria
            int[] _usuarioId = (from aa in db.AreaAtendimentos
                                where aa.areaAtendimentoId == _areaAtendimentoId
                                select aa.usuario1Id).ToArray();

            // obtêm o chamadoId
            int? _chamadoId = (from cham in db.Chamados.AsEnumerable()
                               where cham.associadoId == value.associadoId.Value
                               select cham.chamadoId).LastOrDefault();

            for (int i = 0; i <= _usuarioId.Count() - 1; i++)
            {
                AlertaRepository alerta = new AlertaRepository()
                {
                    usuarioId = _usuarioId[i],
                    sistemaId = _sistemaId,
                    dt_emissao = DateTime.Now,
                    linkText = "<span class=\"label label-warning\">Novo Usuário</span>",
                    url = "../Atendimento/Create?chamadoId=" + _chamadoId.ToString() + "&fluxo=2",
                    mensagemAlerta = "<b>" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "h</b><p>" + chamadoViewModel.assunto + "</p>"
                };

                alerta.uri = value.uri;

                AlertaRepository r = empresaSecurity.InsertAlerta(alerta);
                if (r.mensagem.Code > 0)
                    throw new DbUpdateException(r.mensagem.Message);
            }

            return new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };
        }

        public override Medico MapToEntity(RegisterViewModel value)
        {
            return new Medico()
            {
                associadoId = value.associadoId.Value
            };
        }

        public override RegisterViewModel MapToRepository(Medico entity)
        {
            return new RegisterViewModel()
            {
                associadoId = entity.associadoId,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Medico Find(RegisterViewModel key)
        {
            return db.Medicos.Find(key.associadoId);
        }

        public override Validate Validate(RegisterViewModel value, Crud operation)
        {
            EmpresaSecurity<SecurityContext> empresaSecurity = new EmpresaSecurity<SecurityContext>();

            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };

            #region Valida CPF
            if (!Funcoes.ValidaCpf(value.cpf.Replace(".", "").Replace("-", "")))
            {
                value.mensagem.Code = 29;
                value.mensagem.Message = MensagemPadrao.Message(29, value.login).ToString();
                value.mensagem.MessageBase = "CPF informado está incorreto.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            #endregion

            #region Verifica se o login já existe
            int empresaId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["empresaId"]);
            UsuarioRepository usu = empresaSecurity.getUsuarioByLogin(value.login, empresaId);
            if (usu != null)
            {
                value.mensagem.Code = 41;
                value.mensagem.Message = MensagemPadrao.Message(41, value.login).ToString();
                value.mensagem.MessageBase = "Login já exsitente nos registros da empresa.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            #endregion

            #region Verifica se existe o CPF ou CRM e caso exista, verifica se o usuário já está vinculado
            value.cpf = value.cpf.Replace(".", "").Replace("-", "");
            MedicoViewModel medico = (from med in db.Medicos
                                      where (med.CRM == value.CRM && med.ufCRM == value.ufCRM) ||
                                            med.cpf == value.cpf ||
                                            med.email1 == value.login
                                      select new MedicoViewModel()
                                      {
                                          usuarioId = med.usuarioId,
                                          associadoId = med.associadoId,
                                          CRM = med.CRM,
                                          ufCRM = med.ufCRM,
                                          cpf = med.cpf,
                                          nome = med.nome
                                      }).FirstOrDefault();

            if (medico != null)
            {
                if (medico.usuarioId.HasValue)
                {
                    value.mensagem.Code = 19;
                    value.mensagem.Message = MensagemPadrao.Message(19).ToString();
                    value.mensagem.MessageBase = "Este CRM/CPF já está vinculado a um usuário.";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }

                // Verifica se o CPF e CRM informados estão iguais ao existente no banco de dados
                if ((medico.CRM != null && medico.CRM != "" && medico.CRM != value.CRM) ||
                    (medico.ufCRM != null && medico.ufCRM != "" && medico.ufCRM != value.ufCRM.ToUpper()) ||
                    (medico.cpf != null && medico.cpf != "" && medico.cpf != value.cpf.Replace(".", "").Replace("-", "")) ||
                    (medico.email1 != null && medico.email1 != "" && medico.email1 != value.login))
                {
                    value.mensagem.Code = 50;
                    value.mensagem.Message = MensagemPadrao.Message(50).ToString();
                    value.mensagem.MessageBase = "Dados do associado não conferem com os valores existentes nos registros da empresa.";
                    value.mensagem.MessageType = MsgType.WARNING;
                    return value.mensagem;
                }
            }
            else
            {
                value.mensagem.Code = 20;
                value.mensagem.Message = MensagemPadrao.Message(20).ToString();
                value.mensagem.MessageBase = "Dados de Identificação não encontrado.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            #endregion

            return value.mensagem;
        }
        #endregion
    }
}