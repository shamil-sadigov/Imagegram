using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Imagegram.ExceptionHandling;

public sealed class ExceptionProblemDetailsBuilder
{
    private readonly Dictionary<Type, HttpStatusCode> _mapper = new();

    public ExceptionProblemDetailsBuilder Map<TException>(HttpStatusCode httpStatusCode)
        where TException: Exception
    {
        var exceptionType = typeof(TException);

        if (_mapper.ContainsKey(exceptionType))
        {
            throw new InvalidOperationException(
                $"Exception of type '{exceptionType} was already registered in mapping'");
        }

        _mapper.Add(exceptionType, httpStatusCode);

        return this;
    }

    public ProblemDetails? BuildFromException<TException>(TException exception)
        where TException:Exception
    {
        if (!_mapper.TryGetValue(exception.GetType(), out HttpStatusCode httpStatusCode))
        {
            return null;
        }

        return new ProblemDetails()
        {
            Status = (int)httpStatusCode,
            Detail = exception.Message,
        };
    }
}