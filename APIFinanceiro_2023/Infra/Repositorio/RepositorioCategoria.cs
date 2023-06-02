using Domain.Interfaces.ICategoria;
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
    public class RepositorioCategoria : RepositoryGenerics<Categoria>, InterfaceCategoria
    {
        private readonly DbContextOptions<ContextBase> _OptionsBuilder; // Padrão

        public RepositorioCategoria()
        {
            _OptionsBuilder  = new DbContextOptions<ContextBase>(); 
        }

        public async Task<IList<Categoria>> ListarCategoriasUsuario(string emailUsuario)
        {
            using (var banco = new ContextBase(_OptionsBuilder))
            {
                // Trazendo todas as categorias que o usuário pode criar no Sistema Financeiro (EX: Casa, Trabalho...)
                return await
                    (from s in banco.SistemaFinanceiro // o sistema financeiro
                     join c in banco.Categoria on s.Id equals c.IdSistema // E esse sistema financeiro tem varias categorias
                     join us in banco.UsuarioSistemaFinanceiro on s.Id equals us.IdSistema // Adiciona usuários para o sistema financeiro
                     where us.EmailUsuario.Equals(emailUsuario) && us.SistemaAtual
                     select c).AsNoTracking().ToListAsync();
            }
        }
    }
}
