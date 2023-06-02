using Entities.Entidades;
using Infra.Migrations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infra.Configuracao
{
    public class ContextBase : IdentityDbContext<ApplicationUser>
    {
        public ContextBase(DbContextOptions options) : base(options) { }

        // Vão ser as tabelas que o Entity vai Gerenciar no BD
        public DbSet<SistemaFinanceiro> SistemaFinanceiro { set; get; } 
        public DbSet<UsuarioSistemaFinanceiro> UsuarioSistemaFinanceiro { set; get; }
        public DbSet<Categoria> Categoria { set; get; }
        public DbSet<Despesa> Despesa { set; get; }


        // Vai verificar se a URL está configurada direcionada para o BD
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ObterStringConexao()); // Caso não possua ele vai obter a string de conexão
                base.OnConfiguring(optionsBuilder); // E configurar aqui
            }
        }

        // Vai mapear qual é o Id da tabela AspNetUsers para a tabela ApplicationUser
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().ToTable("AspNetUsers").HasKey(t => t.Id);

            base.OnModelCreating(builder);
        }

        // Config de conexão com BD
        public string ObterStringConexao()
        {
           return "Data Source=DESKTOP-L8175GI\\SQLEXPRESS;Initial Catalog=Financeiro_2023;Integrated Security=False;User ID=sa;Password=123456;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
        }
    }
}

