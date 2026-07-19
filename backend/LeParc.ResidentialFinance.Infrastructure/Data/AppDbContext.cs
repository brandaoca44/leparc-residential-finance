using LeParc.ResidentialFinance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeParc.ResidentialFinance.Infrastructure.Data;

/// <summary>
/// Contexto principal responsável pela comunicação entre a aplicação
/// e o banco de dados SQLite.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Person> People => Set<Person>();

    public DbSet<FinancialTransaction> FinancialTransactions =>
        Set<FinancialTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /*
         * Aplica automaticamente todas as configurações de entidades
         * existentes no assembly de Infrastructure.
         */
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}