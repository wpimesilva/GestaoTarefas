using GestaoTarefas.Api.Dtos;
using GestaoTarefas.Api.Services;
using GestaoTarefas.Api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace GestaoTarefas.Api.Controllers;

[ApiController]
[Route("api/v1/tarefas")]
public class TarefasController : ControllerBase
{
    private readonly ITarefaService _tarefaService;
    private readonly ILogger<TarefasController> _logger;

    public TarefasController(
        ITarefaService tarefaService,
        ILogger<TarefasController> logger)
    {
        _tarefaService = tarefaService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResposta<TarefaResposta>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResposta<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] TarefaCriacaoRequest request)
    {
        var tarefa = await _tarefaService.CriarAsync(request);

        _logger.LogInformation("Tarefa {TarefaId} criada com sucesso.", tarefa.Id);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = tarefa.Id },
            ApiResposta<TarefaResposta>.Ok(tarefa, MensagensResposta.TarefaCriada));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResposta<IEnumerable<TarefaResposta>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] TarefaFiltroRequest filtro)
    {
        var tarefas = await _tarefaService.ListarAsync(filtro);

        return Ok(ApiResposta<IEnumerable<TarefaResposta>>.Ok(
            tarefas,
            MensagensResposta.TarefasListadas));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResposta<TarefaResposta>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResposta<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var tarefa = await _tarefaService.ObterPorIdAsync(id);

        return Ok(ApiResposta<TarefaResposta>.Ok(
            tarefa,
            MensagensResposta.TarefaEncontrada));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResposta<TarefaResposta>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResposta<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResposta<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] TarefaAtualizacaoRequest request)
    {
        var tarefa = await _tarefaService.AtualizarAsync(id, request);

        _logger.LogInformation("Tarefa {TarefaId} atualizada com sucesso.", id);

        return Ok(ApiResposta<TarefaResposta>.Ok(
            tarefa,
            MensagensResposta.TarefaAtualizada));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResposta<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(Guid id)
    {
        await _tarefaService.ExcluirAsync(id);

        _logger.LogInformation("Tarefa {TarefaId} excluída com sucesso.", id);

        return NoContent();
    }
}
