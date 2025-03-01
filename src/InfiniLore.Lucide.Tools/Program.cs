// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;
using CodeOfChaos.CliArgsParser.Library;

namespace InfiniLore.Lucide.Tools;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class Program {
    public async static Task Main(string[] args) {
        // Register & Build the parser
        //      Don't forget to add the current assembly if you built more tools for the current project
        CliArgsParser parser = CliArgsBuilder.CreateFromConfig(
            config => {
                config.AddCommandsFromAssemblyEntrypoint<IAssemblyEntry>();
            }
        ).Build();

        // We are doing this here because else the launchSettings.json file becomes a humongous issue to deal with.
        //      Sometimes CLI params is not the answer.
        //      Code is the true saviour
        string projects = string.Join(";",
            "InfiniLore.Lucide",
            "InfiniLore.Lucide.Generators",
            "InfiniLore.Lucide.Generators.Raw"
        );

        string oneLineArgs = InputHelper.ToOneLine(args).Replace("%PROJECTS%", projects);

        // Finally start executing
        await parser.ParseAsync(oneLineArgs);
    }
}
