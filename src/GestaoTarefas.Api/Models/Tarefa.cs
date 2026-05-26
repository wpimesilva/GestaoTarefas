using GestaoTarefas.Api.Enums;

namespace GestaoTarefas.Api.Models;

public class Tarefa
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateOnly? DataVencimento { get; set; }
    public StatusTarefa Status { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}
