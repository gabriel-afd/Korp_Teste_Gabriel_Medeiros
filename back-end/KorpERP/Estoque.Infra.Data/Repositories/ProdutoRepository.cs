using Estoque.Domain.Entities;
using Estoque.Infra.Data.Data;
using Estoque.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Estoque.Domain.Common;

namespace Estoque.Infra.Data.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly EstoqueDbContext _context;

        public ProdutoRepository(EstoqueDbContext context)
        {
            _context = context;
        }

        public async Task<List<Produto>> GetAllAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        public async Task<Produto?> GetByCodigoAsync(string codigo)
        {
            return await _context.Produtos
                .FirstOrDefaultAsync(p => p.Codigo == codigo);
        }

        public async Task<Produto?> GetByIdAsync(Guid id)
        {
            return await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task AddAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<Produto>> GetPagedAsync(int pagina, int tamanhoPagina)
        {
            var query = _context.Produtos.AsQueryable();

            var total = await query.CountAsync();

            var items = await query
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return new PagedResult<Produto>
            {
                Items = items,
                Total = total,
                Pagina = pagina,
                TamanhoPagina = tamanhoPagina
            };
        }
    }
}
