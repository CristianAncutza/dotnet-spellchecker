using SpellChecker.Domain;

namespace SpellChecker.Tests;

public class SpellCheckerTests
{
    private static SpellingChecker CreateChecker()
    {
        var dictionary = new DictionaryIndex(
        [
            "rain",
            "spain",
            "plain",
            "plaint",
            "pain",
            "main",
            "mainly",
            "the",
            "in",
            "on",
            "fall",
            "falls",
            "his",
            "was"
        ]);

        return new SpellingChecker(dictionary);
    }

    [Fact]
    public void WordInDictionary_IsReturnedUnchanged()
    {
        var checker = CreateChecker();

        Assert.Equal("pain", checker.Correct("pain"));
    }

    [Fact]
    public void MatchingIsCaseInsensitive()
    {
        var checker = CreateChecker();

        Assert.Equal("PAIN", checker.Correct("PAIN"));
    }

    [Fact]
    public void OneEdit_Insertion()
    {
        var checker = CreateChecker();

        Assert.Equal("the", checker.Correct("hte"));
    }

    [Fact]
    public void OneEdit_Deletion()
    {
        var checker = CreateChecker();

        Assert.Equal("falls", checker.Correct("fells"));
    }

    [Fact]
    public void MultipleCorrections_AreReturnedInDictionaryOrder()
    {
        var checker = CreateChecker();

        Assert.Equal(
            "{main mainly}",
            checker.Correct("mainy"));
    }

    [Fact]
    public void NoCorrection_IsMarked()
    {
        var checker = CreateChecker();

        Assert.Equal(
            "{rame?}",
            checker.Correct("rame"));
    }

    [Fact]
    public void OneEditCorrections_ArePreferredOverTwoEdits()
    {
        var checker = new SpellingChecker(
            new DictionaryIndex(
            [
                "abc",
                "abcd",
                "abcefg"
            ]));

        Assert.Equal("abc", checker.Correct("ab"));
    }

    [Fact]
    public void TwoAdjacentDeletions_AreNotAllowed()
    {
        var checker = new SpellingChecker(
            new DictionaryIndex(["ad"]));

        Assert.Equal("{abcd?}", checker.Correct("abcd"));
    }

    [Fact]
    public void TwoNonAdjacentDeletions_AreAllowed()
    {
        var checker = new SpellingChecker(
            new DictionaryIndex(["ac"]));

        Assert.Equal("ac", checker.Correct("abcd"));
    }

    [Fact]
    public void TwoAdjacentInsertions_AreNotAllowed()
    {
        var checker = new SpellingChecker(
            new DictionaryIndex(["abcd"]));

        Assert.Equal("{ad?}", checker.Correct("ad"));
    }
}