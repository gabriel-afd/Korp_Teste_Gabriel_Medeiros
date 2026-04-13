using System.Net;
using System.Text.Json;
using Faturamento.Domain.Exceptions;
using Polly.CircuitBreaker;

namespace Faturamento.Api.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError("Circuit breaker aberto: {Message}", ex.Message);
                await WriteResponse(context, HttpStatusCode.ServiceUnavailable,
                    "Serviço de Estoque indisponível. Tente novamente em instantes.");
            }
            catch (DomainException ex)
            {
                await WriteResponse(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Serviço de Estoque indisponível: {Message}", ex.Message);

                await WriteResponse(context, HttpStatusCode.ServiceUnavailable, "Serviço de Estoque indisponível. Tente novamente em instantes.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Erro de domínio: {Message}", ex.Message);
                await WriteResponse(context, HttpStatusCode.BadRequest, ex.Message);
            }
        }

        private static async Task WriteResponse(HttpContext context, HttpStatusCode status, string message)
        {
            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";

            var body = JsonSerializer.Serialize(new { error = message });
            await context.Response.WriteAsync(body);
        }
    }
}
