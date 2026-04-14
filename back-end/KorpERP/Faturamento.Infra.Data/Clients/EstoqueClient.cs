using System.Net.Http.Json;
using Estoque.Application.DTOs.Response;
using Estoque.Domain.Exceptions;
using Faturamento.Application.Interfaces;

namespace Faturamento.Infra.Data.Clients;

public class EstoqueClient : IEstoqueClient
{
    private readonly HttpClient _httpClient;

    public EstoqueClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task ValidarProdutoAsync(string codigo, int quantidade)
    {
        var response = await _httpClient.GetAsync($"/api/produtos/{codigo}");

        if (!response.IsSuccessStatusCode)
            throw new ProdutoNaoEncontradoException();

        var produto = await response.Content.ReadFromJsonAsync<ProdutoResponseDto>();

        if (produto!.Saldo < quantidade)
            throw new SaldoInsuficienteException();
    }
}
