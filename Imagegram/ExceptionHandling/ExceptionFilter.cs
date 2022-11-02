using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Imagegram.ExceptionHandling;

// Just a lightweight exception to http status code mapper
public class ExceptionFilter : IExceptionFilter
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ExceptionProblemDetailsBuilder _problemDetailsBuilder;

    public ExceptionFilter(IHostEnvironment hostEnvironment, ExceptionProblemDetailsBuilder problemDetailsBuilder)
    {
        _hostEnvironment = hostEnvironment;
        _problemDetailsBuilder = problemDetailsBuilder;
    }

    public void OnException(ExceptionContext context)
    {
        var problemDetails = _problemDetailsBuilder.BuildFromException(context.Exception);

        if (problemDetails is not null)
            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
    }
}