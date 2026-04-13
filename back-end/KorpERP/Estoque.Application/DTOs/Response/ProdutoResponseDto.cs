namespace Estoque.Application.DTOs.Response
{
    public class ProdutoResponseDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public int Saldo { get; set; }
    }
}
