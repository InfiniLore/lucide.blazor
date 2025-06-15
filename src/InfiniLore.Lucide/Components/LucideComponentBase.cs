// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components;

namespace InfiniLore.Lucide;
// -----------------------------------------------------------------------------------------------------------------
// Methods
// -----------------------------------------------------------------------------------------------------------------
public class LucideComponentBase  : ComponentBase {
    [Parameter] public string? Class { get; set; }
    [Parameter] public int Size { get; set; } = 24;
    [Parameter] public string Fill { get; set; } = "none";
    [Parameter] public string Stroke { get; set; } = "currentColor";
    [Parameter] public int StrokeWidth { get; set; } = 2;
    [Parameter] public string StrokeLineCap { get; set; } = "round";
    [Parameter] public string StrokeLineJoin { get; set; } = "round";
}
