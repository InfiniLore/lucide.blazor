// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaosTools.BlazorIcons.Lucide.Library.Contracts;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace CodeOfChaosTools.BlazorIcons.Lucide.Library;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class AutoVersionUpdateLibrary(ILogger<AutoVersionUpdateLibrary> logger, IUpdateLucideParameters parameters) {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task<string> GetCurrentVersionAsync() {
        string referenceCsproj = parameters.AppendRoot("src/CodeOfChaos.BlazorIcons.Lucide/CodeOfChaos.BlazorIcons.Lucide.csproj");

        await using var stream = new FileStream(referenceCsproj, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
        XDocument document = await XDocument.LoadAsync(stream, LoadOptions.PreserveWhitespace, CancellationToken.None);
        XElement? versionElement = document
            .Descendants("PropertyGroup")
            .Elements("Version")
            .FirstOrDefault();
        
        // ReSharper disable once InvertIf
        if (versionElement is null) {
            logger.Error("Could not find version element in csproj file");
            return string.Empty;
        }
        
        return versionElement.Value;
    }
}
