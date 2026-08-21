using SpellChecker.Application;
using SpellChecker.Infrastructure;

if (args.Length != 2)
{
    Console.Error.WriteLine(
        "Usage: SpellChecker <input-file> <output-file>");

    return 1;
}

try
{
    var service = new SpellCheckerService(
        new InputReader(),
        new OutputWriter());

    service.Process(args[0], args[1]);

    return 0;
}
catch (FileNotFoundException ex)
{
    Console.Error.WriteLine($"Input file not found: {ex.FileName}");
    return 1;
}
catch (UnauthorizedAccessException)
{
    Console.Error.WriteLine("Access to the specified file was denied.");
    return 1;
}
catch (InvalidDataException ex)
{
    Console.Error.WriteLine($"Invalid input: {ex.Message}");
    return 1;
}
catch (IOException ex)
{
    Console.Error.WriteLine($"I/O error: {ex.Message}");
    return 1;
}