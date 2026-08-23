using System.Diagnostics;
using SpellChecker.Domain;

namespace SpellChecker.Tests;

public class PerformanceSmokeTests
{
    [Fact]
    public void LargeDictionary_CanProcessManyQueries()
    {
        const int dictionarySize = 20_000;
        const int queryCount = 1_000;

        var dictionaryWords = Enumerable.Range(0, dictionarySize)
            .Select(index => $"word{index:D5}abcdefghijkl")
            .ToList();

        var checker = new SpellingChecker(new DictionaryIndex(dictionaryWords));
        var queries = Enumerable.Range(0, queryCount)
            .Select(index => $"zzzz{index:D6}abcdefghijkl")
            .ToList();

        var stopwatch = Stopwatch.StartNew();

        foreach (var query in queries)
        {
            _ = checker.Correct(query);
        }

        stopwatch.Stop();

        Assert.Equal(queryCount, queries.Count);
        Console.WriteLine(
            $"Performance smoke test: {dictionarySize:N0} dictionary words, " +
            $"{queryCount:N0} queries, {stopwatch.ElapsedMilliseconds:N0} ms.");
    }
}
