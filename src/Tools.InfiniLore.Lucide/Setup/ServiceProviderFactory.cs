// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Tools.InfiniLore.Lucide.Setup;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ServiceProviderFactory {
    public static IServiceProvider CreateProvider() {
        var services = new ServiceCollection();

        // Add Serilog to the services
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .AsAnnaSasDevServerConsole()
            .CreateLogger();
        
        services.AddLogging(static loggingBuilder =>
            loggingBuilder.AddSerilog(Log.Logger, true));
        
        return services.BuildServiceProvider();
    }
}
