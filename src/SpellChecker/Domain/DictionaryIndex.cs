//It takes care of storing and consulting the dictionary
namespace SpellChecker.Domain;
public sealed class DictionaryIndex
{
    private readonly HashSet<string> _words;
    private readonly List<string> _orderedWords;

    public DictionaryIndex(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        _words = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase
        );
        _orderedWords = [];

        foreach (var word in words)
        {
            if (_words.Add(word))
            {
                _orderedWords.Add(word);
            }
        }
    }

    public bool Contains(string word)
    {
        return _words.Contains(word);
    }

    public IReadOnlyList<string> Words => _orderedWords;
}