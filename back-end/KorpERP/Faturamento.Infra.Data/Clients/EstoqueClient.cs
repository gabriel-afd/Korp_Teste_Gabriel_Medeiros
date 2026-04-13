using Faturamento.Application.Interfaces;

namespace Faturamento.Infra.Data.Clients
{
    public class EstoqueClient : IEstoqueClient
    {
        private readonly HttpClient _http;

        public EstoqueClient(HttpClient http)
        {
            _http = http;
        }

        public async Task DebitarAsync(string codigo, int quantidade)
        {
            var response = await _http.PatchAsync($"/api/produtos/{codigo}/debitar?quantidade={quantidade}", null);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao debitar estoque: {erro}");
            }
        }
    }
}
