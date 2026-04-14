using Estoque.Application.DTOs;
using Estoque.Application.DTOs.Create;
using Estoque.Application.DTOs.Response;
using Estoque.Application.Interfaces;
using Estoque.Application.Mappers;
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

        return ProdutoMapper.ToPagedResult(result);
    }

    public async Task<ProdutoResponseDto?> GetByCodigoAsync(string codigo)
    {
        var produto = await _repository.GetByCodigoAsync(codigo);

        if (produto == null)
            throw new ProdutoNaoEncontradoException();

        return ProdutoMapper.ToResponseDto(produto);
    }

    public async Task<ProdutoResponseDto> CriarAsync(ProdutoCreateDto dto)
    {
        var produto = new Produto(dto.Codigo, dto.Descricao, dto.Saldo);
        await _repository.AddAsync(produto);

        return ProdutoMapper.ToResponseDto(produto);
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