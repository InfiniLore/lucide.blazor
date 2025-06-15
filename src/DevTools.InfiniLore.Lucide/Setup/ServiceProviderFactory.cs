// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using DevTools.InfiniLore.Lucide.Library;
using DevTools.InfiniLore.Lucide.Library.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DevTools.InfiniLore.Lucide.Setup;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ServiceProviderFactory {
    public static IServiceProvider CreateProvider(IUpdateLucideParameters parameters) {
        var services = new ServiceCollection();

        // Add Serilog to the services
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .AsAnnaSasDevServerConsole()
            .CreateLogger();
        
        services.AddLogging(static loggingBuilder =>
            loggingBuilder.AddSerilog(Log.Logger, true));


        services.AddSingleton(parameters);
        services.AddSingleton<AutoVersionUpdateLibrary>();
        services.AddSingleton<GatherTestDataLibrary>();
        services.AddSingleton<GenerateRazorLibrary>();
        services.AddSingleton<UpdateLucideStaticLibrary>();
        services.AddSingleton<GitLibrary>();
        
        return services.BuildServiceProvider();
    }
}
