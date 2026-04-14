namespace Estoque.Domain.Exceptions;

public class ConcorrenciaException : DomainException
{
    public ConcorrenciaException() : base("O produto foi alterado por outro processo. Tente novamente.") { }
}
