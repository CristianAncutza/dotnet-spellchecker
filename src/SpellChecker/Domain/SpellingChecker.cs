namespace SpellChecker.Domain;

public sealed class SpellingChecker
{
    private readonly DictionaryIndex _dictionary;
    private readonly CorrectionFinder _correctionFinder;

    public SpellingChecker(DictionaryIndex dictionary)
    {
        _dictionary = dictionary;
        _correctionFinder = new CorrectionFinder(dictionary);
    }

    public string Correct(string word)
    {
        // Correct words are printed exactly as they appeared in the input.
        if (_dictionary.Contains(word))
        {
            return word;
        }

        var corrections = _correctionFinder.FindCorrections(word);

        return corrections.Count switch
        {
            0 => $"{{{word}?}}",
            1 => corrections[0],
            _ => $"{{{string.Join(' ', corrections)}}}"
        };
    }
}