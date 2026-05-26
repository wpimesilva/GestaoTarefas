using GestaoTarefas.Api.Dtos;

namespace GestaoTarefas.Api.Services;

public interface ITarefaService
{
    Task<TarefaResposta> CriarAsync(TarefaCriacaoRequest request);
    Task<IEnumerable<TarefaResposta>> ListarAsync(TarefaFiltroRequest filtro);
    Task<TarefaResposta> ObterPorIdAsync(Guid id);
    Task<TarefaResposta> AtualizarAsync(Guid id, TarefaAtualizacaoRequest request);
    Task ExcluirAsync(Guid id);
}
