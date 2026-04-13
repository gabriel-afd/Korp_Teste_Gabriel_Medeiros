using Estoque.Domain.Exceptions;

namespace Estoque.Domain.Entities;

public class Produto
{
    public Guid Id { get; private set; }
    public string Codigo { get; private set; }
    public string Descricao { get; private set; }
    public int Saldo { get; private set; }

    public Produto(string codigo, string descricao, int saldo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ProdutoInvalidoException("Código é obrigatório");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new ProdutoInvalidoException("Descrição é obrigatória");

        if (saldo < 0)
            throw new ProdutoInvalidoException("Saldo não pode ser negativo");

        Id = Guid.NewGuid();
        Codigo = codigo;
        Descricao = descricao;
        Saldo = saldo;
    }

    public void Debitar(int quantidade)
    {
        if (quantidade <= 0)
            throw new ProdutoInvalidoException("Quantidade inválida");

        if (Saldo < quantidade)
            throw new SaldoInsuficienteException();

        Saldo -= quantidade;
    }
}