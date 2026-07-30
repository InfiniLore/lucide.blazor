// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.BlazorIcons.Lucide.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Text;
using System.Text.Json;

namespace CodeOfChaos.BlazorIcons.Lucide.SourceGenerators.Dtos;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record LucideSvgFileDto(string Name, string SvgContent) {
    public string PascalCaseName => Name.ToPascalCase();
    public string NormalizedName => Name.ToPascalCase().ToLowerInvariant();

    public static ImmutableArray<LucideSvgFileDto> FromIconNodes(AdditionalText file, CancellationToken ct = default) {
        if (file == null) throw new ArgumentNullException(nameof(file));

        // Read and parse the JSON file content
        string jsonContent = file.GetText(ct)?.ToString() ?? throw new InvalidOperationException($"Cannot read content from {file.Path}");

        // Deserialize JSON into a dictionary
        var iconData = JsonSerializer.Deserialize<Dictionary<string, List<List<object>>>>(
            jsonContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        // Validate deserialization
        if (iconData == null) {
            throw new InvalidOperationException($"Invalid JSON structure for file: {file.Path}");
        }

        // Storage for result
        ImmutableArray<LucideSvgFileDto>.Builder result = ImmutableArray.CreateBuilder<LucideSvgFileDto>();

        // Process each icon in the JSON file
        foreach (KeyValuePair<string, List<List<object>>> pair in iconData) {
            string? iconName = pair.Key;
            List<List<object>>? elements = pair.Value;
            
            var svgBuilder = new StringBuilder();

            // Begin SVG build
            foreach (List<object>? element in elements) {
                // Each element must have a type (first item) and attributes (second item)
                if (element.Count != 2 || element[1] is not JsonElement attributes) continue;

                string elementType = element[0]?.ToString() ?? string.Empty;

                if (string.IsNullOrEmpty(elementType)) continue;

                // Append the element, starting with its type
                svgBuilder.Append($"<{elementType}");

                // Handle attributes
                if (attributes.ValueKind == JsonValueKind.Object) {
                    foreach (JsonProperty attribute in attributes.EnumerateObject()) {
                        svgBuilder.Append($" {attribute.Name}=\"{attribute.Value}\"");
                    }
                }

                svgBuilder.Append(" />");
            }

            // Close the SVG tag

            // Add the generated SVG to results
            result.Add(new LucideSvgFileDto(iconName.ToPascalCase(), svgBuilder.ToString()));
        }

        // Return the final result as an ImmutableArray
        return result.ToImmutableArray();
    }
}
