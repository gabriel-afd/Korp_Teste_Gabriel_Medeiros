using Estoque.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infra.Data.Data
{
    public class EstoqueDbContext: DbContext
    {
        public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options){}

        public DbSet<Produto> Produtos { get; set; }
    }
}
