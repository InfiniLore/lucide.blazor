// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;
using CodeOfChaos.GeneratorTools;
using InfiniLore.Lucide.Generators.Raw.Dtos;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tools.InfiniLore.Lucide.Setup;

namespace Tools.InfiniLore.Lucide.Commands.GenerateRazor;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
[CliArgsCommand("generate-razor")]
public partial class GenerateRazorCommand : ICommand<GenerateRazorParameters>  {
    private IServiceProvider Provider { get; } = ServiceProviderFactory.CreateProvider();
    private ILogger<GenerateRazorCommand>? _loggerCache;
    private ILogger<GenerateRazorCommand> Logger => _loggerCache ??= Provider.GetRequiredService<ILogger<GenerateRazorCommand>>();
    private GenerateRazorParameters Parameters { get; set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task ExecuteAsync(GenerateRazorParameters parameters) {
        Parameters = parameters;
        await GlobalCatcher.ExecuteWithGlobalExceptionHandlingAsync(async () => {
            if (!TrySetupOutputFolder(parameters)) {
                Logger.Critical("Could not setup output folder");
                return;
            }
            
            string[] filePaths = GetFiles(parameters);
            if (filePaths.Length == 0) {
                Logger.Critical("No files found");
                return;
            }
            
            LucideSvgFileDto[] fileDtos = await GetFileDtosAsync(filePaths);
            if (fileDtos.Length == 0) {
                Logger.Critical("No svg files found");
                return;
            }
            Logger.Information("Found {count} svg files", fileDtos.Length);
            await Parallel.ForEachAsync(fileDtos, CreateRazorFileAsync);
        });
    }

    private string[] GetFiles(GenerateRazorParameters parameters) {
        string expectedIconsFolder = parameters.AppendRoot("node_modules/lucide-static/icons");
        if (!Directory.Exists(expectedIconsFolder)) {
            Logger.Warning("Could not find lucide-static icons folder at the following path: {path}", Path.GetFullPath(expectedIconsFolder));
            return [];
        }
        
        string[] files = Directory.GetFiles(expectedIconsFolder, "*.svg");
        Logger.Information("Found {count} files in lucide-static icons folder", files.Length);
        return files;
    }

    private bool TrySetupOutputFolder(GenerateRazorParameters parameters) {
        try {
            if (!Directory.Exists(parameters.OutputFolder)) {
                Directory.CreateDirectory(parameters.OutputFolder);
                Logger.Information("Created output folder: {path}", Path.GetFullPath(parameters.OutputFolder));
                return true;
            }
        
            Directory.Delete(parameters.OutputFolder, true);
            Directory.CreateDirectory(parameters.OutputFolder);
            Logger.Information("Deleted and created new output folder: {path}", Path.GetFullPath(parameters.OutputFolder));
            return true;
        }
        catch (Exception e) {
            Logger.Error(e, "Could not setup output folder: {path}", Path.GetFullPath(parameters.OutputFolder));
            return false;
        }
    }

    private async Task<LucideSvgFileDto[]> GetFileDtosAsync(string[] paths) {
        var dtos = new LucideSvgFileDto[paths.Length];

        for (int i = 0; i < paths.Length; i++) {
            string path = paths[i];

            try {
                string svg = await File.ReadAllTextAsync(path);
                LucideSvgFileDto dto = new(Path.GetFileNameWithoutExtension(path), svg);
                dtos[i] = dto;
            }
            catch (Exception e) {
                Logger.Error(e, "Could not read file: {path}", Path.GetFullPath(path));
                return [];
            }
        }
        
        return dtos;
    }
    
    private async ValueTask CreateRazorFileAsync(LucideSvgFileDto dto, CancellationToken ct) {
        var builder = new GeneratorStringBuilder();

        builder.AppendLine("@inherits ComponentBase");
        builder.AppendBody("""
            <svg xmlns="http://www.w3.org/2000/svg"
                 width="@Width"
                 height="@Height"
                 viewBox="0 0 24 24"
                 fill="@Fill"
                 stroke="@Stroke"
                 stroke-width="@StrokeWidth"
                 stroke-linecap="@StrokeLineCap"
                 stroke-linejoin="@StrokeLineJoin"
                 @attributes="AdditionalAttributes">
            """);
        builder.AppendBodyIndented(dto.Svg);
        builder.AppendLine("</svg>");
        builder.AppendLine();
        builder.AppendLine("@code {");
        builder.Indent(b => {
            b.AppendLine("[Parameter] public int Width { get; set; } = 24;");
            b.AppendLine("[Parameter] public int Height { get; set; } = 24;");
            b.AppendLine("[Parameter] public string Fill { get; set; } = \"none\";");
            b.AppendLine("[Parameter] public string Stroke { get; set; } = \"currentColor\";");
            b.AppendLine("[Parameter] public int StrokeWidth { get; set; } = 2;");
            b.AppendLine("[Parameter] public string StrokeLineCap { get; set; } = \"round\";");
            b.AppendLine("[Parameter] public string StrokeLineJoin { get; set; } = \"round\";");
            b.AppendLine("[Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; } = null!;");
        });
        builder.AppendLine("}");
        
        // Output data to the actual file
        string filePath = Path.Combine(Parameters.OutputFolder, dto.Name + ".razor");
        await File.WriteAllTextAsync(filePath, builder.ToString(), ct);
        Logger.Information("Created razor file: {path}", Path.GetFullPath(filePath));
    }
}
