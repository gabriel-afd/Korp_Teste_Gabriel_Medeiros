using Faturamento.Application.DTOs.Create;
using Faturamento.Application.DTOs.Response;
using Faturamento.Application.Interfaces;
using Faturamento.Application.Mappers;
using Faturamento.Application.Messaging;
using Faturamento.Domain.Common;
using Faturamento.Domain.Entities;
using Faturamento.Domain.Enums;
using Faturamento.Domain.Exceptions;

namespace Faturamento.Application.Services
{
    public class NotaFiscalService
    {
        private readonly IEstoqueClient _estoqueClient;
        private readonly INotaFiscalRepository _repository;
        private readonly IEventPublisher _eventPublisher;

        public NotaFiscalService(IEstoqueClient estoqueClient, INotaFiscalRepository repository, IEventPublisher eventPublisher)
        {
            _estoqueClient = estoqueClient;
            _repository = repository;
            _eventPublisher = eventPublisher;
        }

        public async Task<PagedResult<NotaFiscalResponseDto>> GetPagedAsync(int pagina = 1, int tamanhoPagina = 10)
        {
            var result = await _repository.GetPagedAsync(pagina, tamanhoPagina);

            return NotaFiscalMapper.ToPagedResult(result);
        }

        public async Task<NotaFiscalResponseDto?> GetByIdAsync(Guid id)
        {
            var nota = await _repository.GetByIdAsync(id);

            if (nota == null)
                throw new NotaFiscalNaoEncontradaException();

            return NotaFiscalMapper.ToResponseDto(nota);
        }

        public async Task<NotaFiscalResponseDto> CriarAsync(NotaFiscalCreateDto dto)
        {
            var numero = await _repository.ProximoNumeroAsync();

            var nota = new NotaFiscal(numero);

            foreach (var item in dto.Itens)
            {
                nota.AdicionarItem(item.CodigoProduto, item.Descricao, item.Quantidade);
            }

            await _repository.AddAsync(nota);

            foreach (var item in nota.Itens)
            {
                await _estoqueClient.ValidarProdutoAsync(item.CodigoProduto, item.Quantidade);
            }

            return NotaFiscalMapper.ToResponseDto(nota);
        }

        public async Task ImprimirAsync(Guid id)
        {
            var nota = await _repository.GetByIdAsync(id);

            if (nota == null)
                throw new NotaFiscalNaoEncontradaException();

            if (nota.Status == StatusNota.Fechada)
                throw new NotaFiscalJaFechadaException();

            foreach (var item in nota.Itens)
            {
                await _estoqueClient.ValidarProdutoAsync(item.CodigoProduto, item.Quantidade);
            }

            nota.Fechar();
            await _repository.UpdateAsync(nota);

            var evento = NotaFiscalMapper.ToEvent(nota);

            await _eventPublisher.PublishAsync(evento, "nota-impressa");
        }
    }
}
