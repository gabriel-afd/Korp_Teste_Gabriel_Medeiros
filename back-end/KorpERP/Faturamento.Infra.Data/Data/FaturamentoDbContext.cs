

using Faturamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.Infra.Data.Data
{
    public class FaturamentoDbContext : DbContext
    {
        public FaturamentoDbContext(DbContextOptions<FaturamentoDbContext> options) : base(options) { }

        public DbSet<NotaFiscal> NotasFiscais { get; set; }
    }
}
