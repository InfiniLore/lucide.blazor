// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;
using CodeOfChaos.GeneratorTools;
using InfiniLore.Lucide.Generators.Raw.Dtos;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tools.InfiniLore.Lucide.Library;
using Tools.InfiniLore.Lucide.Library.Contracts;
using Tools.InfiniLore.Lucide.Setup;

namespace Tools.InfiniLore.Lucide.Commands.UpdateLucide;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
[CliArgsCommand("update-lucide-static")]
public partial class UpdateLucideStaticCommands : ICommand<UpdateLucideStaticParameters> {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task ExecuteAsync(UpdateLucideStaticParameters args) {
        IServiceProvider provider = ServiceProviderFactory.CreateProvider(args);
        var gatherTestData = provider.GetRequiredService<GatherTestDataLibrary>();
        var generateRazor = provider.GetRequiredService<GenerateRazorLibrary>();
        var updateLucideStatic = provider.GetRequiredService<UpdateLucideStaticLibrary>();
        var git = provider.GetRequiredService<GitLibrary>();
        var logger = provider.GetRequiredService<ILogger<UpdateLucideStaticCommands>>();
        
        await GlobalCatcher.ExecuteWithGlobalExceptionHandlingAsync(async () => {
            
            #region  Stage 1 : Update lucide in package.json
            string latestVersionNumber = await updateLucideStatic.TryGetLatestVersionNumber();
            if (latestVersionNumber.IsNullOrWhiteSpace()) {
                logger.Critical("Could not retrieve latest version number");
                if (args.Strict)return;
            }
            logger.Information("Lucide Latest version is {version}", latestVersionNumber);

            bool resultUpdatePackageJson = await updateLucideStatic.TryUpdatePackageJson(latestVersionNumber);
            if (!resultUpdatePackageJson) {
                logger.Critical("Could not update package.json");
                if (args.Strict) return;
            }

            bool resultNpmInstall = await updateLucideStatic.TryRunNpmInstall(args);
            if (!resultNpmInstall) {
                logger.Critical("Could not run npm install");
                if (args.Strict) return;
            }
            #endregion
            
            #region Stage2 : Generate Razor
            if (!generateRazor.TrySetupOutputFolder()) {
                logger.Critical("Could not setup output folder");
                if (args.Strict) return;
            }
            
            string[] filePaths = generateRazor.GetFiles();
            if (filePaths.Length == 0) {
                logger.Critical("No files found");
                if (args.Strict) return;
            }
            
            LucideSvgFileDto[] fileDtos = await generateRazor.GetFileDtosAsync(filePaths);
            if (fileDtos.Length == 0) {
                logger.Critical("No svg files found");
                if (args.Strict) return;
            }
            logger.Information("Found {count} svg files", fileDtos.Length);
            await Parallel.ForEachAsync(fileDtos, generateRazor.CreateRazorFileAsync);
            #endregion
            
            #region Stage 3 : Update TestConfig.json
            int iconAmount = await gatherTestData.GatherIconAmountAsync();
            if (iconAmount == -1) {
                logger.Critical("Could not retrieve icon amount");
                if (args.Strict) return;
            }
    
            logger.Information("Found {count} icons on Lucide.Dev website", iconAmount);
        
            var data = new TestData { TotalIcons = iconAmount };
            await gatherTestData.SaveDataToTestConfigAsync(data);
            logger.Information("Saved testconfig.json to Tests.InfiniLore.Lucide");
            #endregion
            
            #region Stage 4 : Commit changes
            bool resultCommitChanges = await git.CommitChanges(latestVersionNumber);
            if (!resultCommitChanges) {
                logger.Critical("Could not commit changes");
                if (args.Strict) return;
            }
            logger.Information("Committed changes to git");
            #endregion
            
            logger.Information("Do not forget to run {scriptName} to version the commit and automagically create a nuget package", "`Version: Manual`");
            logger.Information("All Done!");
        });
    }
}
