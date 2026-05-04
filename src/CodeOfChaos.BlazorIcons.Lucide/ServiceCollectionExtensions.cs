// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.BlazorIcons.Lucide;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ServiceCollectionExtensions {
    public static IServiceCollection AddLucideIcons(this IServiceCollection services) {
        services.AddSingletonIfNotExists<ILucideService, LucideService>();
        services.AddSingletonIfNotExists<ILucideDataProvider, LucideDataProvider>();
        return services;
    }
}
