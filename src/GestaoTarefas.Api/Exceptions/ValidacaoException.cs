namespace GestaoTarefas.Api.Exceptions;

public class ValidacaoException : Exception
{
    public IEnumerable<string> Erros { get; }

    public ValidacaoException(string erro)
        : base(erro)
    {
        Erros = new List<string> { erro };
    }

    public ValidacaoException(IEnumerable<string> erros)
        : base("A requisição contém dados inválidos.")
    {
        Erros = erros;
    }
}
