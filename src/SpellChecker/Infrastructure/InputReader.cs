namespace SpellChecker.Infrastructure;

public sealed class InputReader
{
    private const int MaxWordLength = 50;

    /// <summary>
    /// Reads and parses dictionary words line by line from the reader until encountering the '===' section separator.
    /// </summary>
    /// <param name="reader">The text reader stream containing the dictionary section.</param>
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
                ValidateWord(word);
                words.Add(word);
            }
        }

        throw new InvalidDataException(
            "Input does not contain the dictionary terminator '==='.");
    }
    /// <summary>
    /// Reads text lines sequentially using an iterator until encountering the '===' end-of-file separator.
    /// </summary>
    /// <param name="reader">The text reader stream positioned at the input text section.</param>   
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
    /// <summary>
    /// Validates a line of input text to ensure it contains only letters and whitespace, 
    /// and that no individual word exceeds the maximum length threshold.
    /// </summary>
    /// <param name="line">The raw text line to validate.</param>
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
    /// <summary>   
    /// Validates an individual dictionary word to verify length constraints and alphabetic character rules.
    /// </summary>
    /// <param name="word">The dictionary word token to validate.</param>
    private static void ValidateWord(string word)
    {
        if (word.Length > 50)
        {
            throw new InvalidDataException(
                $"Word exceeds the maximum length of 50 characters: '{word}'.");
        }

        if (!word.All(char.IsLetter))
        {
            throw new InvalidDataException(
                $"Invalid word: '{word}'. Words may contain letters only.");
        }
    }
}