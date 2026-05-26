using GestaoTarefas.Api.Dtos;
using GestaoTarefas.Api.Models;

namespace GestaoTarefas.Api.Repositories;

public interface ITarefaRepository
{
    Task<Tarefa> CriarAsync(Tarefa tarefa);
    Task<IEnumerable<Tarefa>> ListarAsync(TarefaFiltroRequest filtro);
    Task<Tarefa?> ObterPorIdAsync(Guid id);
    Task AtualizarAsync(Tarefa tarefa);
    Task ExcluirAsync(Tarefa tarefa);
}
