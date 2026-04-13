using Estoque.Application.DTOs;
using Estoque.Application.DTOs.Create;
using Estoque.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.API.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutosController : ControllerBase
{
    private readonly ProdutoService _service;

    public ProdutosController(ProdutoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10)
    {
        var result = await _service.GetPagedAsync(pagina, tamanhoPagina);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(ProdutoCreateDto dto)
    {
        var produto = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = produto.Id }, produto);
    }

    [HttpPatch("{codigo}/debitar")]
    public async Task<IActionResult> Debitar(string codigo, int quantidade)
    {
        await _service.DebitarAsync(codigo, quantidade);
        return Ok();
    }
}