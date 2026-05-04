// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.BlazorIcons.Lucide;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ServiceCollectionExtensions {
    public static IServiceCollection AddLucideIcons(this IServiceCollection services) {
        services.TryAddSingleton<ILucideService, LucideService>();
        services.TryAddSingleton<ILucideDataProvider, LucideDataProvider>();
        return services;
    }
}
