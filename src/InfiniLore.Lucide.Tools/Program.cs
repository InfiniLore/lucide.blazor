// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

using CliArgsParser;
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
                .AddFromType<HelloAtlas>()
        );
        
        ServiceProvider provider = serviceCollection.BuildServiceProvider();
        
        var cliParser =  provider.GetRequiredService<ICliParser>();
        await cliParser.StartParsingAsync();
    }
}
