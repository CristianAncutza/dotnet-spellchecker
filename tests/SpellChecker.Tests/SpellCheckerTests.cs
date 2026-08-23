using SpellChecker.Domain;

namespace SpellChecker.Tests;

public class SpellCheckerTests
{
    private static SpellingChecker CreateChecker()
    {
        return new SpellingChecker(
            new DictionaryIndex(
            [
                "rain", "spain", "plain", "plaint", "pain", "main", "mainly",
                "the", "in", "on", "fall", "falls", "his", "was"
            ]));
    }

    [Fact]
    public void ExactWord_IsReturnedUnchanged()
    {
        var checker = CreateChecker();

        Assert.Equal("pain", checker.Correct("pain"));
    }

    [Fact]
    public void ExactWord_PreservesInputCasing()
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
    public void MultipleCorrections_KeepDictionaryOrder()
    {
        var checker = CreateChecker();

        Assert.Equal("{main mainly}", checker.Correct("mainy"));
    }

    [Fact]
    public void NoCorrection_UsesQuestionMarkFormat()
    {
        var checker = CreateChecker();

        Assert.Equal("{rame?}", checker.Correct("rame"));
    }

    [Fact]
    public void OneEditCorrections_HavePriorityOverTwoEdits()
    {
        var checker = new SpellingChecker(
            new DictionaryIndex(["mainly", "main"]));

        Assert.Equal("main", checker.Correct("mainy"));
    }

    [Fact]
    public void TwoAdjacentDeletions_AreForbidden()
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
    public void TwoAdjacentInsertions_AreForbidden()
    {
        var checker = new SpellingChecker(
            new DictionaryIndex(["abcd"]));

        Assert.Equal("{ad?}", checker.Correct("ad"));
    }

    [Fact]
    public void TwoNonAdjacentInsertions_AreAllowed()
    {
        var checker = new SpellingChecker(
            new DictionaryIndex(["abcde"]));

        Assert.Equal("abcde", checker.Correct("ace"));
    }

    [Fact]
    public void InsertAndDelete_AreAllowed()
    {
        var checker = new SpellingChecker(
            new DictionaryIndex(["the"]));

        Assert.Equal("the", checker.Correct("hte"));
    }
}
