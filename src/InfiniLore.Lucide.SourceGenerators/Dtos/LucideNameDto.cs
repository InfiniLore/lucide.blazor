// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace InfiniLore.Lucide.SourceGenerators.Dtos;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record LucideNameDto(string Name) {
    public string PascalCaseName => Name.ToPascalCase();
    
    public static ImmutableArray<LucideNameDto> FromIconNodes(AdditionalText file, CancellationToken ct = default) {
        if (file == null) throw new ArgumentNullException(nameof(file));

        // Read and parse the JSON file content
        string jsonContent = file.GetText(ct)?.ToString() ?? throw new InvalidOperationException($"Cannot read content from {file.Path}");

        // Deserialize JSON into a dictionary
        var iconData = JsonSerializer.Deserialize<Dictionary<string, List<List<object>>>>(
            jsonContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        // Validate deserialization
        if (iconData == null) throw new InvalidOperationException($"Invalid JSON structure for file: {file.Path}");

        // Storage for result
        return iconData
            .Select(pair => pair.Key)
            .Select(iconName => new LucideNameDto(iconName))
            .ToImmutableArray();
    }
}
