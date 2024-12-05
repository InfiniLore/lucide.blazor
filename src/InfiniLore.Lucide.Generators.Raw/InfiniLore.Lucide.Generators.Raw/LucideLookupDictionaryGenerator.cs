// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide.Generators.Raw.Helpers;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Text;

namespace InfiniLore.Lucide.Generators.Raw;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[Generator(LanguageNames.CSharp)]
public class LucideLookupDictionaryGenerator : IIncrementalGenerator{

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        context.RegisterSourceOutput(context.CollectLucideSvgFiles(), CreateIconFiles);
    }

    private static void CreateIconFiles(SourceProductionContext context, ImmutableArray<LucideSvgFile> data) {
        var builder = new StringBuilder();
        
        builder
            .AppendLine("using System.Collections.Generic;")
            .AppendLine("namespace InfiniLore.Lucide.Generators;")
            .AppendLine("public partial class LucideLookupDictionary {");
        
        builder.IndentLine(1, "public static readonly Dictionary<string, ILucideIconData> IconsByLucideName = new() {");
        foreach (LucideSvgFile lucideSvgFile in data) {
            builder.IndentLine(2, $"[\"{lucideSvgFile.Name}\"] = new {lucideSvgFile.PascalCaseName}(),");
        }
        builder.IndentLine(1,"};");
        
        builder.IndentLine(1, "public static readonly Dictionary<string, string> IconsByPascalCase = new() {");
        foreach (LucideSvgFile lucideSvgFile in data) {
            builder.IndentLine(2, $"[\"{lucideSvgFile.PascalCaseName}\"] = \"{lucideSvgFile.Name}\",");
        }
        builder.IndentLine(1,"};");
        
        builder.IndentLine(1, "public static readonly Dictionary<string, string> IconsByCamelCase = new() {");
        foreach (LucideSvgFile lucideSvgFile in data) {
            builder.IndentLine(2, $"[\"{lucideSvgFile.CamelCaseName}\"] = \"{lucideSvgFile.Name}\",");
        }
        builder.IndentLine(1,"};");
        
        builder.IndentLine(1, "public static readonly Dictionary<string, string> IconsByPascalLowerInvariant = new() {");
        foreach (LucideSvgFile lucideSvgFile in data) {
            builder.IndentLine(2, $"[\"{lucideSvgFile.PascalCaseName.ToLowerInvariant()}\"] = \"{lucideSvgFile.Name}\",");
        }
        builder.IndentLine(1,"};");
        
        builder.AppendLine("}");
        
        context.AddSource("LucideLookupDictionary.g.cs", builder.ToString());
    }
    
}
