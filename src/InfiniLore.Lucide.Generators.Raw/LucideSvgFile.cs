﻿// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide.Generators.Raw.Helpers;

namespace InfiniLore.Lucide.Generators.Raw;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record LucideSvgFile(string Name, string Svg) {
    public string PascalCaseName => Name.ToPascalCase();
    public string CamelCaseName => Name.ToCamelCase();
}
