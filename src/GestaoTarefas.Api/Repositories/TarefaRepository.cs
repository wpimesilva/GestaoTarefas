using GestaoTarefas.Api.Data;
using GestaoTarefas.Api.Dtos;
using GestaoTarefas.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoTarefas.Api.Repositories;

public class TarefaRepository : ITarefaRepository
{
    private readonly AppDbContext _context;

    public TarefaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Tarefa> CriarAsync(Tarefa tarefa)
    {
        await _context.Tarefas.AddAsync(tarefa);
        await _context.SaveChangesAsync();

        return tarefa;
    }

    public async Task<IEnumerable<Tarefa>> ListarAsync(TarefaFiltroRequest filtro)
    {
        var query = _context.Tarefas.AsNoTracking().AsQueryable();

        if (filtro.Status.HasValue)
            query = query.Where(x => x.Status == filtro.Status.Value);

        if (filtro.DataVencimento.HasValue)
            query = query.Where(x => x.DataVencimento == filtro.DataVencimento.Value);

        return await query
            .OrderBy(x => x.DataVencimento == null)
            .ThenBy(x => x.DataVencimento)
            .ThenByDescending(x => x.DataCriacao)
            .ToListAsync();
    }

    public async Task<Tarefa?> ObterPorIdAsync(Guid id)
    {
        return await _context.Tarefas.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AtualizarAsync(Tarefa tarefa)
    {
        _context.Tarefas.Update(tarefa);
        await _context.SaveChangesAsync();
    }

    public async Task ExcluirAsync(Tarefa tarefa)
    {
        _context.Tarefas.Remove(tarefa);
        await _context.SaveChangesAsync();
    }
}
