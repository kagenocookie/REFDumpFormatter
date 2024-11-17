using REFDumpFormatter;

static void PrintHelp()
{
    Console.WriteLine("""
    Usage: il2cppFormatter.exe <path_to_il2cppdump> [<output_file_path>] [OPTIONS]

    path_to_il2cppdump: Should be the path to the REFramework generated il2cpp_dump.json file
    output_file_path: If not set, it will default to il2cpp_dump.json directory + /csharp_reference

    Output format options:
        --generate-csharp Generate C# classes (default)
        --generate-lua Generate LuaCATS annotations
    Additional options:
        --field-offsets   Include the field offsets in the output, useful when reverse engineering the executable (crashes) or native code
        --include-overloads Include all method overloads
        --ignore-overloads Ignore method overloads
        --namespace-files Write all classes into one file corresponding to its namespace
        --class-files Write all classes into their own file
    """);
}

if (args.Length > 0 && args[0] == "--help") {
    PrintHelp();
    return;
}

var interactive = false;
if (args.Length < 1) {
    PrintHelp();
    Console.WriteLine();
    interactive = true;
}

string? inputFilepath = null;
string? outputFilepath = null;
bool ignoreSystemTypes = false;
bool ignoreArrays = false;
bool? ignoreOverloads = null;
bool? fieldOffsets = null;
bool? joinByNamespace = null;

OutputType action = OutputType.GenerateCsharp;

if (interactive) {
    string? ShowOption(string label, bool lowercase = true) {
        System.Console.Write(label);
        return lowercase ? System.Console.In.ReadLine()?.ToLowerInvariant() : System.Console.In.ReadLine();
    }
    inputFilepath = ShowOption("il2cpp dump file location (folder or full filename): ", false);
    if (inputFilepath != null && !File.Exists(inputFilepath)) {
        inputFilepath = Path.Combine(inputFilepath, "il2cpp_dump.json");
    }
    if (inputFilepath == null || !File.Exists(inputFilepath)) {
        Console.Error.WriteLine("File not found");
        Console.In.ReadLine();
        return;
    }
    outputFilepath = ShowOption($"output location (Default: {inputFilepath}/lang_reference):");
    action = ShowOption("Output format: [C]sharp or [L]ua: ") switch {
        "c" => OutputType.GenerateCsharp,
        "l" => OutputType.GenerateLua,
        _ => OutputType.GenerateCsharp,
    };
    joinByNamespace = ShowOption("Separate files per [c]lass or per [n]amespace: ") switch {
        "c" => false,
        "n" => true,
        _ => null,
    };
    ignoreOverloads = ShowOption("Emit all overloaded methods: [y/n] ") switch {
        "y" => false,
        "n" => true,
        _ => null,
    };
    fieldOffsets = ShowOption("Include field byte offsets: [y/n] ") == "y";
}

foreach (var arg in args) {
    if (arg.StartsWith("--")) {
        switch (arg) {
            case "--include-overloads": ignoreOverloads = false; break;
            case "--ignore-overloads": ignoreOverloads = true; break;
            case "--ignore-arrays": ignoreArrays = true; break;
            case "--generate-csharp": action = OutputType.GenerateCsharp; break;
            case "--generate-lua": action = OutputType.GenerateLua; break;
            case "--field-offsets": fieldOffsets = true; break;
            case "--namespace-files": joinByNamespace = true; break;
            case "--class-files": joinByNamespace = false; break;
            default: Console.WriteLine("Unknown option " + arg); PrintHelp(); return;
        }
    } else {
        if (inputFilepath == null) {
            inputFilepath = arg;
        } else if (outputFilepath == null) {
            outputFilepath = arg;
        }
    }
}

if (inputFilepath == null) {
    PrintHelp();
    if (interactive) System.Console.In.ReadLine();
    return;
}

switch (action)
{
    case OutputType.GenerateCsharp:
        ignoreArrays = true;
        ignoreSystemTypes = true;
        ignoreOverloads ??= false;
        joinByNamespace ??= false;
        break;
    case OutputType.GenerateLua:
        ignoreArrays = true;
        ignoreOverloads ??= true;
        joinByNamespace ??= true;
        break;
    default:
        Console.Error.WriteLine("Output format unspecified, aborting.");
        return;
}

Console.WriteLine($"Reading dump file: {inputFilepath}. This might take a while...");

using var fs = File.OpenRead(inputFilepath);
var entries = System.Text.Json.JsonSerializer.Deserialize<SourceDumpRoot>(fs)
    ?? throw new Exception("File is not a valid dump json file");
fs.Close();


var filteredEntries = entries.Where(e => {
    var name = e.Key;
    var entry = e.Value;
    if (name == "" || name.StartsWith('!')) {
        // skip these, "!" are generic arg names which aren't real types
        return false;
    }

    if (ignoreSystemTypes && (name.StartsWith("System.") || name == "Internal.Runtime.CompilerServices.Unsafe")) {
        return false;
    }

    if (ignoreArrays && name.EndsWith("[]")) {
        return false;
    }

    return true;
}).Select(e => (e.Key, e.Value));

var options = new OutputOptions() {
    Type = action,
    InputFilepath = inputFilepath,
    OutputFilepath = string.IsNullOrWhiteSpace(outputFilepath) ? null : outputFilepath,
    FieldOffsets = fieldOffsets,
    IgnoreOverloads = ignoreOverloads.Value,
    JoinByNamespace = joinByNamespace.Value,
    ClassesPerFile = 250,
};
GeneratorContext? ctx = null;
switch (action)
{
    case OutputType.GenerateCsharp:
        ctx = new GenerateCsharp().GenerateOutput(options, filteredEntries);
        break;
    case OutputType.GenerateLua:
        ctx = new GenerateLua().GenerateOutput(options, filteredEntries);
        break;
}

if (ctx != null && ctx.FailedClassnames.Count != 0) {
    Console.Error.WriteLine($"{ctx.FailedClassnames.Count} classnames failed to generate:");
    foreach (var failed in ctx.FailedClassnames) {
        Console.Error.WriteLine(failed);
    }
    Console.WriteLine("Press any key to close");
    Console.ReadKey();
}