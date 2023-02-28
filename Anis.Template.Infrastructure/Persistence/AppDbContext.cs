using Anis.Template.Domain.Entities;
using Anis.Template.Infrastructure.Persistence.Repositories;
using Anis.Template.Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;


namespace Anis.Template.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CardConfig());
            base.OnModelCreating(modelBuilder);
        }
    }
}
