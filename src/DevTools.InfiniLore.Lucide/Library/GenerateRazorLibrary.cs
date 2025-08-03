// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.GeneratorTools;
using DevTools.InfiniLore.Lucide.Library.Contracts;
using InfiniLore.Lucide.SourceGenerators.Dtos;
using InfiniLore.Lucide.SourceGenerators.Helpers;
using Microsoft.Extensions.Logging;

namespace DevTools.InfiniLore.Lucide.Library;
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
        const int maxRetries = 3;
        const int delayMilliseconds = 500;

        var builder = new GeneratorStringBuilder();

        builder
            .ForEachAppendLine(_lucideLicence, itemFormatter: line => $"@* {line} *@")
            .AppendLine("@namespace InfiniLore.Lucide")
            .AppendLine("@inherits LucideComponentBase")
            .AppendBody("""
                <svg class="@Class"
                     xmlns="http://www.w3.org/2000/svg"
                     width="@Size"
                     height="@Size"
                     viewBox="0 0 24 24"
                     fill="@Fill"
                     stroke="@Stroke"
                     stroke-width="@StrokeWidth"
                     stroke-linecap="@StrokeLineCap"
                     stroke-linejoin="@StrokeLineJoin">
                """)
            .AppendBodyIndented(dto.SvgContent)
            .AppendLine("</svg>");

        // Ensure the directory exists
        string outputFolder = parameters.RazorOutputFolder;
        if (!Directory.Exists(outputFolder)) {
            Directory.CreateDirectory(outputFolder);
            logger.Information("Created missing output folder: {path}", Path.GetFullPath(outputFolder));
        }

        string filePath = Path.Combine(outputFolder, $"Li{dto.PascalCaseName}.razor");

        for (int attempt = 0; attempt < maxRetries; attempt++) {
            try {
                await File.WriteAllTextAsync(filePath, builder.ToString(), ct);
                logger.Information("Created razor file: {path}", Path.GetFullPath(filePath));
                return; // Exit the function on success
            }
            
            catch (IOException ex) when (++attempt <= maxRetries) {
                // Handle other IO-related issues and retry if allowed
                logger.Warning(ex, "Attempt {attempt}/{maxRetries} failed while writing the razor file: {dtoName}. Retrying...", attempt, maxRetries, dto.PascalCaseName);
                await Task.Delay(delayMilliseconds, ct); // Wait before retrying
            }
            
            catch (OperationCanceledException) {
                // Handle cancellation
                logger.Warning("File creation was canceled for DTO: {dtoName}", dto.PascalCaseName);
                throw;
            }
            
            catch (UnauthorizedAccessException ex) {
                // Handle file permission issues
                logger.Error(ex, "Insufficient permissions to write the razor file: {dtoName}", dto.PascalCaseName);
                throw; // No point in retrying
            }
            
            catch (Exception ex) {
                // General exception logging
                logger.Error(ex, "Failed to create the razor file: {dtoName}", dto.PascalCaseName);
                throw;
            }
        }
    }
}
