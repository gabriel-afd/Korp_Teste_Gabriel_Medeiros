namespace Faturamento.Application.Messaging;
public class ItemNotaEvent
{
    public string CodigoProduto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}