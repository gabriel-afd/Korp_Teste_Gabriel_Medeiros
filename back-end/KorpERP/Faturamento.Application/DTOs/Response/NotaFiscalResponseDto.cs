namespace Faturamento.Application.DTOs.Response;

public class NotaFiscalResponseDto
{
    public Guid Id { get; set; }
    public int Numero { get; set; }
    public string Status { get; set; }
    public DateTime Data { get; set; }
    public List<ItemNotaResponseDto> Itens { get; set; } = new();
}