namespace Faturamento.Application.Interfaces
{
    public interface IEstoqueClient
    {
        Task DebitarAsync(string codigo, int quantidade);
    }
}
