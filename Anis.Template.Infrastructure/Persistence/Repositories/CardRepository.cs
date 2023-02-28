using Anis.Template.Application.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anis.Template.Infrastructure.Persistence.Repositories
{
    public class CardRepository : ICardRepository
    {
        private AppDbContext _appDbContext;

        public CardRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
