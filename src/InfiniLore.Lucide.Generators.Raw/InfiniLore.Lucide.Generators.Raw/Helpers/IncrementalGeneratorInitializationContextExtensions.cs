// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;

namespace InfiniLore.Lucide.Generators.Raw.Helpers;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IncrementalGeneratorInitializationContextExtensions {

    private const string RegexIconSvgPath = @"lucide-static\\icons\\.*\.svg$";

    private static IncrementalValuesProvider<LucideSvgFile> SelectLucideSvgFiles(this IncrementalGeneratorInitializationContext context) {
        IncrementalValuesProvider<AdditionalText> files = context.AdditionalTextsProvider
            .Where(file => Regex.IsMatch(file.Path, RegexIconSvgPath));

        return files
            .Select((file, cancellationToken) => new LucideSvgFile(
                Name: Path.GetFileNameWithoutExtension(file.Path),
                Svg: file.GetText(cancellationToken)?.ToString() ?? string.Empty
            ));
    }
    
    public static IncrementalValueProvider<ImmutableArray<LucideSvgFile>> CollectLucideSvgFiles(this IncrementalGeneratorInitializationContext context) {
        return context.SelectLucideSvgFiles().Collect();
    }
}
