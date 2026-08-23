using SpellChecker.Domain;

namespace SpellChecker.Tests;

public class CorrectionFinderTests
{
    [Fact]
    public void Finder_MatchesIndependentReference_ForSmallAlphabet()
    {
        var words = GenerateWords("ab", maxLength: 4).ToList();

        foreach (var input in words)
        {
            var dictionaryWords = words
                .Where(word => !string.Equals(word, input, StringComparison.Ordinal))
                .ToList();

            var finder = new CorrectionFinder(new DictionaryIndex(dictionaryWords));
            var actual = finder.FindCorrections(input);
            var expected = ReferenceCorrections(input, dictionaryWords);

            Assert.Equal(expected, actual);
        }
    }

    private static IReadOnlyList<string> ReferenceCorrections(
        string source,
        IReadOnlyList<string> dictionary)
    {
        var visited = new HashSet<State>();
        var queue = new Queue<State>();
        var minimumDistance = new Dictionary<string, int>(StringComparer.Ordinal);

        var initial = new State(source, 0, EditType.None);
        visited.Add(initial);
        queue.Enqueue(initial);
        minimumDistance[source] = 0;

        while (queue.Count > 0)
        {
            var state = queue.Dequeue();

            if (state.Edits == 2)
            {
                continue;
            }

            if (state.LastEdit != EditType.Insert)
            {
                for (var position = 0; position <= state.Value.Length; position++)
                {
                    foreach (var character in "ab")
                    {
                        var next = state.Value.Insert(position, character.ToString());
                        Enqueue(next, state.Edits + 1, EditType.Insert);
                    }
                }
            }

            if (state.LastEdit != EditType.Delete)
            {
                for (var position = 0; position < state.Value.Length; position++)
                {
                    var next = state.Value.Remove(position, 1);
                    Enqueue(next, state.Edits + 1, EditType.Delete);
                }
            }
        }

        var oneEditExists = dictionary.Any(
            word => minimumDistance.TryGetValue(word, out var distance) && distance == 1);

        return dictionary
            .Where(word =>
                minimumDistance.TryGetValue(word, out var distance) &&
                distance <= 2 &&
                (!oneEditExists || distance == 1))
            .ToList();

        void Enqueue(string value, int edits, EditType lastEdit)
        {
            var state = new State(value, edits, lastEdit);

            if (!visited.Add(state))
            {
                return;
            }

            if (!minimumDistance.TryGetValue(value, out var existing) || edits < existing)
            {
                minimumDistance[value] = edits;
            }

            queue.Enqueue(state);
        }
    }

    private static IEnumerable<string> GenerateWords(string alphabet, int maxLength)
    {
        yield return string.Empty;

        for (var length = 1; length <= maxLength; length++)
        {
            foreach (var word in GenerateWordsOfLength(alphabet, length))
            {
                yield return word;
            }
        }
    }

    private static IEnumerable<string> GenerateWordsOfLength(string alphabet, int length)
    {
        if (length == 0)
        {
            yield return string.Empty;
            yield break;
        }

        foreach (var prefix in GenerateWordsOfLength(alphabet, length - 1))
        {
            foreach (var character in alphabet)
            {
                yield return prefix + character;
            }
        }
    }

    private readonly record struct State(
        string Value,
        int Edits,
        EditType LastEdit);
}
