namespace Imagegram.ExceptionHandler;

public static class ExceptionProblemDetailsBuilderDependencyExtension
{
    public static IServiceCollection AddExceptionToHttpStatusCodeMapping(
        this IServiceCollection services, 
        Action<ExceptionProblemDetailsBuilder> configureProblemDetails)
    {
        var builder = new ExceptionProblemDetailsBuilder();

        configureProblemDetails(builder);

        services.AddSingleton(builder);

        return services;
    }
}