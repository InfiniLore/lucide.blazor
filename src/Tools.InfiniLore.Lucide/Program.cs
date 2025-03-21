// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;
using CodeOfChaos.CliArgsParser.Library;
using Tools.InfiniLore.Lucide.Commands.GenerateRazor;
using Tools.InfiniLore.Lucide.Commands.UpdateLucide;

namespace Tools.InfiniLore.Lucide;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class Program {
    public static async Task Main(string[] args) {
        // Register & Build the parser
        //      Don't forget to add the current assembly if you built more tools for the current project
        CliArgsParser parser = CliArgsBuilder.CreateFromConfig(
            config => {
                config.AddCommandsFromAssemblyEntrypoint<IAssemblyEntry>();
                config.AddCommand<GenerateRazorCommand>();
                config.AddCommand<UpdateLucideStaticCommands>();
            }
        ).Build();

        // We are doing this here because else the launchSettings.json file becomes a humongous issue to deal with.
        //      Sometimes CLI params is not the answer.
        //      Code is the true saviour
        string projects = string.Join(";",
            "InfiniLore.Lucide",
            "InfiniLore.Lucide.Data",
            "InfiniLore.Lucide.Generators.Raw"
        );

        string oneLineArgs = InputHelper.ToOneLine(args).Replace("%PROJECTS%", projects);

        // Finally start executing
        await parser.ParseAsync(oneLineArgs);
    }
}
