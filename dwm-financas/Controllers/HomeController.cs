using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Controllers;
using App_Dominio.Entidades;
using App_Dominio.Models;
using App_Dominio.Pattern;
using App_Dominio.Repositories;
using App_Dominio.Security;
using DWM.Models.BI;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Report;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace DWM.Controllers
{
    public class HomeController : SuperController
    {
        private int _avatarWidth = 100; // ToDo - Change the size of the stored avatar image
        private int _avatarHeight = 100; // ToDo - Change the size of the stored avatar image


        #region Inheritance
        public override int _sistema_id() { return (int)DWM.Models.Enumeracoes.Sistema.DWMFINANCAS; }

        public override string getListName()
        {
            return "Detalhar";
        }

        public override ActionResult List(int? index, int? PageSize, string descricao = null)
        {
            throw new NotImplementedException();
        }
        #endregion

        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeFilter]
        public ActionResult Default()
        {
            if (ViewBag.ValidateRequest)
            {
                Factory<HomeViewModel, ApplicationContext> factory = new Factory<HomeViewModel, ApplicationContext>();
                return View(factory.Execute(new HomeBI(), new HomeViewModel()));
            }
            else
                return View();
        }

        public ActionResult ListParam(int? index, int? pageSize = 20)
        {
            //ListViewContaReceber listCob = new ListViewContaReceber();
            //Facade<ContaReceberViewModel, ContaReceberModel, ApplicationContext> facadeCob = new Facade<ContaReceberViewModel, ContaReceberModel, ApplicationContext>();

            ListViewContaReceberDemonstrativoBI listCob = new ListViewContaReceberDemonstrativoBI();
            Factory<ContaReceberDemonstrativoViewModel, ApplicationContext> facadeCob = new Factory<ContaReceberDemonstrativoViewModel, ApplicationContext>();

            IPagedList pagedList = facadeCob.PagedList(listCob, index, pageSize.Value,
                                            true,
                                            new DateTime(Funcoes.Brasilia().Year, Funcoes.Brasilia().AddMonths(-2).Month, 1),
                                            Funcoes.Brasilia().Date,
                                            true,
                                            Funcoes.Brasilia().AddDays(1),
                                            new DateTime(Funcoes.Brasilia().Year, Funcoes.Brasilia().AddMonths(1).Month, 1).AddDays(-1),
                                            true,
                                            true,
                                            true,
                                            false,
                                            new DateTime(Funcoes.Brasilia().Year, Funcoes.Brasilia().Month, 1),
                                            Funcoes.Brasilia().AddDays(1),
                                            null,
                                            null,
                                            null,
                                            null,
                                            null,
                                            null,
                                            "");
            return View("ListPanorama", pagedList);
        }


        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult _Error()
        {
            return View();
        }

        #region Alerta - segurança
        public ActionResult ReadAlert(int? alertaId)
        {
            try
            {
                EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();
                if (alertaId.HasValue && alertaId > 0)
                    security.ReadAlert(alertaId.Value);
            }
            catch
            {
                return null;
            }

            return null;
        }
        #endregion

        #region Formulário Modal
        #region Formulário Modal Genérico
        public ActionResult LOVModal(IPagedList pagedList)
        {
            return View(pagedList);
        }
        #endregion

        #region Formulário Modal Plano de Contas
        #region Formulário Modal Plano de Contas
        [AuthorizeFilter]
        public ActionResult LovPlanoContaModal(int? index, int? pageSize = 50)
        {
            if (ViewBag.ValidateRequest)
                return this.ListModal(index, pageSize, new LookupPlanoContaModel(), "Plano de Contas", null, DWM.Models.Enumeracoes.Sistema.DWMFINANCAS);
            else
                return View();
        }
        #endregion

        #region Formulário Modal Plano de Contas Pai
        [AuthorizeFilter]
        public ActionResult LovPlanoContasPaiModal(int? index, int? pageSize = 50)
        {
            if (ViewBag.ValidateRequest)
                return this.ListModal(index, pageSize, new LookupPlanoContaPaiModel(), "Descrição", null, DWM.Models.Enumeracoes.Sistema.DWMFINANCAS);
            else
                return View();
        }
        #endregion
        #endregion

        #region Formulário Modal Históricos

        #region Histórico contabilidade
        [AuthorizeFilter]
        public ActionResult LovHistoricoContabilidadeModal(int? index, int? pageSize = 50)
        {
            if (ViewBag.ValidateRequest)
                return this.ListModal(index, pageSize, new LookupHistoricoContabilidadeModel(), "Descrição", null, "C");
            else
                return View();
        }
        #endregion

        #region Histórico Contas a Pagar
        [AuthorizeFilter]
        public ActionResult LovHistoricoPagamentoModal(int? index, int? pageSize = 50)
        {
            if (ViewBag.ValidateRequest)
                return this.ListModal(index, pageSize, new LookupHistoricoContaPagarModel(), "Descrição", null, "P");
            else
                return View();
        }
        #endregion

        #region Histórico Contas a Receber
        [AuthorizeFilter]
        public ActionResult LovHistoricoCobrancaModal(int? index, int? pageSize = 50)
        {
            if (ViewBag.ValidateRequest)
                return this.ListModal(index, pageSize, new LookupHistoricoContaReceberModel(), "Descrição", null, "R");
            else
                return View();
        }
        #endregion
        #endregion

        #region Formulário Modal Centro de Custos

        [AuthorizeFilter]
        public ActionResult LovCentroCustoModal(int? index, int? pageSize = 50)
        {
            if (ViewBag.ValidateRequest)
                return this.ListModal(index, pageSize, new LookupCentroCustoModel(), "Descrição");
            else
                return View();
        }

        #endregion

        #region Formulário Modal Enquadramentos

        [AuthorizeFilter]
        public ActionResult LovEnquadramentoModal(int? index, int? pageSize = 50)
        {
            if (ViewBag.ValidateRequest)
            {
                Facade<EnquadramentoViewModel, EnquadramentoModel, ApplicationContext> facade = new Facade<EnquadramentoViewModel, EnquadramentoModel, ApplicationContext>();
                IPagedList pagedList = facade.getPagedList((ListViewModel<EnquadramentoViewModel, ApplicationContext>)new LookupEnquadramentoModel(), index, pageSize.Value);
                return View("LOVModal", pagedList);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult LovEnquadramentoOperacaoModal(int? index, int? pageSize = 50)
        {
            // Este método foi criado para diferenciar o EnquadramentoModal do Contas a pagar e receber do Enquandramento Modal usado na inclusão da contabilidade
            // Na Contabilidade quando o usuário seleciona o enquadramento desejado, o sistema recupera todos os itens do respectivo enquadramento e MOVE para os itens da contabilidade
            // No contas a pagar e receber, o enquadramento será usado exclusivamente para o TYPEAHEAD e quando o usuário selecionar o formulário modal e mandar MOVER (javascript) 
            // a linha selecionada, o sistema irá apenas mover apenas a descrição do enquadramento para o textbox do typeahead
            if (ViewBag.ValidateRequest)
            {
                Facade<EnquadramentoViewModel, EnquadramentoModel, ApplicationContext> facade = new Facade<EnquadramentoViewModel, EnquadramentoModel, ApplicationContext>();
                IPagedList pagedList = facade.getPagedList((ListViewModel<EnquadramentoViewModel, ApplicationContext>)new LookupEnquadramentoOperacaoModel(), index, pageSize.Value);
                return View("LOVModal", pagedList);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult LovEnquadramentoOperacaoAmortizacaoModal(int? index, int? pageSize = 50)
        {
            // Este método foi criado para diferenciar o EnquadramentoModal do Contas a pagar e receber do Enquandramento Modal usado na inclusão da contabilidade
            // Na Contabilidade quando o usuário seleciona o enquadramento desejado, o sistema recupera todos os itens do respectivo enquadramento e MOVE para os itens da contabilidade
            // No contas a pagar e receber, o enquadramento será usado exclusivamente para o TYPEAHEAD e quando o usuário selecionar o formulário modal e mandar MOVER (javascript) 
            // a linha selecionada, o sistema irá apenas mover apenas a descrição do enquadramento para o textbox do typeahead de AMORTIZAÇÃO
            if (ViewBag.ValidateRequest)
            {
                Facade<EnquadramentoViewModel, EnquadramentoModel, ApplicationContext> facade = new Facade<EnquadramentoViewModel, EnquadramentoModel, ApplicationContext>();
                IPagedList pagedList = facade.getPagedList((ListViewModel<EnquadramentoViewModel, ApplicationContext>)new LookupEnquadramentoOperacaoAmortizacaoModel(), index, pageSize.Value);
                return View("LOVModal", pagedList);
            }
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult LovRazaoModal(int? index, int? pageSize = 50)
        {
            string data1 = DateTime.Today.ToString("yyyy-MM-") + "01";
            string data2 = DateTime.Today.ToString("yyyy-MM-dd");

            return this.ListLovModal(index, pageSize, new LookupRazaoModel(), "Descrição", data1, data2, null, 1, "N", "N");
            //if (ViewBag.ValidateRequest)
            //    return this.ListModal(index, pageSize, new LookupRazaoModel(), "Descrição");
            //else
            //    return View();
        }


        #endregion

        #region Formulário Modal Grupo de Credores (fornecedores)

        [AuthorizeFilter]
        public ActionResult LovGrupoCredorModal(int? index, int? pageSize = 50)
        {
            if (ViewBag.ValidateRequest)
                return this.ListModal(index, pageSize, new LookupGrupoCredorModel(), "Descrição");
            else
                return View();
        }

        #endregion

        #region Formulário Modal Credores (fornecedores)

        [AuthorizeFilter]
        public ActionResult LovCredorModal(int? index, int? pageSize = 50)
        {
            if (ViewBag.ValidateRequest)
                return this.ListModal(index, pageSize, new LookupCredorModel(), "Descrição");
            else
                return View();
        }

        #endregion

        #region Formulário Modal Grupo de Clientes

        [AuthorizeFilter]
        public ActionResult LovGrupoClienteModal(int? index, int? pageSize = 50)
        {
            if (ViewBag.ValidateRequest)
                return this.ListModal(index, pageSize, new LookupGrupoClienteModel(), "Descrição");
            else
                return View();
        }

        #endregion

        #region Formulário Modal Clientes 

        [AuthorizeFilter]
        public ActionResult LovClienteModal(int? index, int? pageSize = 50)
        {
            if (ViewBag.ValidateRequest)
                return this.ListModal(index, pageSize, new LookupClienteModel(), "Descrição");
            else
                return View();
        }

        #endregion

        #region Formulário Modal Bancos

        [AuthorizeFilter]
        public ActionResult LovBancoModal(int? index, int? pageSize = 50)
        {
            if (ViewBag.ValidateRequest)
                return this.ListModal(index, pageSize, new LookupBancoModel(), "Descrição");
            else
                return View();
        }

        #endregion

        #region Formulário Modal Bancos2

        [AuthorizeFilter]
        public ActionResult LovBanco2Modal(int? index, int? pageSize = 50)
        {
            if (ViewBag.ValidateRequest)
                return this.ListModal(index, pageSize, new LookupBanco2Model(), "Descrição");
            else
                return View();
        }

        #endregion

        #region Formulário Modal Bancos3

        [AuthorizeFilter]
        public ActionResult LovBanco3Modal(int? index, int? pageSize = 50)
        {
            if (ViewBag.ValidateRequest)
                return this.ListModal(index, pageSize, new LookupBanco3Model(), "Descrição");
            else
                return View();
        }

        #endregion

        #region Formulário Modal Eventos

        [AuthorizeFilter]
        public ActionResult LovEventoModal(int? index, int? pageSize = 50)
        {
            if (ViewBag.ValidateRequest)
                return this.ListModal(index, pageSize, new LookupEventoModel(), "Descrição");
            else
                return View();
        }

        #endregion
        #endregion


        #region Avatar
        [HttpGet]
        [AuthorizeFilter]
        public ActionResult Avatar()
        {
            if (ViewBag.ValidateRequest)
            {
                EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();
                UsuarioRepository u = security.getUsuarioRepositoryById();

                UsuarioViewModel r = new UsuarioViewModel()
                {
                    usuarioId = u.usuarioId,
                    empresaId = u.empresaId,
                    dt_cadastro = u.dt_cadastro,
                    Grupos = u.Grupos,
                    isAdmin = u.isAdmin,
                    login = u.login,
                    nome = u.nome,
                    nome_sistema = u.nome_sistema,
                    situacao = u.situacao
                };

                return View(r);
            }
            return View();
        }

        [ValidateAntiForgeryToken]
        [AuthorizeFilter]
        public ActionResult Avatar(IEnumerable<HttpPostedFileBase> files)
        {
            if (ViewBag.ValidateRequest)
            {
                string errorMessage = "";

                if (files != null && files.Count() > 0)
                {
                    // Get one only
                    var file = files.FirstOrDefault();
                    // Check if the file is an image
                    if (file != null && IsImage(file))
                    {
                        // Verify that the user selected a file
                        if (file != null && file.ContentLength > 0)
                        {
                            var webPath = SaveTemporaryFile(file);
                            return Json(new { success = true, fileName = webPath.Replace("/", "\\") }); // success
                        }
                        errorMessage = "Arquivo não pode ser de tamnho nulo."; //failure
                    }
                    errorMessage = "Formato de arquivo inválido."; //failure
                }
                errorMessage = "Nenhum arquivo foi enviado."; //failure

                return Json(new { success = false, errorMessage = errorMessage });
            }
            return View();
        }

        [HttpPost]
        public ActionResult Save(string t, string l, string h, string w, string fileName, string key)
        {
            try
            {
                // Get file from temporary folder
                var fn = Path.Combine(Server.MapPath("~/Temp"), Path.GetFileName(fileName));

                // Calculate dimesnions
                int top = Convert.ToInt32(t.Replace("-", "").Replace("px", ""));
                int left = Convert.ToInt32(l.Replace("-", "").Replace("px", ""));
                int height = Convert.ToInt32(h.Replace("-", "").Replace("px", ""));
                int width = Convert.ToInt32(w.Replace("-", "").Replace("px", ""));

                // Get image and resize it, ...
                var img = new WebImage(fn);
                img.Resize(width, height);
                // ... crop the part the user selected, ...
                img.Crop(top, left, img.Height - top - _avatarHeight, img.Width - left - _avatarWidth);
                // ... delete the temporary file,...
                System.IO.File.Delete(fn);
                // ... and save the new one.

                string oldName = new UsuarioViewModel().Avatar();
                if (oldName.Substring(0, 4) != "http")
                    System.IO.File.Delete(Server.MapPath(oldName));

                string newName = String.Format("{0}" + new FileInfo(fn).Extension, key);
                //AssociadoViewModel value = (AssociadoViewModel)getModel().getObject(new AssociadoViewModel() { associadoId = int.Parse(key) });
                //if (value.avatar != null)
                //    newName = value.avatar;

                //string newFileName = System.Configuration.ConfigurationManager.AppSettings["Avatar"] + "/" + newName; // Path.GetFileName(fn);
                string newFileLocation = HttpContext.Server.MapPath(new UsuarioViewModel().Path());
                if (Directory.Exists(Path.GetDirectoryName(newFileLocation)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newFileLocation));
                }

                img.FileName = Path.Combine(newFileLocation, Path.GetFileName(newName));  //newName;
                img.Save(img.FileName);
                return Json(new { success = true, avatarFileLocation = newFileLocation });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Não foi possível fazer o upload do arquivo.\nERRORINFO: " + ex.Message });
            }
        }

        private bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            var extensions = new string[] { ".jpg", ".png", ".gif", ".jpeg" }; // add more if you like...

            // linq from Henrik Stenbæk
            return extensions.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        private string SaveTemporaryFile(HttpPostedFileBase file)
        {
            // Define destination
            var folderName = "/Temp";
            var serverPath = HttpContext.Server.MapPath(folderName);
            if (Directory.Exists(serverPath) == false)
            {
                Directory.CreateDirectory(serverPath);
            }

            // Generate unique file name
            var fileName = Path.GetFileName(file.FileName);
            fileName = SaveTemporaryAvatarFileImage(file, serverPath, fileName);

            // Clean up old files after every save
            CleanUpTempFolder(1);

            return Path.Combine(folderName, fileName);
        }

        private string SaveTemporaryAvatarFileImage(HttpPostedFileBase file, string serverPath, string fileName)
        {
            var img = new WebImage(file.InputStream);
            double ratio = (double)img.Height / (double)img.Width;

            string fullFileName = Path.Combine(serverPath, fileName);

            img.Resize(400, (int)(400 * ratio)); // ToDo - Change the value of the width of the image oin the screen

            if (System.IO.File.Exists(fullFileName))
                System.IO.File.Delete(fullFileName);

            img.Save(fullFileName);

            return Path.GetFileName(img.FileName);
        }

        private void CleanUpTempFolder(int hoursOld)
        {
            try
            {
                DateTime fileCreationTime;
                DateTime currentUtcNow = DateTime.UtcNow;

                var serverPath = HttpContext.Server.MapPath("/Temp");
                if (Directory.Exists(serverPath))
                {
                    string[] fileEntries = Directory.GetFiles(serverPath);
                    foreach (var fileEntry in fileEntries)
                    {
                        fileCreationTime = System.IO.File.GetCreationTimeUtc(fileEntry);
                        var res = currentUtcNow - fileCreationTime;
                        if (res.TotalHours > hoursOld)
                        {
                            System.IO.File.Delete(fileEntry);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        #endregion
    }
}