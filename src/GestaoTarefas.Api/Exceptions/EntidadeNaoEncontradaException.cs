namespace GestaoTarefas.Api.Exceptions;

public class EntidadeNaoEncontradaException : Exception
{
    public EntidadeNaoEncontradaException(string message)
        : base(message)
    {
    }
}
