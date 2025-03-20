// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components;

namespace InfiniLore.Lucide;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class LucideIcon {
    public static RenderFragment Render(string iconSvgContent, Dictionary<string, object>? attributes = null) 
        => builder => {
            builder.OpenComponent<LucideIcon>(0); // Parent LucideIcon component
            builder.AddAttribute(1, "SvgContent", new MarkupString(iconSvgContent));
            builder.AddMultipleAttributes(2, attributes ?? new Dictionary<string, object>()); // Dynamically apply attributes
            builder.CloseComponent();
        };

    // Predefined Signature RenderFragment with customizable attributes
    public static RenderFragment Signature(string @class = "") =>
        Render(Data.Signature._flatSvgContent, new Dictionary<string, object>() {
            ["class"] = @class,
        });
}
