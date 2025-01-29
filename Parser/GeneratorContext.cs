namespace REFDumpFormatter;

using System.Globalization;
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

    public Dictionary<string, REFDumpFormatter.ObjectDef> classNames;
    public Dictionary<string, string>? remappedTypeNames;
    public OutputOptions options;
    private readonly Dictionary<string, int> modifiedFilenames = new();

    public Func<Classname, GeneratorContext, bool>? ClassnameFilter { get; set; }
    public string OutputDirectory { get; }

    public int TotalGeneratedCount { get; private set; }
    public HashSet<string> FailedClassnames { get; } = new();

    public GeneratorContext(Dictionary<string, REFDumpFormatter.ObjectDef> classNames, OutputOptions options, string outputDirectory)
    {
        this.classNames = classNames;
        this.options = options;
        OutputDirectory = outputDirectory;

        if (!Directory.Exists(outputDirectory)) {
            Directory.CreateDirectory(outputDirectory);
        }
        Console.WriteLine("Writing output to " + outputDirectory);
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

        TotalGeneratedCount++;
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

    public IEnumerable<(string name, string value, string valueHex)> GetSortedEnumValues(ObjectDef item, string backingType)
    {
        if (item.fields == null) yield break;

        foreach (var (name, field) in item.fields.OrderBy(f => f.Value.Id)) {
            if (!field.Flags.Contains("SpecialName") && field.IsStatic && field.Default is JsonElement elem && elem.ValueKind == JsonValueKind.Number) {
                if (backingType == "System.Long" || backingType == "long") {
                    var val = ((long)elem.GetUInt64());
                    yield return (name, val.ToString(), val.ToString("X"));
                } else if (backingType == "System.ULong" || backingType == "ulong") {
                    yield return (name, elem.GetUInt64().ToString(), elem.GetUInt64().ToString("X"));
                } else {
                    var baseVal = elem.GetInt64();
                    var valstr = backingType switch {
                        "System.Int32" => (baseVal >= 2147483648 ? (baseVal - 2 * 2147483648L) : baseVal).ToString(),
                        "int" => (baseVal >= 2147483648 ? (baseVal - 2 * 2147483648L) : baseVal).ToString(),
                        "System.Int16" => (baseVal >= 32768 ? (baseVal - 2 * 32768) : baseVal).ToString(),
                        "short" => (baseVal >= 32768 ? (baseVal - 2 * 32768) : baseVal).ToString(),
                        "System.SByte" => (baseVal >= 128 ? (baseVal - 2 * 128) : baseVal).ToString(),
                        "sbyte" => (baseVal >= 128 ? (baseVal - 2 * 128) : baseVal).ToString(),
                        _ => baseVal.ToString(),
                    };
                    var hex = backingType switch {
                        "System.Int32" => int.Parse(valstr).ToString("X"),
                        "int" => int.Parse(valstr).ToString("X"),
                        "System.UInt32" => uint.Parse(valstr).ToString("X"),
                        "uint" => uint.Parse(valstr).ToString("X"),

                        "System.Int16" => short.Parse(valstr).ToString("X"),
                        "short" => short.Parse(valstr).ToString("X"),
                        "System.UInt16" => ushort.Parse(valstr).ToString("X"),
                        "ushort" => ushort.Parse(valstr).ToString("X"),

                        "System.SByte" => sbyte.Parse(valstr).ToString("X"),
                        "sbyte" => sbyte.Parse(valstr).ToString("X"),
                        "System.Byte" => byte.Parse(valstr).ToString("X"),
                        "byte" => byte.Parse(valstr).ToString("X"),

                        _ => baseVal.ToString("X"),
                    };
                    yield return (name, valstr, hex);
                }
            }
        }
    }

    public (string path, bool newFile) GetOutputFile(ClassSummary cls, int sequence = 0)
    {
        var ext = options.Type == OutputType.GenerateLua ? ".lua" : ".cs";
        var filename = options.JoinByNamespace && !string.IsNullOrEmpty(cls.Name.Namespace) ? cls.Name.Namespace : cls.Name.StringifyForFilename();
        var outputPath = options.JoinByNamespace
            ? Path.Combine(OutputDirectory, (sequence == 0 ? filename : filename + "-" + sequence) + ext)
            : Path.Combine(OutputDirectory, cls.Name.Namespace.Replace('.', Path.DirectorySeparatorChar), filename + ext);

        if (!modifiedFilenames.TryGetValue(outputPath, out var writeCount)) {
            modifiedFilenames[outputPath] = 1;
            Directory.CreateDirectory(Directory.GetParent(outputPath)!.FullName);
            return (outputPath, true);
        } else if (writeCount > options.ClassesPerFile) {
            return GetOutputFile(cls, sequence + 1);
        } else {
            modifiedFilenames[outputPath] = writeCount + 1;
            return (outputPath, false);
        }
    }

    public void CopyIncludes()
    {
        var sourceDir = $"Includes/{(options.Type == OutputType.GenerateLua ? "lua" : "csharp")}";
        if (Directory.Exists(sourceDir)) {
            CopyFilesRecursively(sourceDir, OutputDirectory);
        }
    }

    // https://stackoverflow.com/a/3822913/4721768
    private static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)) {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories)) {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}
