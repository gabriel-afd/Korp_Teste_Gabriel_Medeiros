namespace Faturamento.Domain.Exceptions;

public class NotaFiscalJaFechadaException : DomainException
{
    public NotaFiscalJaFechadaException() : base("Nota fiscal já está fechada") { }
}