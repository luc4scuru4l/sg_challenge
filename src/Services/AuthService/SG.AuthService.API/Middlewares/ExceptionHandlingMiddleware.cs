using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SG.AuthService.Application.Exceptions;
using SG.AuthService.Domain.Exceptions;

namespace SG.AuthService.API.Middlewares;

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
      _logger.LogError(ex, "Excepción capturada por el middleware global.");
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
      case InvalidCredentialsException ex:
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        response.Status = StatusCodes.Status401Unauthorized;
        response.Title = "Unauthorized";
        response.Detail = ex.Message;
        break;
      case UserAlreadyExistsException ex:
        context.Response.StatusCode = StatusCodes.Status409Conflict;
        response.Status = StatusCodes.Status409Conflict;
        response.Title = "Conflict User";
        response.Detail = ex.Message;
        break;

      case InvalidUserException ex:
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        response.Status = StatusCodes.Status400BadRequest;
        response.Title = "Invalid User or Password";
        response.Detail = ex.Message;
        break;
      
      case InvalidPasswordException ex:
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        response.Status = StatusCodes.Status400BadRequest;
        response.Title = "Invalid Password";
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

    context.Response.StatusCode = response.Status ?? StatusCodes.Status500InternalServerError;
    
    var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    var json = JsonSerializer.Serialize(response, jsonOptions);

    await context.Response.WriteAsync(json);
  }
}