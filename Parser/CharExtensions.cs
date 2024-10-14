namespace REFDumpFormatter;

public static class CharExtensions
{
    public static int IndexOfAnyOffset(this ReadOnlySpan<char> text, char ch1, char ch2, int start)
    {
        var idx = text[start..].IndexOfAny(ch1, ch2);
        if (idx == -1) return -1;
        return idx + start;
    }
    public static int IndexOfAnyOffset(this ReadOnlySpan<char> text, char ch1, char ch2, char ch3, int start)
    {
        var idx = text[start..].IndexOfAny(ch1, ch2, ch3);
        if (idx == -1) return -1;
        return idx + start;
    }
    public static int IndexOfOffset(this ReadOnlySpan<char> text, char ch, int start)
    {
        var idx = text[start..].IndexOf(ch);
        if (idx == -1) return -1;
        return idx + start;
    }
    public static int IndexOfOffset(this ReadOnlySpan<char> text, string substr, int start)
    {
        var idx = text[start..].IndexOf(substr);
        if (idx == -1) return -1;
        return idx + start;
    }
}
