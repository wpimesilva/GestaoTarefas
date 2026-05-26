using System.ComponentModel.DataAnnotations;
using GestaoTarefas.Api.Dtos;
using GestaoTarefas.Api.Enums;
using GestaoTarefas.Api.Exceptions;
using GestaoTarefas.Api.Models;
using GestaoTarefas.Api.Repositories;
using GestaoTarefas.Api.Shared;

namespace GestaoTarefas.Api.Services;

public class TarefaService : ITarefaService
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly ILogger<TarefaService> _logger;

    public TarefaService(
        ITarefaRepository tarefaRepository,
        ILogger<TarefaService> logger)
    {
        _tarefaRepository = tarefaRepository;
        _logger = logger;
    }

    public async Task<TarefaResposta> CriarAsync(TarefaCriacaoRequest request)
    {
        ValidarRequest(request);

        var tarefa = new Tarefa
        {
            Id = Guid.NewGuid(),
            Titulo = request.Titulo!.Trim(),
            Descricao = string.IsNullOrWhiteSpace(request.Descricao)
                ? null
                : request.Descricao.Trim(),
            DataVencimento = request.DataVencimento,
            Status = request.Status,
            DataCriacao = DateTime.UtcNow
        };

        await _tarefaRepository.CriarAsync(tarefa);

        _logger.LogInformation("Tarefa {TarefaId} criada com sucesso.", tarefa.Id);

        return MapearResposta(tarefa);
    }

    public async Task<IEnumerable<TarefaResposta>> ListarAsync(TarefaFiltroRequest filtro)
    {
        ValidarRequest(filtro);

        var tarefas = await _tarefaRepository.ListarAsync(filtro);

        return tarefas.Select(MapearResposta);
    }

    public async Task<TarefaResposta> ObterPorIdAsync(Guid id)
    {
        var tarefa = await BuscarTarefaAsync(id);

        return MapearResposta(tarefa);
    }

    public async Task<TarefaResposta> AtualizarAsync(Guid id, TarefaAtualizacaoRequest request)
    {
        ValidarRequest(request);

        var tarefa = await BuscarTarefaAsync(id);

        tarefa.Titulo = request.Titulo!.Trim();
        tarefa.Descricao = string.IsNullOrWhiteSpace(request.Descricao)
            ? null
            : request.Descricao.Trim();
        tarefa.DataVencimento = request.DataVencimento;
        tarefa.Status = request.Status;
        tarefa.DataAtualizacao = DateTime.UtcNow;

        await _tarefaRepository.AtualizarAsync(tarefa);

        _logger.LogInformation("Tarefa {TarefaId} atualizada com sucesso.", tarefa.Id);

        return MapearResposta(tarefa);
    }

    public async Task ExcluirAsync(Guid id)
    {
        var tarefa = await BuscarTarefaAsync(id);

        await _tarefaRepository.ExcluirAsync(tarefa);

        _logger.LogInformation("Tarefa {TarefaId} excluída com sucesso.", tarefa.Id);
    }

    private async Task<Tarefa> BuscarTarefaAsync(Guid id)
    {
        var tarefa = await _tarefaRepository.ObterPorIdAsync(id);

        if (tarefa is null)
            throw new EntidadeNaoEncontradaException(MensagensResposta.TarefaNaoEncontrada);

        return tarefa;
    }

    private static void ValidarRequest<TRequest>(TRequest request)
    {
        var validationContext = new ValidationContext(request!);
        var validationResults = new List<ValidationResult>();

        var valido = Validator.TryValidateObject(
            request!,
            validationContext,
            validationResults,
            validateAllProperties: true);

        if (!valido)
            throw new ValidacaoException(validationResults.Select(x => x.ErrorMessage!));
    }

    private static TarefaResposta MapearResposta(Tarefa tarefa)
    {
        return new TarefaResposta
        {
            Id = tarefa.Id,
            Titulo = tarefa.Titulo,
            Descricao = tarefa.Descricao,
            DataVencimento = tarefa.DataVencimento,
            Status = tarefa.Status,
            DataCriacao = tarefa.DataCriacao,
            DataAtualizacao = tarefa.DataAtualizacao
        };
    }
}