// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.GeneratorTools;
using InfiniLore.Lucide.Generators.Raw.Helpers;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace InfiniLore.Lucide.Generators.Raw;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[Generator(LanguageNames.CSharp)]
public class LucideLookupDictionaryGenerator : IIncrementalGenerator {

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        context.RegisterSourceOutput(context.CollectLucideSvgFiles(), CreateIconFiles);
    }

    private static void CreateIconFiles(SourceProductionContext context, ImmutableArray<LucideSvgFile> data) {
        var builder = new GeneratorStringBuilder();

        builder
            .AppendLine("using System.Collections.Generic;")
            .AppendLine("namespace InfiniLore.Lucide.Generators;")
            .AppendLine("public partial class LucideLookupDictionary {");

        builder.Indent(b => {
            b.AppendLine("public static readonly Dictionary<string, ILucideIconData> IconsByLucideName = new() {");
            b.ForEachAppendLineIndented(data, itemFormatter: d => $"[\"{d.Name}\"] = new {d.PascalCaseName}(),");
            b.AppendLine("};");
        });

        builder.Indent(b => {
            b.AppendLine("public static readonly Dictionary<string, string> IconsByPascalCase = new() {");
            b.ForEachAppendLineIndented(data, itemFormatter: d => $"[\"{d.PascalCaseName}\"] = \"{d.Name}\",");
            b.AppendLine("};");
        });

        builder.Indent(b => {
            b.AppendLine("public static readonly Dictionary<string, string> IconsByCamelCase = new() {");
            b.ForEachAppendLineIndented(data, itemFormatter: d => $"[\"{d.CamelCaseName}\"] = \"{d.Name}\",");
            b.AppendLine("};");
        });

        builder.Indent(b => {
            b.AppendLine("public static readonly Dictionary<string, string> IconsByPascalLowerInvariant = new() {");
            b.ForEachAppendLineIndented(data, itemFormatter: d => $"[\"{d.PascalCaseName.ToLowerInvariant()}\"] = \"{d.Name}\",");
            b.AppendLine("};");
        });

        builder.AppendLine("}");

        context.AddSource("LucideLookupDictionary.g.cs", builder.ToString());
    }

}
