namespace GestaoTarefas.Api.Dtos;

public class ApiResposta<T>
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public T? Dados { get; set; }
    public IEnumerable<string>? Erros { get; set; }

    public static ApiResposta<T> Ok(T dados, string mensagem)
    {
        return new ApiResposta<T>
        {
            Sucesso = true,
            Mensagem = mensagem,
            Dados = dados
        };
    }

    public static ApiResposta<T> Falha(string mensagem, IEnumerable<string>? erros = null)
    {
        return new ApiResposta<T>
        {
            Sucesso = false,
            Mensagem = mensagem,
            Erros = erros
        };
    }
}
