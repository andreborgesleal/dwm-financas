using App_Dominio.Entidades;
using App_Dominio.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DWM.Models.Entidades
{
    public static class DWMSessaoLocal
    {
        public static SessaoLocal GetSessaoLocal(Sessao sessaoCorrente, DbContext _db)
        {
            ApplicationContext db = (ApplicationContext)_db;
            SessaoLocal SessaoLocal = new SessaoLocal()
            {
                dt_atualizacao = sessaoCorrente.dt_atualizacao,
                dt_criacao = sessaoCorrente.dt_criacao,
                dt_desativacao = sessaoCorrente.dt_desativacao,
                empresaId = sessaoCorrente.empresaId,
                ip = sessaoCorrente.ip,
                isOnline = sessaoCorrente.isOnline,
                login = sessaoCorrente.login,
                sessaoId = sessaoCorrente.sessaoId,
                sistemaId = sessaoCorrente.sistemaId,
                usuarioId = sessaoCorrente.usuarioId,
                value1 = sessaoCorrente.value1,
                value2 = sessaoCorrente.value2,
                value3 = sessaoCorrente.value3,
                value4 = sessaoCorrente.value4,
            };

            return SessaoLocal;
        }

        /// <summary>
        /// Abre a conexão com o banco de dados e retorna a SessaoLocal
        /// </summary>
        /// <param name="Token"></param>
        /// <returns>Retorna Null se a sessão estiver expirada</returns>
        public static SessaoLocal GetSessaoLocal(string Token = null)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();
                Sessao sessaoCorrente = security.getSessaoCorrente(Token);
                SessaoLocal SessaoLocal = null; // se a sessão estiver expirada retorna null
                if (sessaoCorrente == null)
                    return SessaoLocal;
                else
                    return GetSessaoLocal(sessaoCorrente, db);
            }
        }

    }
}