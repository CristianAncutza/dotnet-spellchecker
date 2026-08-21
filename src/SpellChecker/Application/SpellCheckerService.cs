using System.Text;
using SpellChecker.Domain;

namespace SpellChecker.Application;

public sealed class SpellCheckerService
{
    public void Process(string inputPath, string outputPath)
    {
        var lines = File.ReadAllLines(inputPath);

        var dictionaryWords = new List<string>();
        var textLines = new List<string>();

        var readingDictionary = true;

        foreach (var line in lines)
        {
            if (line == "===")
            {
                readingDictionary = false;
                break;
            }

            dictionaryWords.AddRange(
                line.Split(
                    (char[]?)null,
                    StringSplitOptions.RemoveEmptyEntries));
        }

        var separatorIndex = Array.IndexOf(lines, "===");

        if (separatorIndex >= 0)
        {
            for (var i = separatorIndex + 1; i < lines.Length; i++)
            {
                if (lines[i] == "===")
                {
                    break;
                }

                textLines.Add(lines[i]);
            }
        }

        var dictionary = new DictionaryIndex(dictionaryWords);
        var spellChecker = new SpellingChecker(dictionary);

        using var writer = new StreamWriter(outputPath);

        foreach (var line in textLines)
        {
            writer.WriteLine(ProcessLine(line, spellChecker));
        }
    }

    private static string ProcessLine(string line, SpellingChecker spellChecker)
    {
        var result = new StringBuilder();
        var index = 0;

        while (index < line.Length)
        {
            if (!char.IsLetter(line[index]))
            {
                result.Append(line[index]);
                index++;
                continue;
            }

            var start = index;

            while (index < line.Length && char.IsLetter(line[index]))
            {
                index++;
            }

            var word = line[start..index];

            result.Append(spellChecker.Correct(word));
        }

        return result.ToString();
    }
}