namespace SpellChecker.Domain;

public sealed class CorrectionFinder
{
    private const int MaxEdits = 2;

    private readonly TrieNode _root;

    /// <summary>
    /// Builds the internal Trie tree using the provided dictionary words.
    /// </summary>
    /// <param name="dictionary">The dictionary index containing the list of valid words and their order.</param>
    public CorrectionFinder(DictionaryIndex dictionary)
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        _root = new TrieNode();

        for (var order = 0; order < dictionary.Words.Count; order++)
        {
            AddWord(dictionary.Words[order], order);
        }
    }

    /// <summary>
    /// Finds spelling corrections for a target word, prioritizing matches with 1 edit 
    /// before falling back to up to MaxEdits.
    /// </summary>
    /// <param name="word">The word to find corrections for.</param>
    public IReadOnlyList<string> FindCorrections(string word)
    {
        ArgumentNullException.ThrowIfNull(word);

        var oneEdit = FindCorrections(word, maxEdits: 1);
        return oneEdit.Count > 0
            ? oneEdit


            
            : FindCorrections(word, MaxEdits);
    }

    /// <summary>
    /// Performs a fuzzy search in the Trie tree allowing up to a specified maximum number of edit operations.
    /// </summary>
    /// <param name="word">The input word being analyzed.</param>
    /// <param name="maxEdits">The maximum allowed number of edit operations (deletions or insertions).</param>
    private IReadOnlyList<string> FindCorrections(string word, int maxEdits)
    {
        var matches = new List<Match>();
        var matchedOrders = new HashSet<int>();
        var visited = new HashSet<SearchState>();

        Search(
            _root,
            word,
            inputIndex: 0,
            edits: 0,
            lastEdit: EditType.None,
            maxEdits,
            matches,
            matchedOrders,
            visited);

        matches.Sort(static (left, right) =>
            left.DictionaryOrder.CompareTo(right.DictionaryOrder));

        return matches.Select(static match => match.Word).ToList();
    }

    /// <summary>
    /// Recursive depth-first search (DFS) algorithm that explores paths in the Trie 
    /// evaluating matching characters, single character deletions, and single character insertions.
    /// </summary>
    private static void Search(
        TrieNode node, //The current node in the Trie structure
        string word, //The source word being evaluated
        int inputIndex, //The current character index in the source word
        int edits, //The total count of edits performed so far
        EditType lastEdit, //The last type of edit operation executed
        int maxEdits, //The maximum allowed number of edits
        List<Match> matches, //The collection storing discovered valid candidate matches
        HashSet<int> matchedOrders, //A tracking set to prevent duplicate entries based on dictionary order
        HashSet<SearchState> visited) //A tracking set of state configurations to prevent redundant recursive paths
    {
        var state = new SearchState(node, inputIndex, edits, lastEdit);

        if (!visited.Add(state))
        {
            return;
        }

        if (inputIndex == word.Length)
        {
            if (node.Word is not null && matchedOrders.Add(node.DictionaryOrder))
            {
                matches.Add(new Match(node.Word, node.DictionaryOrder));
            }

            if (edits < maxEdits && lastEdit != EditType.Insert)
            {
                foreach (var child in node.Children.Values)
                {
                    Search(
                        child,
                        word,
                        inputIndex,
                        edits + 1,
                        EditType.Insert,
                        maxEdits,
                        matches,
                        matchedOrders,
                        visited);
                }
            }

            return;
        }

        // Consume a matching character without using an edit.
        var currentCharacter = char.ToLowerInvariant(word[inputIndex]);

        if (node.Children.TryGetValue(currentCharacter, out var matchingChild))
        {
            Search(
                matchingChild,
                word,
                inputIndex + 1,
                edits,
                EditType.None,
                maxEdits,
                matches,
                matchedOrders,
                visited);
        }

        if (edits == maxEdits)
        {
            return;
        }

        // Delete one input character.
        // A second deletion is only allowed after a matching character
        // or another edit has separated the deleted characters.
        if (lastEdit != EditType.Delete)
        {
            Search(
                node,
                word,
                inputIndex + 1,
                edits + 1,
                EditType.Delete,
                maxEdits,
                matches,
                matchedOrders,
                visited);
        }

        // Insert one dictionary character.
        // Two insertions in the same gap would affect adjacent characters,
        // so an insertion cannot immediately follow another insertion.
        if (lastEdit != EditType.Insert)
        {
            foreach (var child in node.Children.Values)
            {
                Search(
                    child,
                    word,
                    inputIndex,
                    edits + 1,
                    EditType.Insert,
                    maxEdits,
                    matches,
                    matchedOrders,
                    visited);
            }
        }
    }

    /// <summary>
    /// Inserts a word character by character into the internal Trie structure.
    /// </summary>
    /// <param name="word">The word to add to the Trie.</param>
    /// <param name="dictionaryOrder">The original 0-based index position of the word in the dictionary source.</param>
    private void AddWord(string word, int dictionaryOrder)
    {
        var current = _root;

        foreach (var character in word)
        {
            var normalized = char.ToLowerInvariant(character);

            if (!current.Children.TryGetValue(normalized, out var child))
            {
                child = new TrieNode();
                current.Children.Add(normalized, child);
            }

            current = child;
        }

        if (current.Word is null)
        {
            current.Word = word;
            current.DictionaryOrder = dictionaryOrder;
        }
    }

    private sealed class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; } = [];
        public string? Word { get; set; }
        public int DictionaryOrder { get; set; } = int.MaxValue;
    }

    private readonly record struct SearchState(
        TrieNode Node,
        int InputIndex,
        int Edits,
        EditType LastEdit);

    private readonly record struct Match(
        string Word,
        int DictionaryOrder);
}
