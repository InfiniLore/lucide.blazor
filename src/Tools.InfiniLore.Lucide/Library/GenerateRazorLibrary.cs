// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.GeneratorTools;
using InfiniLore.Lucide.SourceGenerators.Dtos;
using InfiniLore.Lucide.SourceGenerators.Helpers;
using Microsoft.Extensions.Logging;
using Tools.InfiniLore.Lucide.Library.Contracts;

namespace Tools.InfiniLore.Lucide.Library;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class GenerateRazorLibrary(IUpdateLucideParameters parameters, ILogger<GenerateRazorLibrary> logger) {
    private readonly string[] _lucideLicence = GeneratorStringBuilderExtensions.GetLucideLicense();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public string[] GetFiles() {
        string expectedIconsFolder = parameters.AppendRoot("node_modules/lucide-static/icons");
        if (!Directory.Exists(expectedIconsFolder)) {
            logger.Warning("Could not find lucide-static icons folder at the following path: {path}", Path.GetFullPath(expectedIconsFolder));
            return [];
        }

        string[] files = Directory.GetFiles(expectedIconsFolder, "*.svg");
        logger.Information("Found {count} files in lucide-static icons folder", files.Length);
        return files;
    }

    public bool TrySetupOutputFolder() {
        try {
            if (!Directory.Exists(parameters.RazorOutputFolder)) {
                Directory.CreateDirectory(parameters.RazorOutputFolder);
                logger.Information("Created output folder: {path}", Path.GetFullPath(parameters.RazorOutputFolder));
                return true;
            }

            Directory.Delete(parameters.RazorOutputFolder, true);
            Directory.CreateDirectory(parameters.RazorOutputFolder);
            logger.Information("Deleted and created new output folder: {path}", Path.GetFullPath(parameters.RazorOutputFolder));
            return true;
        }
        catch (Exception e) {
            logger.Error(e, "Could not setup output folder: {path}", Path.GetFullPath(parameters.RazorOutputFolder));
            return false;
        }
    }

    public async Task<LucideSvgFileDto[]> GetFileDtosAsync(string[] paths) {
        var dtos = new LucideSvgFileDto[paths.Length];

        for (int i = 0; i < paths.Length; i++) {
            string path = paths[i];

            try {
                string svg = await File.ReadAllTextAsync(path);
                LucideSvgFileDto dto = new(Path.GetFileNameWithoutExtension(path), svg);
                dtos[i] = dto;
            }
            catch (Exception e) {
                logger.Error(e, "Could not read file: {path}", Path.GetFullPath(path));
                return [];
            }
        }

        return dtos;
    }

    public async ValueTask CreateRazorFileAsync(LucideSvgFileDto dto, CancellationToken ct) {
        var builder = new GeneratorStringBuilder();

        builder
            .ForEachAppendLine(_lucideLicence, itemFormatter: line => $"@* {line} *@")
            .AppendLine("@namespace InfiniLore.Lucide")
            .AppendLine("@inherits LucideComponentBase")
            .AppendBody("""
                <svg class="@Class"
                     xmlns="http://www.w3.org/2000/svg"
                     width="@Width"
                     height="@Height"
                     viewBox="0 0 24 24"
                     fill="@Fill"
                     stroke="@Stroke"
                     stroke-width="@StrokeWidth"
                     stroke-linecap="@StrokeLineCap"
                     stroke-linejoin="@StrokeLineJoin">
                """)
            .AppendBodyIndented(dto.SvgContent)
            .AppendLine("</svg>");

        // Output data to the actual file
        string filePath = Path.Combine(parameters.RazorOutputFolder, $"Li{dto.PascalCaseName}.razor");
        await File.WriteAllTextAsync(filePath, builder.ToString(), ct);
        logger.Information("Created razor file: {path}", Path.GetFullPath(filePath));
    }
}
