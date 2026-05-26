using System.ComponentModel.DataAnnotations;
using GestaoTarefas.Api.Enums;
using GestaoTarefas.Api.Shared;

namespace GestaoTarefas.Api.Dtos;

public class TarefaCriacaoRequest
{
    [Required(ErrorMessage = MensagensResposta.TituloObrigatorio)]
    [MinLength(3, ErrorMessage = MensagensResposta.TituloMinimoCaracteres)]
    [MaxLength(150, ErrorMessage = MensagensResposta.TituloLimiteCaracteres)]
    public string? Titulo { get; set; }

    [MaxLength(500, ErrorMessage = MensagensResposta.DescricaoLimiteCaracteres)]
    public string? Descricao { get; set; }

    public DateOnly? DataVencimento { get; set; }

    [EnumDataType(typeof(StatusTarefa), ErrorMessage = MensagensResposta.StatusInvalido)]
    public StatusTarefa Status { get; set; } = StatusTarefa.Pendente;
}