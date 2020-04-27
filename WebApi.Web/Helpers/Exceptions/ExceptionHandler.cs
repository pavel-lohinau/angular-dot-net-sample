using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebApi.Web.Helpers.Exceptions
{
  public class ExceptionHandler
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandler> _logger;
    private readonly IMapper _mapper;
    private const string ContentType = "application/json; charset=utf-8";

    public ExceptionHandler(
      RequestDelegate next,
      ILoggerFactory loggerFactory,
      IMapper mapper)
    {
      _next = next;
      _logger = loggerFactory.CreateLogger<ExceptionHandler>();
      _mapper = mapper;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
      try
      {
        await _next(httpContext).ConfigureAwait(false);
      }
      catch (Exception exception)
      {
        if (httpContext.Response.HasStarted)
        {
          _logger.LogWarning("The response has already started, the http status code middleware will not be executed.");
          throw;
        }

        await HandleExceptionAsync(httpContext, exception).ConfigureAwait(false);
      }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
      var code = HttpStatusCode.InternalServerError;

      if (exception is UnauthorizedAccessException)
          code = HttpStatusCode.Unauthorized;
      if (exception is KeyNotFoundException)
          code = HttpStatusCode.BadRequest;

      var response = _mapper.Map<ErrorModel>(exception);

      var message = JsonConvert.SerializeObject(response);

      _logger.LogError(message);

      httpContext.Response.Clear();
      httpContext.Response.StatusCode = (int)code;
      httpContext.Response.ContentType = ExceptionHandler.ContentType;

      await httpContext.Response.WriteAsync(message).ConfigureAwait(false);
    }
  }
}
