// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using CliArgsParser;
using InfiniLore.Lucide.Tools.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniLore.Lucide.Tools;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class Program {
    public async static Task Main(string[] args) {
        IServiceCollection serviceCollection = new ServiceCollection();

        serviceCollection.AddCliArgsParser(configuration =>
            configuration
                .SetConfig(new CliArgsParserConfig {
                    Overridable = true,
                    GenerateShortNames = true
                })
                .AddFromType<UpdateLucideStaticCommands>()
        );

        ServiceProvider provider = serviceCollection.BuildServiceProvider();

        var argsParser = provider.GetRequiredService<IArgsParser>();
        await argsParser.ParseAsyncLinear(args);
    }
}
