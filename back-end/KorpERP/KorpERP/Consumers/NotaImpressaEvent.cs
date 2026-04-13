namespace Estoque.Api.Consumers;

public class NotaImpressaEvent
{
    public Guid NotaId { get; set; }
    public List<ItemNotaEvent> Itens { get; set; } = new();
}


