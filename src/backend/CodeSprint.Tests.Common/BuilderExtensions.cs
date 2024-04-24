using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeSprint.Tests.Common;

public static class BuilderExtensions
{
    public static IServiceCollection Replace<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime)
        where TService : class
        where TImplementation : class, TService
    {
        return services.Replace(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
    }

    public static IServiceCollection Replace<TService>(this IServiceCollection services, object implementation, ServiceLifetime lifetime)
        where TService : class
    {
        return services.Replace(new ServiceDescriptor(typeof(TService), _ => implementation, ServiceLifetime.Scoped));
    }
}