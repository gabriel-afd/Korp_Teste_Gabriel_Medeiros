namespace Faturamento.Application.Interfaces;

public interface IEstoqueClient
{
    Task ValidarProdutoAsync(string codigo, int quantidade);
}
