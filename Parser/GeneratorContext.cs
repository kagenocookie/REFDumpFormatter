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
    private readonly Dictionary<string, int> modifiedFilenames = new();

    public Func<Classname, GeneratorContext, bool>? ClassnameFilter { get; set; }
    public string OutputDirectory { get; }

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
