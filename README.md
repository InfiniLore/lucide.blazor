# CodeOfChaos.BlazorIcons.Lucide

**CodeOfChaos.BlazorIcons.Lucide** lets you integrate [Lucide Icons](https://lucide.dev/) into Blazor apps with a simple Razor component that renders SVG markup with configurable styling.

**This package is developed as an independent project and is not affiliated, associated, or endorsed by the creators or maintainers of the Lucide library.**
**It is built to enhance the experience of using Lucide's open-source icon set within Blazor applications.**

This package uses the Minor section of semantic versions (`0.x.0`) to denote which Lucide version is used to build the current package.

---

## Features
- **Reusable Components**: Add icons via Razor components.
- **Icon Customization**: Configure `size`, `fill`, `stroke`, and related SVG styling.
- **Dynamic Icons**: Change the icon name on the fly using `<LucideIcon Name="..."/>`.
- **Static Icons**: Prefer static markup? Use `<Li.../>` components with baked-in SVG data.
- **WASM Support**: Use the same components and services on server and client.

---

## Getting Started

### Install the Package
Make sure you have [.NET 9.0](https://dotnet.microsoft.com/) installed. Then install **CodeOfChaos.BlazorIcons.Lucide**:

```shell
dotnet add package CodeOfChaos.BlazorIcons.Lucide
```

---

## Usage

### Register Services (for `LucideIcon`)
`LucideIcon` uses `ILucideService` to resolve SVG data. Register the service once at startup.
`Li...` components do not depend on this service.

`Program.cs`
```csharp
builder.Services.AddLucideIcons();
```

`_Imports.razor`
```csharp
@using CodeOfChaos.BlazorIcons.Lucide
```

### Add the Component
`LucideIcon` refreshes its SVG when `Name` changes. `Li...` components are static and must be swapped to change icons at runtime.

The naming convention for `Li...` components is `Li{LucideName.ToPascalCase()}` with dashes removed.
The `LucideIcon.Name` parameter accepts common variations like `arrow-big-down-dash`, `arrowbigdowndash`, or `ArrowBigDownDash`.

`class` is supported on both `LucideIcon` and `Li...` components.

```razor
<!-- Minimal -->
<LucideIcon Name="signature" />

<!-- Full options -->
<LucideIcon Name="arrow-right"
           Size="48"
           Fill="none"
           Stroke="black"
           StrokeWidth="2"
           StrokeLineCap="round"
           StrokeLineJoin="round" />

<!-- Static icon -->
<LiSignature />
<LiDam class="some-red-class" />
```

### Parameters (`LucideIcon`)

| Parameter        | Type                 | Default          | Description                                   |
|------------------|----------------------|------------------|-----------------------------------------------|
| `Name`           | string               | **Required**     | Icon name (case insensitive, flexible input). |
| `Class`          | string?              | `null`           | CSS class for the `<svg>`.                    |
| `Size`           | int                  | `24`             | Width and height of the icon.                 |
| `Fill`           | string               | `"none"`         | Fill color of the icon.                       |
| `Stroke`         | string               | `"currentColor"` | Stroke color of the icon.                     |
| `StrokeWidth`    | int                  | `2`              | Stroke width of the icon.                     |
| `StrokeLineCap`  | string               | `"round"`        | Line cap style (`butt`, `round`).             |
| `StrokeLineJoin` | string               | `"round"`        | Line join style (`miter`, `round`, `bevel`).   |
| `ChildContent`   | RenderFragment?      | `null`           | Extra SVG children appended to the icon.      |

---

## Integration Details

This library:
- Uses `lucide-static` for icon definitions during package development.
- Exposes `ILucideIconData` for SVG data encapsulation.

### Dependencies
- [Lucide-Static](https://www.npmjs.com/package/lucide-static) for icon SVG content.
- **CodeOfChaos.GeneratorTools** for generator tooling in source generation scenarios.

### Supported Platforms
- **.NET 9.0** Blazor Server and WebAssembly.
- **.NET 10.0** Blazor Server and WebAssembly.

---

## Development Notes

This project follows a modular structure for maintainability:
- `CodeOfChaos.BlazorIcons.Lucide`: Blazor components and service logic.
- `CodeOfChaos.BlazorIcons.Lucide.Data`: Handles icon definitions and metadata, data provided by `CodeOfChaos.BlazorIcons.Lucide.Generators.Raw`.
- `CodeOfChaos.BlazorIcons.Lucide.Generators.Raw`: Implements tooling via Roslyn to read data from lucide-static package.
- `Tools.CodeOfChaos.BlazorIcons.Lucide`: A set of development tools, like the Razor file generator.

---

## License

CodeOfChaos.BlazorIcons.Lucide is built on **Lucide**, which is distributed under the ICS license:

```text
Permission to use, copy, modify, and/or distribute this software for any
purpose with or without fee is hereby granted, provided that the above
copyright notice and this permission notice appear in all copies.

THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
```

For full license details, see [Lucide License](https://lucide.dev/license).

---

## Contributions

Contributions are welcome! To get started:
1. Fork the repository.
2. Create a new branch for your feature or fix.
3. Submit a pull request with a detailed description of your changes.

For further development or issues, feel free to raise a GitHub issue or suggest improvements.

Enjoy using **CodeOfChaos.BlazorIcons.Lucide** in your Blazor projects.
