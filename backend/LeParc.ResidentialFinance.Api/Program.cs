using System.Text.Json.Serialization;
using LeParc.ResidentialFinance.Api.Middlewares;
using LeParc.ResidentialFinance.Application.Interfaces.Repositories;
using LeParc.ResidentialFinance.Application.Interfaces.Services;
using LeParc.ResidentialFinance.Application.Services;
using LeParc.ResidentialFinance.Infrastructure.Data;
using LeParc.ResidentialFinance.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "A string de conexão DefaultConnection não foi configurada.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IPersonService, PersonService>();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        /*
         * Permite que enums sejam enviados e recebidos como texto,
         * por exemplo: "Expense" e "Income".
         */
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    /*
     * Aplica migrations pendentes automaticamente somente em
     * desenvolvimento, facilitando a execução local e evitando
     * alterações automáticas no banco em produção.
     */
    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    dbContext.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("Frontend");

app.UseAuthorization();

app.MapControllers();

app.Run();