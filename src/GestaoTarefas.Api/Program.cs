using GestaoTarefas.Api.Data;
using GestaoTarefas.Api.Middlewares;
using GestaoTarefas.Api.Repositories;
using GestaoTarefas.Api.Services;
using GestaoTarefas.Api.Shared;
using GestaoTarefas.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("GestaoTarefasDb");
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var erros = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors)
            .Select(x => string.IsNullOrWhiteSpace(x.ErrorMessage)
                ? MensagensResposta.RequisicaoInvalida
                : x.ErrorMessage)
            .ToList();

        var resposta = ApiResposta<object>.Falha(
            MensagensResposta.RequisicaoInvalida,
            erros);

        return new BadRequestObjectResult(resposta);
    };
});

builder.Services.AddScoped<ITarefaRepository, TarefaRepository>();
builder.Services.AddScoped<ITarefaService, TarefaService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = string.Empty;
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GestaoTarefas.Api v1");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program { }