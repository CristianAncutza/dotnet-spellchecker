using SpellChecker.Application;
using SpellChecker.Infrastructure;

namespace SpellChecker.Tests;

public class IntegrationTests
{
    [Fact]
    public void SampleInput_ProducesExpectedOutput()
    {
        var inputPath = Path.Combine(AppContext.BaseDirectory, "TestData", "sample.in");
        var expectedPath = Path.Combine(AppContext.BaseDirectory, "TestData", "sample.out");
        var outputPath = Path.Combine(Path.GetTempPath(), $"cqg-{Guid.NewGuid():N}.out");

        try
        {
            new SpellCheckerService(
                    new InputReader(),
                    new OutputWriter())
                .Process(inputPath, outputPath);

            var actual = File.ReadAllText(outputPath);
            var expected = File.ReadAllText(expectedPath);

            Assert.Equal(expected, actual);
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }
}
