using Domain.Interfaces.IDespesa;
using Entities.Entidades;
using Infra.Configuracao;
using Infra.Repositorio.Generics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repositorio
{
    public class RepositorioDespesa : RepositoryGenerics<Despesa>, InterfaceDespesa
    {
        private readonly DbContextOptions<ContextBase> _OptionsBuilder; // Padrão

        public RepositorioDespesa()
        {
            _OptionsBuilder = new DbContextOptions<ContextBase>();
        }

        public async Task<IList<Despesa>> ListarDespesasUsuario(string emailUsuario)
        {
            using (var banco = new ContextBase(_OptionsBuilder))
            {
                return await
                   (from s in banco.SistemaFinanceiro // Cria o SistemaFinanceiro
                    join c in banco.Categoria on s.Id equals c.IdSistema // Cria a categoria
                    join us in banco.UsuarioSistemaFinanceiro on s.Id equals us.IdSistema // Atrelando o usuário ao Sistema Financeiro
                    join d in banco.Despesa on c.Id equals d.IdCategoria // Ligando a categoria que esta despesa está
                    where us.EmailUsuario.Equals(emailUsuario) && s.Mes == d.Mes && s.Ano == d.Ano // O mês corrente tem que ser o mesmo do mês da despesas
                    select d).AsNoTracking().ToListAsync();
            }
        }
        public async Task<IList<Despesa>> ListarDespesasUsuarioNaoPagasMesesAnterior(string emailUsuario)
        {
            using (var banco = new ContextBase(_OptionsBuilder))
            {
                return await
                   (from s in banco.SistemaFinanceiro // Cria o SistemaFinanceiro
                    join c in banco.Categoria on s.Id equals c.IdSistema // Cria a categoria
                    join us in banco.UsuarioSistemaFinanceiro on s.Id equals us.IdSistema // Atrelando o usuário ao Sistema Financeiro
                    join d in banco.Despesa on c.Id equals d.IdCategoria // Ligando a categoria que esta despesa está
                    where us.EmailUsuario.Equals(emailUsuario) && d.Mes < DateTime.Now.Month && !d.Pago // listar contas não pagas no mês anterior
                    select d).AsNoTracking().ToListAsync();
            }
        }
    }
}
