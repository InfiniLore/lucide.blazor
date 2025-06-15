// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide.SourceGenerators.Dtos;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace InfiniLore.Lucide.SourceGenerators.Helpers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IncrementalGeneratorInitializationContextExtensions {
    private static IncrementalValuesProvider<AdditionalText> FilterLucideFiles(this IncrementalGeneratorInitializationContext context) 
        => context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith(".svg"));

    public static IncrementalValueProvider<ImmutableArray<LucideSvgFileDto>> CollectLucideSvgFiles(this IncrementalGeneratorInitializationContext context) 
        => context.FilterLucideFiles()
            .Select(LucideSvgFileDto.FromAdditionalText)
            .Collect();
    
    public static IncrementalValueProvider<ImmutableArray<LucideNameDto>> CollectLucideNames(this IncrementalGeneratorInitializationContext context)
        => context.FilterLucideFiles()
            .Select(LucideNameDto.FromAdditionalText)
            .Collect();
}
