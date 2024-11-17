namespace REFDumpFormatter;

public ref struct StringParser
{
    public readonly ReadOnlySpan<char> text;

    public StringParser(ReadOnlySpan<char> text) : this()
    {
        this.text = text;
    }

    public int pos;
    public Classname? parentClass;
    public Classname? containingClass;
    public char Cur => pos < text.Length ? text[pos] : '\0';
    public bool MayFail { get; set; }
}
