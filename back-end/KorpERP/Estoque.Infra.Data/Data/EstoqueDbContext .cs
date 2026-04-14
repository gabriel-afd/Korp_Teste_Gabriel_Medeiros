using Estoque.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infra.Data.Data
{
    public class EstoqueDbContext: DbContext
    {
        public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options){}

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<MensagemProcessada> MensagensProcessadas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>()
                .Property(p => p.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<MensagemProcessada>()
                .HasKey(m => m.MessageId);
        }
    }
}
