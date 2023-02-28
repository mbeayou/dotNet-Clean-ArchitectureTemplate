using Anis.Template.Application.Contracts.Repositories;
using Anis.Template.Infrastructure.Persistence;
using Anis.Template.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anis.Template.Infra.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;
        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        private ICardRepository _cardRepository;
        public ICardRepository Cards
        {
            get => _cardRepository ??= new CardRepository(_appDbContext);
        }


        public void Dispose()
        {
            _appDbContext.Dispose();
        }

        public async Task SaveChangesAsync()
        {
            await _appDbContext.SaveChangesAsync();
        }
    }
}
