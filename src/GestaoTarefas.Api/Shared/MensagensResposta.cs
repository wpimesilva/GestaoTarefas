namespace GestaoTarefas.Api.Shared;

public static class MensagensResposta
{
    public const string TarefaCriada = "Tarefa criada com sucesso.";
    public const string TarefaAtualizada = "Tarefa atualizada com sucesso.";
    public const string TarefaExcluida = "Tarefa excluída com sucesso.";
    public const string TarefaEncontrada = "Tarefa encontrada com sucesso.";
    public const string TarefasListadas = "Tarefas listadas com sucesso.";
    public const string TarefaNaoEncontrada = "Tarefa não encontrada.";

    public const string TituloObrigatorio = "O título da tarefa é obrigatório.";
    public const string TituloMinimoCaracteres = "O título da tarefa deve conter no mínimo 3 caracteres.";
    public const string TituloLimiteCaracteres = "O título da tarefa deve conter no máximo 150 caracteres.";
    public const string DescricaoLimiteCaracteres = "A descrição da tarefa deve conter no máximo 500 caracteres.";
    public const string StatusInvalido = "O status informado é inválido.";

    public const string RequisicaoInvalida = "A requisição contém dados inválidos.";
    public const string ErroInterno = "Ocorreu um erro inesperado ao processar a solicitação.";
}