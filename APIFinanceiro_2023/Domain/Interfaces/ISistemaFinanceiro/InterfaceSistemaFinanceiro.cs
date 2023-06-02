using Domain.Interfaces.Generics;
using Entities.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ISistemaFinanceiro
{
    public interface InterfaceSistemaFinanceiro : InterfaceGenerics<SistemaFinanceiro>
    {
        Task<IList<SistemaFinanceiro>> ListaSistemasUsuario(string emailUsuario);
    }
}
