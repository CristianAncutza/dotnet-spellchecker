using System.IO;

namespace SpellChecker.Infrastructure;

public sealed class InputReader
{
    public IReadOnlyList<string> ReadDictionary(StreamReader reader)
    {
        var words = new List<string>();
        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            if (line == "===")
            {
                return words;
            }

            foreach (var word in line.Split(
                         (char[]?)null,
                         StringSplitOptions.RemoveEmptyEntries))
            {
                words.Add(word);
            }
        }

        throw new InvalidDataException(
            "Input does not contain the dictionary terminator '==='.");
    }

    public IEnumerable<string> ReadTextLines(StreamReader reader)
    {
        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            if (line == "===")
            {
                yield break;
            }

            yield return line;
        }

        throw new InvalidDataException(
            "Input does not contain the text terminator '==='.");
    }
}