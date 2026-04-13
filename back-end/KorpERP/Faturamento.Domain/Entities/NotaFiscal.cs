using Faturamento.Domain.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Faturamento.Domain.Entities;

public class NotaFiscal
{
    public Guid Id { get; private set; }
    public int Numero { get; private set; }
    public StatusNota Status { get; private set; }
    public List<ItemNota> Itens { get; private set; } = new();
    public DateTime Data { get; private set; }

    public NotaFiscal(int numero)
    {
        Id = Guid.NewGuid();
        Numero = numero;
        Status = StatusNota.Aberta;
        Data = DateTime.Now;
    }

    public void Fechar()
    {
        if (Status == StatusNota.Fechada)
            throw new Exception("Nota já está fechada");

        Status = StatusNota.Fechada;
    }

    public void AdicionarItem(string codigoProduto, int quantidade)
    {
        if (Status == StatusNota.Fechada)
            throw new Exception("Não é possível alterar nota fechada");

        if (quantidade <= 0)
            throw new Exception("Quantidade inválida");

        Itens.Add(new ItemNota(codigoProduto, quantidade));
    }
}