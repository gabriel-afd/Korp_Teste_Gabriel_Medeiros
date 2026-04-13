namespace Faturamento.Application.DTOs.Create
{
    public class NotaFiscalCreateDto
    {
        public List<ItemNotaCreateDto> Itens { get; set; } = new();

    }
}
