# Spell Checker

A C#/.NET 10 console application that implements the spell-checking assignment provided by 

The application reads a dictionary and a text from an input file, identifies words that are not present in the dictionary, and replaces them with valid corrections according to the assignment rules.

## Requirements

* .NET 10 SDK

Verify the installed SDK with:

```bash
dotnet --version
```

## Solution Structure

```text
SpellChecker/
│
├── SpellChecker.slnx
│
├── src/
│   └── SpellChecker/
│       ├── Program.cs
│       │
│       ├── Application/
│       │   └── SpellCheckerService.cs
│       │
│       ├── Domain/
│       │   ├── SpellChecker.cs
│       │   ├── CorrectionFinder.cs
│       │   ├── DictionaryIndex.cs
│       │   └── EditType.cs
│       │
│       └── Infrastructure/
│           ├── InputReader.cs
│           └── OutputWriter.cs
│
├── tests/
│   └── SpellChecker.Tests/
│       ├── DictionaryIndexTests.cs
│       ├── SpellCheckerTests.cs
│       └── CorrectionFinderTests.cs
│
├── test-data/
│   ├── sample.in
│   └── sample.out
│
└── README.md
```

## Assignment Rules

A correction may be at most two edits away from the input word.

An edit is one of:

* inserting a single letter;
* deleting a single letter.

When both edits are insertions, or both edits are deletions, the affected characters may not be adjacent.

The input is case-insensitive. Corrections are printed using the casing stored in the dictionary, while words already present in the dictionary are printed using their original casing from the input.

If a word is not in the dictionary:

1. If there are no valid corrections, `{word?}` is printed.
2. If at least one one-edit correction exists, all two-edit corrections are ignored.
3. If exactly one correction remains, that word is printed.
4. If multiple corrections remain, they are printed as `{word1 word2 ...}` in dictionary order.

Whitespace in the input text is preserved.

## Design

The solution is intentionally kept small and separated into a few focused components.

### `DictionaryIndex`

Stores dictionary words and provides fast case-insensitive lookups.

The original dictionary spelling is preserved so that corrections can be printed using the exact casing provided by the input dictionary.

### `CorrectionFinder`

Contains the main correction algorithm.

The dictionary is represented internally as a Trie. During the search, the algorithm walks through the Trie and the input word simultaneously.

At each position it can perform one of three actions:

```text
Match
Delete
Insert
```

The search is limited to two edits.

The previous edit type is tracked so that two consecutive insertions or two consecutive deletions are not allowed.

One-edit corrections are searched first. A two-edit search is only performed when no one-edit correction exists.

### `SpellChecker`

Applies the business rules around corrections:

```text
word in dictionary
        |
        +-- yes --> return original word
        |
        +-- no --> find corrections
                      |
                      +-- none --> {word?}
                      +-- one --> correction
                      +-- many --> {word1 word2 ...}
```

### `InputReader`

Reads the dictionary and text sections from the input file.

The dictionary ends at the first:

```text
===
```

The text ends at the second:

```text
===
```

The text itself is processed line by line so that large input files do not need to be loaded entirely into memory.

### `OutputWriter`

Writes corrected text to the output file.

### `SpellCheckerService`

Coordinates the input reader, dictionary, spell checker, and output writer.

## Algorithm

A Trie is used to avoid comparing every input word against every dictionary word character-by-character.

For each word:

1. Search for corrections requiring zero or one edit.
2. If one-edit corrections exist, return them.
3. Otherwise search for corrections requiring up to two edits.
4. Preserve dictionary order when returning multiple corrections.

The search state contains:

```text
current Trie node
input word position
number of edits used
previous edit type
```

This allows the adjacency restriction to be enforced during traversal.

### Case Handling

The Trie stores characters using `char.ToLowerInvariant`.

The original dictionary word is stored separately in the terminal Trie node.

For example, a dictionary entry:

```text
Hello
```

matches:

```text
hello
HELLO
HeLlO
```

but the correction returned from the dictionary is:

```text
Hello
```

## Complexity

Let:

* `L` be the maximum word length;
* `D` be the number of dictionary words.

Dictionary construction is proportional to the total number of characters stored in the dictionary.

Correction searching is bounded by a maximum of two edits and by the Trie traversal. The small edit bound significantly limits the search space compared with a full unrestricted edit-distance algorithm.

The implementation also avoids a traditional Levenshtein distance calculation because the assignment does not allow substitutions. Only insertions and deletions are valid operations.

## Input Format

The dictionary is provided first in free format and ends with:

```text
===
```

The text follows and ends with another:

```text
===
```

Example:

```text
rain spain plain plaint pain main mainly
the in on fall falls his was
===
hte rame in pain fells
mainy oon teh lain
was hints pliant
===
```

## Output Format

For the example above, the expected output is:

```text
the {rame?} in pain falls
{main mainly} on the plain
was {hints?} plaint
```

## Build

From the solution root:

```bash
dotnet restore
dotnet build
```

## Run Tests

```bash
dotnet test
```

## Run the Application

The application expects two command-line arguments:

```text
<input-file> <output-file>
```

Example:

```bash
dotnet run \
  --project src/SpellChecker \
  -- test-data/sample.in \
  test-data/sample.out
```

The generated output can then be compared with `test-data/sample.out`.

## Error Handling

The application handles common command-line and file errors, including:

* invalid argument count;
* missing input files;
* unauthorized file access;
* invalid input structure;
* general I/O errors.

Errors are written to standard error and the application returns a non-zero exit code.

## Testing

The test suite covers the main business rules, including:

* exact dictionary matches;
* case-insensitive dictionary lookup;
* preservation of dictionary casing;
* insertion corrections;
* deletion corrections;
* multiple valid corrections;
* no valid correction;
* preference for one-edit corrections;
* restrictions on consecutive insertions;
* restrictions on consecutive deletions;
* valid non-adjacent edits.

The sample input/output from the assignment is also used as an integration scenario.

## Design Decisions

The solution intentionally avoids unnecessary frameworks and infrastructure.

There is no database, ORM, dependency-injection framework, or external service because none is required by the problem.

The application is split into a small number of components with clear responsibilities while keeping the overall solution straightforward to compile, test, inspect, and extend.

The main algorithm is isolated in `CorrectionFinder`, making it possible to replace or optimize the search strategy without changing file handling or the higher-level spell-checking rules.

## Possible Future Improvements

Potential improvements, depending on the expected input size, include:

* streaming the dictionary instead of storing all dictionary words;
* additional indexing by word length;
* benchmarking with large dictionaries and text files;
* further reducing duplicate Trie search states through memoization;
* adding dedicated performance tests.

These optimizations should only be introduced when supported by profiling or input-size requirements, keeping the implementation simple unless additional complexity provides a measurable benefit.
