namespace SpellChecker.Domain;

public sealed class CorrectionFinder
{
    private const int MaxEdits = 2;

    private readonly TrieNode _root;

    public CorrectionFinder(DictionaryIndex dictionary)
    {
        _root = new TrieNode();

        for (var i = 0; i < dictionary.Words.Count; i++)
        {
            AddWord(dictionary.Words[i], i);
        }
    }

    public IReadOnlyList<string> FindCorrections(string word)
    {
        // First look for corrections requiring only one edit.
        var oneEditCorrections = FindCorrections(word, 1);

        if (oneEditCorrections.Count > 0)
        {
            return oneEditCorrections;
        }

        // Only search two-edit corrections when no one-edit correction exists.
        return FindCorrections(word, MaxEdits);
    }

    private List<string> FindCorrections(string word, int maxEdits)
    {
        var results = new List<WordMatch>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        Search(
            node: _root,
            word: word,
            inputIndex: 0,
            editsUsed: 0,
            lastEdit: EditType.None,
            maxEdits: maxEdits,
            results: results,
            seen: seen);

        results.Sort((x, y) => x.Order.CompareTo(y.Order));

        return results.Select(x => x.Word).ToList();
    }

    private static void Search(
        TrieNode node,
        string word,
        int inputIndex,
        int editsUsed,
        EditType lastEdit,
        int maxEdits,
        List<WordMatch> results,
        HashSet<string> seen)
    {
        if (inputIndex == word.Length && node.Word is not null)
        {
            if (seen.Add(node.Word))
            {
                results.Add(new WordMatch(node.Word, node.Order));
            }
        }

        if (editsUsed == maxEdits)
        {
            // No more edits are allowed. We can only match remaining characters.
            if (inputIndex < word.Length &&
                node.Children.TryGetValue(ToLower(word[inputIndex]), out var child))
            {
                Search(
                    child,
                    word,
                    inputIndex + 1,
                    editsUsed,
                    EditType.None,
                    maxEdits,
                    results,
                    seen);
            }

            return;
        }

        // 1. Match the current character.
        if (inputIndex < word.Length &&
            node.Children.TryGetValue(ToLower(word[inputIndex]), out var matchingChild))
        {
            Search(
                matchingChild,
                word,
                inputIndex + 1,
                editsUsed,
                EditType.None,
                maxEdits,
                results,
                seen);
        }

        // 2. Insert a character into the input word.
        //
        // Two insertions cannot be adjacent, so another insertion
        // immediately after an insertion is forbidden.
        if (lastEdit != EditType.Insert)
        {
            foreach (var child in node.Children.Values)
            {
                Search(
                    child,
                    word,
                    inputIndex,
                    editsUsed + 1,
                    EditType.Insert,
                    maxEdits,
                    results,
                    seen);
            }
        }

        // 3. Delete a character from the input word.
        //
        // Two deletions cannot be adjacent, so another deletion
        // immediately after a deletion is forbidden.
        if (inputIndex < word.Length && lastEdit != EditType.Delete)
        {
            Search(
                node,
                word,
                inputIndex + 1,
                editsUsed + 1,
                EditType.Delete,
                maxEdits,
                results,
                seen);
        }
    }

    private void AddWord(string word, int order)
    {
        var current = _root;

        foreach (var character in word)
        {
            var normalized = ToLower(character);

            if (!current.Children.TryGetValue(normalized, out var child))
            {
                child = new TrieNode();
                current.Children[normalized] = child;
            }

            current = child;
        }

        current.Word ??= word;
        current.Order = Math.Min(current.Order, order);
    }

    private static char ToLower(char character)
    {
        return char.ToLowerInvariant(character);
    }

    private sealed class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; } = [];

        public string? Word { get; set; }

        public int Order { get; set; } = int.MaxValue;
    }

    private sealed record WordMatch(string Word, int Order);
}