using App_Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DWM.Models.Entidades
{
    public class ApplicationContext : App_DominioContext
    {
        public DbSet<Parametro> Parametros { get; set; }
        public DbSet<GrupoCredor> GrupoCredores { get; set; }
        public DbSet<Credor> Credores { get; set; }
        public DbSet<CentroCusto> CentroCustos { get; set; }
        public DbSet<Historico> Historicos { get; set; }
        public DbSet<GrupoCobranca> GrupoCobrancas { get; set;  }
        public DbSet<PlanoConta> PlanoContas { get; set; }
        public DbSet<Contabilidade> Contabilidades { get; set; }
        public DbSet<ContabilidadeItem> ContabilidadeItems { get; set; }
        public DbSet<Exercicio> Exercicios { get; set; }
        public DbSet<Enquadramento> Enquadramentos { get; set; }
        public DbSet<EnquadramentoItem> EnquadramentoItems { get; set; }
        public DbSet<MovtoBancario> MovtoBancarios { get; set; }
        public DbSet<Banco> Bancos { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<ContaPagarParcelaEvento> ContaPagarParcelaEventos { get; set; }
        public DbSet<ContaPagarParcela> ContaPagarParcelas { get; set; }
        public DbSet<ContaPagar> ContaPagars { get; set; }
        public DbSet<GrupoCliente> GrupoClientes { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Cidade> Cidades { get; set; }
        public DbSet<Feriado> Feriados { get; set; }
        public DbSet<EmpresaInterna> EmpresaInternas { get; set; }
        public DbSet<ContaReceber> ContaRecebers { get; set; }
        public DbSet<ContaReceberParcela> ContaReceberParcelas { get; set; }
        public DbSet<ContaReceberParcelaEvento> ContaReceberParcelaEventos { get; set; }
        public DbSet<Cobranca> Cobrancas { get; set; }
        public DbSet<CobrancaCliente> CobrancaClientes { get; set; }
    }
}
