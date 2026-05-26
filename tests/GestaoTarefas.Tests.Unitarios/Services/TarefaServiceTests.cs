using FluentAssertions;
using GestaoTarefas.Api.Data;
using GestaoTarefas.Api.Dtos;
using GestaoTarefas.Api.Enums;
using GestaoTarefas.Api.Exceptions;
using GestaoTarefas.Api.Repositories;
using GestaoTarefas.Api.Services;
using GestaoTarefas.Api.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace GestaoTarefas.Tests.Unitarios.Services;

public class TarefaServiceTests
{
    [Fact]
    public async Task CriarAsync_DeveCriarTarefa_QuandoDadosForemValidos()
    {
        var service = CriarService();

        var request = new TarefaCriacaoRequest
        {
            Titulo = "Estudar .NET",
            Descricao = "Revisar testes unitários",
            DataVencimento = new DateOnly(2026, 06, 01),
            Status = StatusTarefa.Pendente
        };

        var resultado = await service.CriarAsync(request);

        resultado.Id.Should().NotBeEmpty();
        resultado.Titulo.Should().Be("Estudar .NET");
        resultado.Descricao.Should().Be("Revisar testes unitários");
        resultado.Status.Should().Be(StatusTarefa.Pendente);
        resultado.DataCriacao.Should().NotBe(default);
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarErro_QuandoTituloNaoForInformado()
    {
        var service = CriarService();

        var request = new TarefaCriacaoRequest
        {
            Titulo = "",
            Status = StatusTarefa.Pendente
        };

        var action = async () => await service.CriarAsync(request);

        await action.Should()
            .ThrowAsync<ValidacaoException>()
            .Where(x => x.Erros.Contains(MensagensResposta.TituloObrigatorio));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarErro_QuandoTituloTiverMenosDeTresCaracteres()
    {
        var service = CriarService();

        var request = new TarefaCriacaoRequest
        {
            Titulo = "AB",
            Status = StatusTarefa.Pendente
        };

        var action = async () => await service.CriarAsync(request);

        await action.Should()
            .ThrowAsync<ValidacaoException>()
            .Where(x => x.Erros.Contains(MensagensResposta.TituloMinimoCaracteres));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarErro_QuandoTituloUltrapassarLimite()
    {
        var service = CriarService();

        var request = new TarefaCriacaoRequest
        {
            Titulo = new string('A', 151),
            Status = StatusTarefa.Pendente
        };

        var action = async () => await service.CriarAsync(request);

        await action.Should()
            .ThrowAsync<ValidacaoException>()
            .Where(x => x.Erros.Contains(MensagensResposta.TituloLimiteCaracteres));
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarTarefa_QuandoElaExistir()
    {
        var service = CriarService();

        var criada = await service.CriarAsync(new TarefaCriacaoRequest
        {
            Titulo = "Tarefa inicial",
            Status = StatusTarefa.Pendente
        });

        var atualizada = await service.AtualizarAsync(criada.Id, new TarefaAtualizacaoRequest
        {
            Titulo = "Tarefa atualizada",
            Status = StatusTarefa.Concluida
        });

        atualizada.Titulo.Should().Be("Tarefa atualizada");
        atualizada.Status.Should().Be(StatusTarefa.Concluida);
        atualizada.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarErro_QuandoTarefaNaoExistir()
    {
        var service = CriarService();

        var action = async () => await service.AtualizarAsync(Guid.NewGuid(), new TarefaAtualizacaoRequest
        {
            Titulo = "Tarefa inexistente",
            Status = StatusTarefa.EmProgresso
        });

        await action.Should()
            .ThrowAsync<EntidadeNaoEncontradaException>()
            .WithMessage(MensagensResposta.TarefaNaoEncontrada);
    }

    [Fact]
    public async Task ListarAsync_DeveFiltrarPorStatus()
    {
        var service = CriarService();

        await service.CriarAsync(new TarefaCriacaoRequest
        {
            Titulo = "Tarefa pendente",
            Status = StatusTarefa.Pendente
        });

        await service.CriarAsync(new TarefaCriacaoRequest
        {
            Titulo = "Tarefa concluída",
            Status = StatusTarefa.Concluida
        });

        var resultado = await service.ListarAsync(new TarefaFiltroRequest
        {
            Status = StatusTarefa.Concluida
        });

        resultado.Should().HaveCount(1);
        resultado.First().Status.Should().Be(StatusTarefa.Concluida);
    }

    [Fact]
    public async Task ListarAsync_DeveFiltrarPorDataVencimento()
    {
        var service = CriarService();

        var dataFiltro = new DateOnly(2026, 06, 01);

        await service.CriarAsync(new TarefaCriacaoRequest
        {
            Titulo = "Tarefa com vencimento",
            DataVencimento = dataFiltro,
            Status = StatusTarefa.Pendente
        });

        await service.CriarAsync(new TarefaCriacaoRequest
        {
            Titulo = "Tarefa sem vencimento",
            Status = StatusTarefa.Pendente
        });

        var resultado = await service.ListarAsync(new TarefaFiltroRequest
        {
            DataVencimento = dataFiltro
        });

        resultado.Should().HaveCount(1);
        resultado.First().DataVencimento.Should().Be(dataFiltro);
    }

    [Fact]
    public async Task ExcluirAsync_DeveExcluirTarefaLogicamente_QuandoElaExistir()
    {
        var context = CriarContext();
        var repository = new TarefaRepository(context);
        var service = new TarefaService(repository, NullLogger<TarefaService>.Instance);

        var criada = await service.CriarAsync(new TarefaCriacaoRequest
        {
            Titulo = "Excluir tarefa",
            Status = StatusTarefa.Pendente
        });

        await service.ExcluirAsync(criada.Id);

        var tarefaExcluida = await context.Tarefas
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == criada.Id);

        tarefaExcluida.Should().NotBeNull();
        tarefaExcluida!.Excluida.Should().BeTrue();
        tarefaExcluida.DataExclusao.Should().NotBeNull();
    }

    [Fact]
    public async Task ExcluirAsync_DeveRemoverTarefaDaListagem_QuandoExcluidaLogicamente()
    {
        var service = CriarService();

        var criada = await service.CriarAsync(new TarefaCriacaoRequest
        {
            Titulo = "Tarefa para listagem",
            Status = StatusTarefa.Pendente
        });

        await service.ExcluirAsync(criada.Id);

        var resultado = await service.ListarAsync(new TarefaFiltroRequest());

        resultado.Should().NotContain(x => x.Id == criada.Id);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNaoEncontrado_QuandoTarefaEstiverExcluidaLogicamente()
    {
        var service = CriarService();

        var criada = await service.CriarAsync(new TarefaCriacaoRequest
        {
            Titulo = "Tarefa excluída",
            Status = StatusTarefa.Pendente
        });

        await service.ExcluirAsync(criada.Id);

        var action = async () => await service.ObterPorIdAsync(criada.Id);

        await action.Should()
            .ThrowAsync<EntidadeNaoEncontradaException>()
            .WithMessage(MensagensResposta.TarefaNaoEncontrada);
    }

    [Fact]
    public async Task ExcluirAsync_DeveRetornarErro_QuandoTarefaNaoExistir()
    {
        var service = CriarService();

        var action = async () => await service.ExcluirAsync(Guid.NewGuid());

        await action.Should()
            .ThrowAsync<EntidadeNaoEncontradaException>()
            .WithMessage(MensagensResposta.TarefaNaoEncontrada);
    }

    private static TarefaService CriarService()
    {
        var context = CriarContext();
        var repository = new TarefaRepository(context);

        return new TarefaService(repository, NullLogger<TarefaService>.Instance);
    }

    private static AppDbContext CriarContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}