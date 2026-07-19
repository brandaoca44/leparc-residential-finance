using LeParc.ResidentialFinance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeParc.ResidentialFinance.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade Person no banco de dados.
/// </summary>
public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("People");

        builder.HasKey(person => person.Id);

        builder.Property(person => person.Id)
            .ValueGeneratedOnAdd();

        builder.Property(person => person.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(person => person.Age)
            .IsRequired();

        builder.HasMany(person => person.Transactions)
            .WithOne(transaction => transaction.Person)
            .HasForeignKey(transaction => transaction.PersonId)
            /*
             * Ao remover uma pessoa, todas as transações associadas
             * serão removidas automaticamente pelo banco de dados.
             */
            .OnDelete(DeleteBehavior.Cascade);
    }
}