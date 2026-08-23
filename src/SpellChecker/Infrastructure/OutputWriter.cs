namespace SpellChecker.Infrastructure;

public sealed class OutputWriter
{
    public void WriteLine(StreamWriter writer, string line)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(line);
        
        writer.WriteLine(line);
    }
}