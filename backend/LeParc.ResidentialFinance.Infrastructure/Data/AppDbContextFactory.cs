using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LeParc.ResidentialFinance.Infrastructure.Data;

/// <summary>
/// Cria o contexto durante a execução dos comandos do Entity Framework,
/// como migrations add e database update.
/// </summary>
public class AppDbContextFactory
    : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var solutionDirectory = FindSolutionDirectory();

        var databasePath = Path.Combine(
            solutionDirectory,
            "backend",
            "LeParc.ResidentialFinance.Api",
            "leparc-finance.db");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlite($"Data Source={databasePath}");

        return new AppDbContext(optionsBuilder.Options);
    }

    /// <summary>
    /// Procura a pasta raiz da solução subindo pelos diretórios
    /// até encontrar um arquivo com extensão .sln.
    /// </summary>
    private static string FindSolutionDirectory()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (directory is not null)
        {
            if (directory.GetFiles("*.sln").Length > 0)
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            "Não foi possível localizar a pasta da solução.");
    }
}