using Faturamento.Application.DTOs.Create;
using Faturamento.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Faturamento.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotasFiscaisController : ControllerBase
    {
        private readonly NotaFiscalService _service;

        public NotasFiscaisController(NotaFiscalService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10)
        {
            var result = await _service.GetPagedAsync(pagina, tamanhoPagina);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var nota = await _service.GetByIdAsync(id);
            return Ok(nota);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] NotaFiscalCreateDto dto)
        {
            var nota = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = nota.Id }, nota);
        }

        [HttpPost("{id:guid}/imprimir")]
        public async Task<IActionResult> Imprimir(Guid id)
        {
            await _service.ImprimirAsync(id);
            return NoContent();
        }
    }
}
