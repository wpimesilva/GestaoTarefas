using GestaoTarefas.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoTarefas.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tarefa> Tarefas => Set<Tarefa>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tarefa>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Titulo)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Descricao)
                .HasMaxLength(500);

            entity.Property(x => x.Status)
                .IsRequired();

            entity.Property(x => x.DataCriacao)
                .IsRequired();

            entity.Property(x => x.Excluida)
                .IsRequired();

            entity.HasQueryFilter(x => !x.Excluida);
        });
    }
}