using Microsoft.AspNetCore.Builder;

namespace WebApi.Web.Helpers.Exceptions
{
  public static class ExceptionHandlingMiddlewareExtensions
  {
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<ExceptionHandler>();
    }
  }
}
