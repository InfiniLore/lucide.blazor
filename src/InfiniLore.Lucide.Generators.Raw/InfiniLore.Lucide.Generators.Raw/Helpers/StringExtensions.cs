// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System;

namespace InfiniLore.Lucide.Generators.Raw.Helpers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class LucideStringExtensions {
    public static string ToPascalCase(this string input) {
        if (string.IsNullOrEmpty(input))
            return input;

        string[] words = input.Split([' ', '-'], StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < words.Length; i++) {
            string word = words[i];
            if (word.Length > 0) {
                words[i] = char.ToUpper(word[0]) + word.Substring(1).ToLower();
            }
        }
        return string.Concat(words);
    }
    
    public static string ToCamelCase(this string input) {
        if (string.IsNullOrEmpty(input))
            return input;

        string[] words = input.Split([' ', '-'], StringSplitOptions.RemoveEmptyEntries);
        
        for (int i = 0; i < words.Length; i++) {
            string word = words[i];
            if (word.Length <= 0) continue;
            if (i == 0)
                words[i] = char.ToLower(word[0]) + word.Substring(1);
            else
                words[i] = char.ToUpper(word[0]) + word.Substring(1).ToLower();
        }

        return string.Concat(words);
    }
}
