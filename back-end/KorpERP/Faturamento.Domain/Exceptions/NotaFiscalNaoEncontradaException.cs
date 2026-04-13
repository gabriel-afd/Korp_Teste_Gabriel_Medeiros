namespace Faturamento.Domain.Exceptions;

public class NotaFiscalNaoEncontradaException : DomainException
{
    public NotaFiscalNaoEncontradaException() : base("Nota fiscal não encontrada") { }
}