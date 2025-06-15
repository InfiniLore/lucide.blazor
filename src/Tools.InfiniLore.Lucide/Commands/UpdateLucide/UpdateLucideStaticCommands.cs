// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;
using CodeOfChaos.GeneratorTools;
using InfiniLore.Lucide.SourceGenerators.Dtos;
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
        var autoVersionUpdate = provider.GetRequiredService<AutoVersionUpdateLibrary>();
        var gatherTestData = provider.GetRequiredService<GatherTestDataLibrary>();
        var generateRazor = provider.GetRequiredService<GenerateRazorLibrary>();
        var updateLucideStatic = provider.GetRequiredService<UpdateLucideStaticLibrary>();
        var git = provider.GetRequiredService<GitLibrary>();
        var logger = provider.GetRequiredService<ILogger<UpdateLucideStaticCommands>>();
        
        await GlobalCatcher.ExecuteWithGlobalExceptionHandlingAsync(async () => {
            
            #region  Stage 1 : Update lucide in package.json
            string latestVersionNumber = await updateLucideStatic.TryGetLatestVersionNumber();
            if (latestVersionNumber.IsNullOrWhiteSpace()) {
                logger.Error("Could not retrieve latest version number");
                if (args.Strict)return;
            }
            logger.Information("Lucide Latest version is {version}", latestVersionNumber);

            bool resultUpdatePackageJson = await updateLucideStatic.TryUpdatePackageJson(latestVersionNumber);
            if (!resultUpdatePackageJson) {
                logger.Error("Could not update package.json");
                if (args.Strict) return;
            }

            bool resultNpmInstall = await updateLucideStatic.TryRunNpmInstall(args);
            if (!resultNpmInstall) {
                logger.Error("Could not run npm install");
                if (args.Strict) return;
            }
            #endregion
            
            #region Stage2 : Generate Razor
            if (!generateRazor.TrySetupOutputFolder()) {
                logger.Error("Could not setup output folder");
                if (args.Strict) return;
            }
            
            string[] filePaths = generateRazor.GetFiles();
            if (filePaths.Length == 0) {
                logger.Error("No files found");
                if (args.Strict) return;
            }
            
            LucideSvgFileDto[] fileDtos = await generateRazor.GetFileDtosAsync(filePaths);
            if (fileDtos.Length == 0) {
                logger.Error("No svg files found");
                if (args.Strict) return;
            }
            logger.Information("Found {count} svg files", fileDtos.Length);
            await Parallel.ForEachAsync(fileDtos, generateRazor.CreateRazorFileAsync);
            #endregion
            
            #region Stage 3 : Update TestConfig.json
            // int iconAmount = await gatherTestData.GatherIconAmountAsync();
            // if (iconAmount == -1) {
            //     logger.Error("Could not retrieve icon amount");
            //     if (args.Strict) return;
            // }
    
            // logger.Information("Found {count} icons on Lucide.Dev website", iconAmount);
            
            string[] iconsNames = gatherTestData.GetIconNames();
            if (iconsNames.Length == 0) {
                logger.Error("No icons found");
                if (args.Strict) return;
            }

            // if (iconsNames.Length < iconAmount) {
            //     logger.Error("Icon amount does not match icon names");
            //     if (args.Strict) return;
            // }

            // if (iconsNames.Length > iconAmount) {
            //     logger.Warning("Icon amount does not match icon names, this is most likely due to the website being outdated");
            //     iconAmount = iconsNames.Length;
            // }
        
            var data = new TestData {
                IconAmount = iconsNames.Length,
                IconNames = iconsNames
            };
            await gatherTestData.SaveDataToTestConfigAsync(data);
            logger.Information("Saved testconfig.json to Tests.InfiniLore.Lucide");
            #endregion
            
            #region Stage 4 : Commit changes
            bool resultCommitChanges = await git.CommitChanges(latestVersionNumber);
            if (!resultCommitChanges) {
                logger.Error("Could not commit changes");
                if (args.Strict) return;
            }
            logger.Information("Committed changes to git");
            #endregion
            
            #region Stage 5 : Update Version
            string currentVersion = await autoVersionUpdate.GetCurrentVersionAsync();
            string newVersionPrefix = string.Join('.', currentVersion.Split('.')[..2]);
            string newLucideVersion = latestVersionNumber.Split('.')[1];
            string newVersion = $"{newVersionPrefix}.{newLucideVersion}";
            
            await git.AutomateVersionBumpAsync(newVersion);
            #endregion
            
            logger.Information("Do not forget to run {scriptName} to version the commit and automagically create a nuget package", "`Version: Manual`");
            logger.Information("All Done!");
        });
    }
}
