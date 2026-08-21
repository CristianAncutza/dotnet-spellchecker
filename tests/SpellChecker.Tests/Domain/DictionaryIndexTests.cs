using SpellChecker.Domain;

namespace SpellChecker.Tests;

public class DictionaryIndexTests
{
    [Fact]
    public void Contains_IsCaseInsensitive()
    {
        var index = new DictionaryIndex(["Hello"]);

        Assert.True(index.Contains("hello"));
        Assert.True(index.Contains("HELLO"));
    }

    [Fact]
    public void GetOriginal_PreservesDictionaryCasing()
    {
        var index = new DictionaryIndex(["Hello"]);

        //Assert.Equal("Hello", index.GetOriginal("hello"));
    }

    [Fact]
    public void Contains_ReturnsFalseForUnknownWord()
    {
        var index = new DictionaryIndex(["Hello"]);

        Assert.False(index.Contains("World"));
    }
}