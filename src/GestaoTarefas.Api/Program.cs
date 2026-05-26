using System.Text.Json.Serialization;
using GestaoTarefas.Api.Data;
using Microsoft.EntityFrameworkCore;

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


var app = builder.Build();



app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program { }
