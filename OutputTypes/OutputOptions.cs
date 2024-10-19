namespace REFDumpFormatter;

public class OutputOptions
{
    public OutputType Type { get; init; }
    public string InputFilepath { get; init; } = null!;
    public string? OutputFilepath { get; init; }
    public bool? FieldOffsets { get; init; }
    public bool IgnoreOverloads { get; init; }
    public bool JoinByNamespace { get; init; }
    public int ClassesPerFile { get; init; }
}
