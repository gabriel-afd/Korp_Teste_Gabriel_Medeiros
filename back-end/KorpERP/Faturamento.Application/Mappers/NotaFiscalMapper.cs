using Faturamento.Application.DTOs.Response;
using Faturamento.Application.Messaging;
using Faturamento.Domain.Common;
using Faturamento.Domain.Entities;

namespace Faturamento.Application.Mappers;

public static class NotaFiscalMapper
{
    public static NotaFiscalResponseDto ToResponseDto(NotaFiscal nota)
    {
        return new NotaFiscalResponseDto
        {
            Id = nota.Id,
            Numero = nota.Numero,
            Status = nota.Status.ToString(),
            Data = nota.Data,
            Itens = nota.Itens.Select(ItemNotaMapper.ToResponseDto).ToList()
        };
    }

    public static List<NotaFiscalResponseDto> ToResponseDtoList(IEnumerable<NotaFiscal> notas)
    {
        return notas.Select(ToResponseDto).ToList();
    }

    public static PagedResult<NotaFiscalResponseDto> ToPagedResult(PagedResult<NotaFiscal> result)
    {
        return new PagedResult<NotaFiscalResponseDto>
        {
            Items = ToResponseDtoList(result.Items),
            Total = result.Total,
            Pagina = result.Pagina,
            TamanhoPagina = result.TamanhoPagina
        };
    }

    public static NotaImpressaEvent ToEvent(NotaFiscal nota)
    {
        return new NotaImpressaEvent
        {
            NotaId = nota.Id,
            Itens = nota.Itens.Select(ItemNotaMapper.ToEvent).ToList()
        };
    }
}
