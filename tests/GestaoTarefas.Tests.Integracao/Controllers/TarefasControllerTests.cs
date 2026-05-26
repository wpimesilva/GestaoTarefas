using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GestaoTarefas.Api.Dtos;
using GestaoTarefas.Api.Enums;
using Xunit;

namespace GestaoTarefas.Tests.Integracao.Controllers;

public class TarefasControllerTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public TarefasControllerTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_DeveRetornarCreated_QuandoTarefaForValida()
    {
        var request = new TarefaCriacaoRequest
        {
            Titulo = "Criar teste integrado",
            Descricao = "Validar endpoint de criação",
            Status = StatusTarefa.Pendente
        };

        var response = await _client.PostAsJsonAsync("/api/v1/tarefas", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<ApiResposta<TarefaResposta>>();
        body.Should().NotBeNull();
        body!.Sucesso.Should().BeTrue();
        body.Dados!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Post_DeveRetornarBadRequest_QuandoTituloNaoForInformado()
    {
        var request = new TarefaCriacaoRequest
        {
            Titulo = "",
            Status = StatusTarefa.Pendente
        };

        var response = await _client.PostAsJsonAsync("/api/v1/tarefas", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_DeveRetornarOk_ComListaDeTarefas()
    {
        await _client.PostAsJsonAsync("/api/v1/tarefas", new TarefaCriacaoRequest
        {
            Titulo = "Listar tarefa",
            Status = StatusTarefa.EmProgresso
        });

        var response = await _client.GetAsync("/api/v1/tarefas");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ApiResposta<IEnumerable<TarefaResposta>>>();
        body.Should().NotBeNull();
        body!.Dados.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetPorId_DeveRetornarNotFound_QuandoTarefaNaoExistir()
    {
        var response = await _client.GetAsync($"/api/v1/tarefas/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_DeveRetornarNoContent_QuandoTarefaExistir()
    {
        var criarResponse = await _client.PostAsJsonAsync("/api/v1/tarefas", new TarefaCriacaoRequest
        {
            Titulo = "Excluir via API",
            Status = StatusTarefa.Pendente
        });

        var tarefaCriada = await criarResponse.Content.ReadFromJsonAsync<ApiResposta<TarefaResposta>>();

        var response = await _client.DeleteAsync($"/api/v1/tarefas/{tarefaCriada!.Dados!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_DeveRemoverTarefaDaConsulta_QuandoExcluidaLogicamente()
    {
        var criarResponse = await _client.PostAsJsonAsync("/api/v1/tarefas", new TarefaCriacaoRequest
        {
            Titulo = "Tarefa para exclusão lógica",
            Status = StatusTarefa.Pendente
        });

        var tarefaCriada = await criarResponse.Content.ReadFromJsonAsync<ApiResposta<TarefaResposta>>();

        await _client.DeleteAsync($"/api/v1/tarefas/{tarefaCriada!.Dados!.Id}");

        var buscarResponse = await _client.GetAsync($"/api/v1/tarefas/{tarefaCriada.Dados.Id}");

        buscarResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_DeveRetornarNotFound_QuandoTarefaNaoExistir()
    {
        var response = await _client.DeleteAsync($"/api/v1/tarefas/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}