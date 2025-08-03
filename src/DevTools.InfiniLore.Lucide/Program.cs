// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;
using CodeOfChaos.CliArgsParser.Library;
using DevTools.InfiniLore.Lucide.Commands.UpdateLucide;

namespace DevTools.InfiniLore.Lucide;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class Program {
    public static async Task Main(string[] args) {
        // Register & Build the parser
        //      Don't forget to add the current assembly if you built more tools for the current project
        ICliParser parser = CliParser.CreateBuilder()
            .AddFromAssembly<IAssemblyEntry>()
            .AddFromAssembly(typeof(UpdateLucideStaticCommands).Assembly)
            .Build();

        // We are doing this here because else the launchSettings.json file becomes a humongous issue to deal with.
        //      Sometimes CLI params are not the answer.
        //      Code is the true savior
        string projects = string.Join(";",
            "InfiniLore.Lucide",
            "InfiniLore.Lucide.SourceGenerators"
        );

        // Finally, start executing
        string oneLineArgs = ArgsInputHelper.ToOneLine(args).Replace("%PROJECTS%", projects);
        await parser.ExecuteAsync(oneLineArgs);
    }
}
