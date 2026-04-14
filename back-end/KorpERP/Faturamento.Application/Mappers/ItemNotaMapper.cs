using Faturamento.Application.DTOs.Response;
using Faturamento.Application.Messaging;
using Faturamento.Domain.Entities;

namespace Faturamento.Application.Mappers;

public static class ItemNotaMapper
{
    public static ItemNotaResponseDto ToResponseDto(ItemNota item)
    {
        return new ItemNotaResponseDto
        {
            CodigoProduto = item.CodigoProduto,
            DescricaoProduto = item.DescricaoProduto,
            Quantidade = item.Quantidade
        };
    }

    public static ItemNotaEvent ToEvent(ItemNota item)
    {
        return new ItemNotaEvent
        {
            CodigoProduto = item.CodigoProduto,
            Quantidade = item.Quantidade
        };
    }
}
