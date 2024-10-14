namespace REFDumpFormatter;

public class ClassSummary
{
    public string OriginalName { get; set; } = null!;
    public Classname Name { get; set; } = null!;

    public ClassSummary(string originalName, Classname name)
    {
        OriginalName = originalName;
        Name = name;
    }
}
