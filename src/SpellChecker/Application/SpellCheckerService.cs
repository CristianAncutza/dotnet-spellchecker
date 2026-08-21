using System.Text;
using SpellChecker.Domain;
using SpellChecker.Infrastructure;

namespace SpellChecker.Application;

public sealed class SpellCheckerService
{
    private readonly InputReader _inputReader;
    private readonly OutputWriter _outputWriter;

    public SpellCheckerService(
        InputReader inputReader,
        OutputWriter outputWriter)
    {
        _inputReader = inputReader;
        _outputWriter = outputWriter;
    }

    public void Process(string inputPath, string outputPath)
    {
        using var reader = new StreamReader(inputPath);

        var dictionaryWords = _inputReader.ReadDictionary(reader);

        var dictionary = new DictionaryIndex(dictionaryWords);
        var spellChecker = new SpellingChecker(dictionary);

        using var writer = new StreamWriter(outputPath);

        foreach (var line in _inputReader.ReadTextLines(reader))
        {
            var correctedLine = ProcessLine(line, spellChecker);
            _outputWriter.WriteLine(writer, correctedLine);
        }
    }

    private static string ProcessLine(
        string line,
        SpellingChecker spellChecker)
    {
        var result = new StringBuilder(line.Length);
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