using Estoque.Domain.Common;
using Estoque.Domain.Entities;

namespace Estoque.Application.Interfaces
{
    public interface IProdutoRepository
    {
        Task<List<Produto>> GetAllAsync();
        Task<Produto?> GetByCodigoAsync(string codigo);
        Task<Produto?> GetByIdAsync(Guid id);
        Task AddAsync(Produto produto);
        Task UpdateAsync(Produto produto);
        Task<PagedResult<Produto>> GetPagedAsync(int pagina, int tamanhoPagina);
    }
}
