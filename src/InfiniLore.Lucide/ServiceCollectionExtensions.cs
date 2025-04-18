// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide;
using InfiniLore.Lucide.Data;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ServiceCollectionExtensions {
    public static IServiceCollection AddLucideIcons(this IServiceCollection services) {
        services.AddSingletonIfNotExists<ILucideService, LucideService>();
        services.AddSingletonIfNotExists<ILucideLookupDictionary, LucideLookupDictionary>();
        return services;
    }
}
