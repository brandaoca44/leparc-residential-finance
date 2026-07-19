using System.Net;
using System.Text.Json;
using LeParc.ResidentialFinance.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LeParc.ResidentialFinance.Api.Middlewares;

/// <summary>
/// Converte exceções conhecidas em respostas HTTP padronizadas,
/// evitando exposição de detalhes internos da aplicação.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException)
            when (context.RequestAborted.IsCancellationRequested)
        {
            /*
             * A requisição foi cancelada pelo cliente.
             * Não é necessário gerar uma resposta adicional.
             */
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var statusCode = exception switch
        {
            NotFoundException =>
                HttpStatusCode.NotFound,

            ConflictException =>
                HttpStatusCode.Conflict,

            ArgumentException or
            ArgumentOutOfRangeException or
            InvalidOperationException =>
                HttpStatusCode.BadRequest,

            _ =>
                HttpStatusCode.InternalServerError
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(
                exception,
                "Ocorreu um erro interno não tratado. TraceId: {TraceId}",
                context.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning(
                "A requisição foi rejeitada. Status: {StatusCode}. " +
                "Motivo: {Message}. TraceId: {TraceId}",
                (int)statusCode,
                exception.Message,
                context.TraceIdentifier);
        }

        var message = statusCode == HttpStatusCode.InternalServerError
            ? "Ocorreu um erro interno ao processar a solicitação."
            : exception.Message;

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = GetTitle(statusCode),
            Detail = message,
            Instance = context.Request.Path
        };

        problemDetails.Extensions["traceId"] =
            context.TraceIdentifier;

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(
            problemDetails,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        await context.Response.WriteAsync(json);
    }

    private static string GetTitle(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest =>
                "Dados inválidos",

            HttpStatusCode.NotFound =>
                "Recurso não encontrado",

            HttpStatusCode.Conflict =>
                "Conflito de dados",

            _ =>
                "Erro interno"
        };
    }
}