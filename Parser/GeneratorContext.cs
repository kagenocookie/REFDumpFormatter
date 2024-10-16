namespace REFDumpFormatter;

using System.Text.Json;
using System.Text.RegularExpressions;

public partial class GeneratorContext
{
    [GeneratedRegex("^T\\d+$")]
    public static partial Regex GenericParamNameRegex();

    [GeneratedRegex("(\\[\\[)|(\\]\\])")]
    public static partial Regex BracketsOuter();

    [GeneratedRegex(", [\\w\\d]+, [\\w\\d, =.]+\\]")]
    public static partial Regex BracketsNamespace();

    [GeneratedRegex("\\[([\\w\\d,`<>\\.]+)\\]")]
    public static partial Regex BracketsWrapped();

    [GeneratedRegex("\\[([\\w\\d,`<>\\.]+)\\[\\]\\]")]
    public static partial Regex BracketsArrayWrapped();

    private readonly PriorityQueue<string, ulong> enumSorter = new();

    public Dictionary<string, REFDumpFormatter.ObjectDef> classNames;
    public Dictionary<string, string>? remappedTypeNames;
    public OutputOptions options;

    public Func<Classname, GeneratorContext, bool>? ClassnameFilter { get; set; }

    public GeneratorContext(Dictionary<string, REFDumpFormatter.ObjectDef> classNames, OutputOptions options)
    {
        this.classNames = classNames;
        this.options = options;
    }

    private static string CleanBracketGeneric(ReadOnlySpan<char> str)
    {
        var outStr = BracketsOuter().Replace(str.ToString(), (match) => match.Groups[1].Success ? "<[" : "]>"); // -- turn all [[ and ]] into <[ and ]>
        outStr = BracketsNamespace().Replace(outStr, "]"); // remove all namespace+version+culture+token segments
        outStr = BracketsWrapped().Replace(outStr, "]"); // remove single [] brackets around types
        outStr = BracketsWrapped().Replace(outStr, "]"); // repeat above to also handle nested generics
        outStr = BracketsArrayWrapped().Replace(outStr, "]"); // a few types also have an array value for the last generic, handle that too
        return outStr;
    }

    public ClassSummary? GenerateSummary(string name, ObjectDef item)
    {
        if (name.StartsWith('<') || name.Contains("DisplayClass") || name.Contains("<>") || name.Contains(".<") || name.Contains("<!0>")) {
            // skip invalid / compiler generated stuff
            return null;
        }

        if (item.is_generic_type && !item.is_generic_type_definition) {
            return null;
        }

        if (string.IsNullOrEmpty(item.address)) {
            // probably not callable, very likely useless
            return null;
        }

        var clsName = Classname.Parse(name, this, ignoreNonDefinitions: true);
        if (clsName == null) {
            // ignore
            return null;
        }
        if (ClassnameFilter?.Invoke(clsName, this) == false) {
            return null;
        }

        var cls = new ClassSummary(name, clsName);
        var genericSection = string.Empty;

        return cls;
    }

    public string? GetEnumType(ObjectDef item, Classname cls)
    {
        if (item.fields == null) {
            return null;
        }

        string enumType = "int";
        foreach (var (fieldName, field) in item.fields) {
            if (fieldName == "value__") {
                enumType = cls.PreprocessTypeForDisplay(field.Type);
                break;
            }
        }

        return enumType;
    }

    public IEnumerable<(string name, ulong value)> GetSortedEnumValues(ObjectDef item)
    {
        if (item.fields == null) {
            yield break;
        }

        enumSorter.Clear();
        foreach (var (fieldName, field) in item.fields) {
            if (fieldName != "value__" && field.IsStatic && field.Default is JsonElement elem && elem.ValueKind == JsonValueKind.Number) {
                var val = elem.GetUInt64();
                enumSorter.Enqueue(fieldName, val);
            }
        }

        // foreach (var (enumValue, enumName) in enumSorter)
        while (enumSorter.TryDequeue(out var enumName, out var enumValue)) {
            yield return (enumName, enumValue);
        }
    }
}
