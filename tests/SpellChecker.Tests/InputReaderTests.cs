using System.Text;
using SpellChecker.Infrastructure;

namespace SpellChecker.Tests;

public class InputReaderTests
{
    [Fact]
    public void ReadDictionary_StopsAtFirstSeparator()
    {
        var reader = new StringReader("one two\nthree\n===\ntext\n===\n");
        var inputReader = new InputReader();

        var words = inputReader.ReadDictionary(reader);

        Assert.Equal(["one", "two", "three"], words);
    }

    [Fact]
    public void ReadTextLines_PreservesWhitespace()
    {
        var reader = new StringReader("text\n===\n  one   two\tthree  \n===\n");
        var inputReader = new InputReader();

        _ = inputReader.ReadDictionary(reader);
        var lines = inputReader.ReadTextLines(reader).ToList();

        Assert.Single(lines);
        Assert.Equal("  one   two\tthree  ", lines[0]);
    }

    [Fact]
    public void MissingDictionarySeparator_Throws()
    {
        var reader = new StringReader("one two\n");
        var inputReader = new InputReader();

        Assert.Throws<InvalidDataException>(() => inputReader.ReadDictionary(reader));
    }

    [Fact]
    public void MissingTextSeparator_Throws()
    {
        var reader = new StringReader("one\n===\ntext\n");
        var inputReader = new InputReader();

        _ = inputReader.ReadDictionary(reader);

        Assert.Throws<InvalidDataException>(
            () => inputReader.ReadTextLines(reader).ToList());
    }

    [Fact]
    public void DictionaryWordLongerThan50Characters_Throws()
    {
        var word = new string('a', 51);
        var reader = new StringReader($"{word}\n===\n");
        var inputReader = new InputReader();

        Assert.Throws<InvalidDataException>(() => inputReader.ReadDictionary(reader));
    }

    [Fact]
    public void InvalidDictionaryCharacter_Throws()
    {
        var reader = new StringReader("hello!\n===\n");
        var inputReader = new InputReader();

        Assert.Throws<InvalidDataException>(() => inputReader.ReadDictionary(reader));
    }

    [Fact]
    public void InvalidTextCharacter_Throws()
    {
        var reader = new StringReader("hello\n===\nhello!\n===\n");
        var inputReader = new InputReader();

        _ = inputReader.ReadDictionary(reader);

        Assert.Throws<InvalidDataException>(
            () => inputReader.ReadTextLines(reader).ToList());
    }
}
