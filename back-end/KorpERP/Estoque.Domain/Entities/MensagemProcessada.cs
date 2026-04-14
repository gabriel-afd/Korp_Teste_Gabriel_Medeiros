namespace Estoque.Domain.Entities;

public class MensagemProcessada
{
    public string MessageId { get; private set; }
    public DateTime ProcessadaEm { get; private set; }

    public MensagemProcessada(string messageId)
    {
        MessageId = messageId;
        ProcessadaEm = DateTime.UtcNow;
    }
}
