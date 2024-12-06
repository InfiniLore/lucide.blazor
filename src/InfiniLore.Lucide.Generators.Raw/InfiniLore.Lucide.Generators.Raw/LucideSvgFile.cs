// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide.Generators.Raw.Helpers;

namespace InfiniLore.Lucide.Generators.Raw;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public readonly struct LucideSvgFile(string name, string svg) {
    public string Name => name;
    public string PascalCaseName => name.ToPascalCase();
    public string CamelCaseName => name.ToCamelCase();
    
    public string Svg => svg;
}
