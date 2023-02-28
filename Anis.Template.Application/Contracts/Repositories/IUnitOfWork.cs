using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anis.Template.Application.Contracts.Repositories
{
    public interface IUnitOfWork
    {
        public interface IUnitOfWork : IDisposable
        {
            ICardRepository Cards { get; }
            Task SaveChangesAsync();
        }
    }
}
