namespace SpellChecker.Infrastructure;

public sealed class OutputWriter
{
    public void WriteLine(StreamWriter writer, string line)
    {
        writer.WriteLine(line);
    }
}