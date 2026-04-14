using System.Text;
using System.Text.Json;
using Estoque.Application.Services;
using Estoque.Domain.Entities;
using Estoque.Domain.Exceptions;
using Estoque.Infra.Data.Data;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Estoque.Api.Consumers;

public class NotaImpressaConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _rabbitMqHost;
    private IConnection? _connection;
    private IChannel? _channel;

    public NotaImpressaConsumer(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _rabbitMqHost = configuration["RabbitMQ:Host"] ?? "localhost";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var factory = new ConnectionFactory { HostName = _rabbitMqHost };
                _connection = await factory.CreateConnectionAsync(stoppingToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

                await _channel.QueueDeclareAsync(
                    queue: "nota-impressa",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    cancellationToken: stoppingToken
                );

                await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += async (sender, args) =>
                {
                    try
                    {
                        var messageId = args.BasicProperties?.MessageId
                            ?? Convert.ToBase64String(args.Body.ToArray());

                        using var scope = _scopeFactory.CreateScope();
                        var context = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();

                        if (await context.MensagensProcessadas.AnyAsync(m => m.MessageId == messageId))
                        {
                            await _channel.BasicAckAsync(args.DeliveryTag, multiple: false);
                            return;
                        }

                        var json = Encoding.UTF8.GetString(args.Body.ToArray());
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var evento = JsonSerializer.Deserialize<NotaImpressaEvent>(json, options);

                        if (evento?.Itens.Count > 0)
                        {
                            var produtoService = scope.ServiceProvider.GetRequiredService<ProdutoService>();

                            foreach (var item in evento.Itens)
                            {
                                await DebitarComRetryAsync(produtoService, item.CodigoProduto, item.Quantidade);
                            }
                        }

                        context.MensagensProcessadas.Add(new MensagemProcessada(messageId));
                        await context.SaveChangesAsync();

                        await _channel.BasicAckAsync(args.DeliveryTag, multiple: false);
                    }
                    catch
                    {
                        await _channel.BasicAckAsync(args.DeliveryTag, multiple: false);
                    }
                };

                await _channel.BasicConsumeAsync(queue: "nota-impressa", autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch
            {
                try { if (_channel != null) await _channel.CloseAsync(); } catch { }
                try { if (_connection != null) await _connection.CloseAsync(); } catch { }
                _channel = null;
                _connection = null;

                try { await Task.Delay(5000, stoppingToken); }
                catch (OperationCanceledException) { break; }
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) try { await _channel.CloseAsync(cancellationToken); } catch { }
        if (_connection != null) try { await _connection.CloseAsync(cancellationToken); } catch { }
        await base.StopAsync(cancellationToken);
    }

    private static async Task DebitarComRetryAsync(ProdutoService service, string codigo, int quantidade, int maxRetries = 3)
    {
        for (var tentativa = 1; tentativa <= maxRetries; tentativa++)
        {
            try
            {
                await service.DebitarAsync(codigo, quantidade);
                return;
            }
            catch (ConcorrenciaException) when (tentativa < maxRetries)
            {
                await Task.Delay(100 * tentativa);
            }
        }
    }
}