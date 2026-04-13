namespace Faturamento.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(T evento, string fila);
}