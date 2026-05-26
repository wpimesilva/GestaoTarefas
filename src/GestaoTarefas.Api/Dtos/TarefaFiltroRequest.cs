using System.ComponentModel.DataAnnotations;
using GestaoTarefas.Api.Enums;
using GestaoTarefas.Api.Shared;

namespace GestaoTarefas.Api.Dtos;

public class TarefaFiltroRequest
{
    [EnumDataType(typeof(StatusTarefa), ErrorMessage = MensagensResposta.StatusInvalido)]
    public StatusTarefa? Status { get; set; }

    public DateOnly? DataVencimento { get; set; }
}