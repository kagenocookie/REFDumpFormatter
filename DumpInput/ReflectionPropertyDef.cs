using System.Text.Json.Serialization;

namespace REFDumpFormatter;

public class ReflectionPropertyDef
{
    [JsonPropertyName("order")]
    public int Order { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}
