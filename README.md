# InfiniLore.Lucide

**InfiniLore.Lucide** is a package that allows you to seamlessly integrate [Lucide Icons](https://lucide.dev/) into your Blazor applications. This library provides an easy-to-use component and tools to render SVG icons dynamically with customizable properties.

---

## Features
- **Reusable Components**: Incorporate Lucide's rich collection of SVG icons using Razor components.
- **Icon Customization**: Adjust properties like `fill`, `stroke`, `width`, `height`, and more.
- **Performance-Oriented Rendering**: Icons are served as SVG with minimal overhead.
- **Fully Typed API**: Seamless integration with C# for type-safe development.
- **Support for .NET 9.0**: Built with the latest .NET technology for modern application development.

---

## Getting Started

To get started with **InfiniLore.Lucide**, follow the steps below:

### 1. **Install the Package**

Make sure you have [.NET 9.0](https://dotnet.microsoft.com/) installed. Then, install **InfiniLore.Lucide** into your Blazor project:

```shell script
dotnet add package InfiniLore.Lucide
```

---

### 2. **Usage**

#### Add the Lucide Component
To include an icon in your Blazor application, use the `LucideSvg` component:

```html
<!-- Minimal requirement -->
<LucideIcon Name="signature"/>

<!-- Full options -->
<LucideIcon Name="arrow-right"
           Width="48"
           Height="48"
           Fill="none"
           Stroke="black"
           StrokeWidth="2"
           StrokeLineCap="round"
           StrokeLineJoin="round" />
```

#### Parameters
Below are the parameters you can configure for the `LucideSvg` component:

| Parameter          | Type    | Default       | Description                                    |
|--------------------|---------|---------------|------------------------------------------------|
| `IconName`         | string  | **Required**  | Name of the icon (case sensitive).             |
| `Width`            | int     | `24`          | Width of the icon.                             |
| `Height`           | int     | `24`          | Height of the icon.                            |
| `Fill`             | string  | `"none"`      | Fill color of the icon.                        |
| `Stroke`           | string  | `"currentColor"` | Stroke color of the icon.                   |
| `StrokeWidth`      | int     | `2`           | Stroke width of the icon.                      |
| `StrokeLineCap`    | string  | `"round"`     | Shape of the ends of lines (`butt`, `round`).  |
| `StrokeLineJoin`   | string  | `"round"`     | Style of corners (`miter`, `round`, `bevel`).  |

---

### 3. **Custom SVG Icons**

The library can also accept raw SVG content for advanced use cases. You can programmatically get an SVG's content using the `LucideService.GetIconContent(string iconName)` method.

---

## Integration Details

This library:
- Internally utilizes the `lucide-static` package for icon definitions.
- Includes `ILucideIconData` for icon data encapsulation, providing structured interfaces for SVG manipulation.
- Supports additional data manipulation for expanding sources via the `AdditionalSourcesCollection` class.

### Dependencies
- [Lucide-Static](https://www.npmjs.com/package/lucide-static) for icon SVG content.
- **CodeOfChaos.GeneratorTools** for generator tooling in source generation scenarios.

### Supported Platforms
- **.NET 9.0** Blazor Server and WebAssembly.

---

## Development Notes

This project follows a modular structure for maintainability:
- `InfiniLore.Lucide`: Blazor components and view logic.
- `InfiniLore.Lucide.Data`: Handles icon definitions and metadata.
- `InfiniLore.Lucide.Generators`: Implements tooling via Roslyn to read data from lucide.static package.

---

## License

InfiniLore.Lucide is built on **Lucide**, which is distributed under the ICS license:

```plain text
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

Enjoy using **InfiniLore.Lucide** in your Blazor projects! 🚀
