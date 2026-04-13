namespace Faturamento.Domain.Entities;

public class ItemNota
{
    public Guid Id { get; private set; }
    public Guid NotaFiscalId { get; private set; }
    public string CodigoProduto { get; private set; }
    public int Quantidade { get; private set; }

    public ItemNota(string codigoProduto, int quantidade)
    {
        if (string.IsNullOrWhiteSpace(codigoProduto))
            throw new Exception("Produto inválido");

        if (quantidade <= 0)
            throw new Exception("Quantidade inválida");

        Id = Guid.NewGuid();
        CodigoProduto = codigoProduto;
        Quantidade = quantidade;
    }
}