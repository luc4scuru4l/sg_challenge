using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SG.AccountService.Domain.Exceptions;
using SG.AccountService.Application.Exceptions;

namespace SG.AccountService.API.Middlewares;

public class ExceptionHandlingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ExceptionHandlingMiddleware> _logger;

  public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
    catch (Exception ex)
    {
      await HandleExceptionAsync(context, ex);
    }
  }

  private async Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    context.Response.ContentType = "application/json";
    var response = new ProblemDetails
    {
      Status = StatusCodes.Status500InternalServerError,
      Title = "Server Error",
      Detail = "Ocurrió un error inesperado.",
      Instance = context.Request.Path
    };

    switch (exception)
    {
      case InvalidAmountException ex:
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        response.Status = StatusCodes.Status400BadRequest;
        response.Title = "Invalid Transaction Amount";
        response.Detail = ex.Message;
        break;
      case InsufficientFundsException ex:
        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        response.Status = StatusCodes.Status422UnprocessableEntity;
        response.Title = "Business Validation Error";
        response.Detail = ex.Message;
        break;

      case AccountNotFoundException ex:
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        response.Status = StatusCodes.Status404NotFound;
        response.Title = "Resource Not Found";
        response.Detail = ex.Message;
        break;

      case DomainException ex:
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        response.Status = StatusCodes.Status400BadRequest;
        response.Title = "Bad Request";
        response.Detail = ex.Message;
        break;

      default:
        _logger.LogError(exception, "Error no controlado en el servidor");
        break;
    }

    var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    var json = JsonSerializer.Serialize(response, jsonOptions);

    await context.Response.WriteAsync(json);
  }
}