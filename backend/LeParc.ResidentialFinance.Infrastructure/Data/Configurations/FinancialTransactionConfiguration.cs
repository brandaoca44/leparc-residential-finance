using LeParc.ResidentialFinance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeParc.ResidentialFinance.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade FinancialTransaction no banco de dados.
/// </summary>
public class FinancialTransactionConfiguration
    : IEntityTypeConfiguration<FinancialTransaction>
{
    public void Configure(EntityTypeBuilder<FinancialTransaction> builder)
    {
        builder.ToTable("FinancialTransactions");

        builder.HasKey(transaction => transaction.Id);

        builder.Property(transaction => transaction.Id)
            .ValueGeneratedOnAdd();

        builder.Property(transaction => transaction.Description)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(transaction => transaction.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(transaction => transaction.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(transaction => transaction.PersonId)
            .IsRequired();
    }
}