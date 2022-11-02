namespace Imagegram.Api.ExceptionHandling;

public static class ExceptionProblemDetailsBuilderDependencyExtension
{
    public static IServiceCollection AddExceptionToHttpStatusCodeMapping(
        this IServiceCollection services, 
        Action<ExceptionProblemDetailsBuilder> configure)
    {
        var builder = new ExceptionProblemDetailsBuilder();

        configure(builder);

        services.AddSingleton(builder);

        return services;
    }
}