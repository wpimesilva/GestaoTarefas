using GestaoTarefas.Api.Dtos;
using GestaoTarefas.Api.Exceptions;
using GestaoTarefas.Api.Shared;
using System.Net;
using System.Text.Json;

namespace GestaoTarefas.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidacaoException ex)
        {
            await EscreverRespostaAsync(
                context,
                HttpStatusCode.BadRequest,
                MensagensResposta.RequisicaoInvalida,
                ex.Erros);
        }
        catch (EntidadeNaoEncontradaException ex)
        {
            await EscreverRespostaAsync(
                context,
                HttpStatusCode.NotFound,
                ex.Message,
                new[] { ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao processar a requisição.");

            await EscreverRespostaAsync(
                context,
                HttpStatusCode.InternalServerError,
                MensagensResposta.ErroInterno,
                new[] { MensagensResposta.ErroInterno });
        }
    }

    private static async Task EscreverRespostaAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string mensagem,
        IEnumerable<string> erros)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var resposta = ApiResposta<object>.Falha(mensagem, erros);

        var json = JsonSerializer.Serialize(resposta, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
