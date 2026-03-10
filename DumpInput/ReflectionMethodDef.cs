using System.Text.Json.Serialization;

namespace REFDumpFormatter;

public class ReflectionMethodDef
{
    [JsonPropertyName("function")]
    public string? Function { get; set; }

    [JsonPropertyName("params")]
    public ParamDef?[]? Params { get; set; }

    // note: is useless most of the type because the il2cpp gets confused and spits out a method name instead, ignoring
    // [JsonPropertyName("returns")]
    // public string? Returns { get; set; }
}
