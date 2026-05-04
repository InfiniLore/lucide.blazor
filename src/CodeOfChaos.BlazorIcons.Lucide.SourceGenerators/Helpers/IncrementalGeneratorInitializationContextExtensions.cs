// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.BlazorIcons.Lucide.SourceGenerators.Dtos;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.IO;

namespace CodeOfChaos.BlazorIcons.Lucide.SourceGenerators.Helpers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IncrementalGeneratorInitializationContextExtensions {
    private static IncrementalValuesProvider<AdditionalText> FilterLucideIconNodes(this IncrementalGeneratorInitializationContext context)
        => context.AdditionalTextsProvider
            .Where(file => Path.GetFileName(file.Path) == "icon-nodes.json");
    
    public static IncrementalValueProvider<ImmutableArray<LucideSvgFileDto>> CollectLucideIcons(this IncrementalGeneratorInitializationContext context) 
        => context.FilterLucideIconNodes()
            .SelectMany(LucideSvgFileDto.FromIconNodes)
            .Collect();
    
    public static IncrementalValueProvider<ImmutableArray<LucideNameDto>> CollectManyLucideIcons(this IncrementalGeneratorInitializationContext context)
        => context.FilterLucideIconNodes()
            .SelectMany(LucideNameDto.FromIconNodes)
            .Collect();
}
