using Estoque.Application.DTOs;
using Estoque.Application.DTOs.Create;
using Estoque.Application.DTOs.Response;
using Estoque.Application.Interfaces;
using Estoque.Domain.Common;
using Estoque.Domain.Entities;
using Estoque.Domain.Exceptions;

namespace Estoque.Application.Services;

public class ProdutoService
{
    private readonly IProdutoRepository _repository;

    public ProdutoService(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ProdutoResponseDto>> GetPagedAsync(int pagina = 1, int tamanhoPagina = 10)
    {
        var result = await _repository.GetPagedAsync(pagina, tamanhoPagina);

        return new PagedResult<ProdutoResponseDto>
        {
            Items = result.Items.Select(p => new ProdutoResponseDto
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Descricao = p.Descricao,
                Saldo = p.Saldo
            }).ToList(),
            Total = result.Total,
            Pagina = result.Pagina,
            TamanhoPagina = result.TamanhoPagina
        };
    }

    public async Task<ProdutoResponseDto> CriarAsync(ProdutoCreateDto dto)
    {
        var produto = new Produto(dto.Codigo, dto.Descricao, dto.Saldo);
        await _repository.AddAsync(produto);

        return new ProdutoResponseDto
        {
            Id = produto.Id,
            Codigo = produto.Codigo,
            Descricao = produto.Descricao,
            Saldo = produto.Saldo
        };
    }

    public async Task DebitarAsync(string codigo, int quantidade)
    {
        var produto = await _repository.GetByCodigoAsync(codigo);

        if (produto == null)
            throw new ProdutoNaoEncontradoException();

        produto.Debitar(quantidade);

        await _repository.UpdateAsync(produto);
    }
}