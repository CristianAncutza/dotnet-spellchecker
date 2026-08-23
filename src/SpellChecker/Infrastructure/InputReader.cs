namespace SpellChecker.Infrastructure;

public sealed class InputReader
{
    private const int MaxWordLength = 50;
    public IReadOnlyList<string> ReadDictionary(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

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

    public IEnumerable<string> ReadTextLines(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            if (line == "===")
            {
                yield break;
            }

            ValidateTextLine(line);
            yield return line;
        }

        throw new InvalidDataException(
            "Input does not contain the text terminator '==='.");
    }

    private static void ValidateTextLine(string line)
    {
        var wordLength = 0;

        foreach (var character in line)
        {
            if (char.IsLetter(character))
            {
                wordLength++;

                if (wordLength > MaxWordLength)
                {
                    throw new InvalidDataException(
                        $"Word length exceeds {MaxWordLength} characters.");
                }
            }
            else if (char.IsWhiteSpace(character))
            {
                wordLength = 0;
            }
            else
            {
                throw new InvalidDataException(
                    $"Invalid character '{character}' in text input.");
            }
        }
    }

        private static void ValidateWord(string word)
    {
        if (word.Length == 0 || word.Length > MaxWordLength)
        {
            throw new InvalidDataException(
                $"Dictionary word length must be between 1 and {MaxWordLength} characters.");
        }

        foreach (var character in word)
        {
            if (!char.IsLetter(character))
            {
                throw new InvalidDataException(
                    $"Dictionary word '{word}' contains a non-letter character.");
            }
        }
    }
}