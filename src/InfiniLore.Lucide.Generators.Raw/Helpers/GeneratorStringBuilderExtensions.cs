// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.GeneratorTools;

namespace InfiniLore.Lucide.Generators.Raw.Helpers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class GeneratorStringBuilderExtensions {
    public static GeneratorStringBuilder WriteLucideLicense(this GeneratorStringBuilder builder) => builder
        .AppendLine("// ---")
        .AppendLine("// ISC License")
        .AppendLine("// Copyright (c) for portions of Lucide are held by Cole Bemis 2013-2022 as part of Feather (MIT). All other copyright (c) for Lucide are held by Lucide Contributors 2022.")
        .AppendLine("//")
        .AppendLine("// Permission to use, copy, modify, and/or distribute this software for any")
        .AppendLine("// purpose with or without fee is hereby granted, provided that the above")
        .AppendLine("// copyright notice and this permission notice appear in all copies.")
        .AppendLine("//")
        .AppendLine("// THE SOFTWARE IS PROVIDED \"AS IS\" AND THE AUTHOR DISCLAIMS ALL WARRANTIES")
        .AppendLine("// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF")
        .AppendLine("// MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR")
        .AppendLine("// ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES")
        .AppendLine("// WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN")
        .AppendLine("// ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF")
        .AppendLine("// OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.")
        .AppendLine("//")
        .AppendLine("// ---");
}
