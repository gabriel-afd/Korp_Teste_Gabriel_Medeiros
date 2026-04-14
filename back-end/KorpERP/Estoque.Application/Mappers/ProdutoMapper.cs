using Estoque.Application.DTOs.Response;
using Estoque.Domain.Common;
using Estoque.Domain.Entities;

namespace Estoque.Application.Mappers;

public static class ProdutoMapper
{
    public static ProdutoResponseDto ToResponseDto(Produto produto)
    {
        return new ProdutoResponseDto
        {
            Id = produto.Id,
            Codigo = produto.Codigo,
            Descricao = produto.Descricao,
            Saldo = produto.Saldo
        };
    }

    public static List<ProdutoResponseDto> ToResponseDtoList(IEnumerable<Produto> produtos)
    {
        return produtos.Select(ToResponseDto).ToList();
    }

    public static PagedResult<ProdutoResponseDto> ToPagedResult(PagedResult<Produto> result)
    {
        return new PagedResult<ProdutoResponseDto>
        {
            Items = ToResponseDtoList(result.Items),
            Total = result.Total,
            Pagina = result.Pagina,
            TamanhoPagina = result.TamanhoPagina
        };
    }
}
