// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Collections.Immutable;

namespace InfiniLore.Lucide;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class LucideDynamicIcon : LucideComponentBase {
    [Parameter, EditorRequired] public string Name { get; set; } = string.Empty;
    
    public IDictionary<string, object?> DynamicParameters => new Specific(
        Class,
        Size,
        Fill,
        Stroke,
        StrokeWidth,
        StrokeLineCap,
        StrokeLineJoin
    );
    
    private record Specific(string? Class,int Size,string Fill,string Stroke,int StrokeWidth,string StrokeLineCap,string StrokeLineJoin) : IDictionary<string, object?> {
        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() {
            yield return new KeyValuePair<string, object?>(nameof(Class), Class);
            yield return new KeyValuePair<string, object?>(nameof(Size), Size);
            yield return new KeyValuePair<string, object?>(nameof(Fill), Fill);
            yield return new KeyValuePair<string, object?>(nameof(Stroke), Stroke);
            yield return new KeyValuePair<string, object?>(nameof(StrokeWidth), StrokeWidth);
            yield return new KeyValuePair<string, object?>(nameof(StrokeLineCap), StrokeLineCap);
            yield return new KeyValuePair<string, object?>(nameof(StrokeLineJoin), StrokeLineJoin);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public int Count => throw new NotImplementedException();
        public bool IsReadOnly => true;
        public void Clear() => throw new NotImplementedException();
        
        public ICollection<string> Keys => throw new NotImplementedException();
        public ICollection<object?> Values => throw new NotImplementedException();
        public void Add(KeyValuePair<string, object?> item) => throw new NotImplementedException();
        public bool Contains(KeyValuePair<string, object?> item) => throw new NotImplementedException();
        public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex) => throw new NotImplementedException();
        public bool Remove(KeyValuePair<string, object?> item) => throw new NotImplementedException();
        public void Add(string key, object? value) => throw new NotImplementedException();
        public bool ContainsKey(string key) => throw new NotImplementedException();
        public bool Remove(string key) => throw new NotImplementedException();
        public bool TryGetValue(string key, out object? value) => throw new NotImplementedException();
        public object? this[string key] {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}
