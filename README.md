# Spell Checker

.NET 10 console application for the CQG spell-checker assignment.

## Requirements

- .NET 10 SDK

## Build

```bash
dotnet build
```

## Test

```bash
dotnet test
```

The test suite contains unit tests, an integration test for the supplied example, exhaustive small-alphabet verification of the correction search, invalid-input tests, and a performance smoke test.

## Run

```bash
dotnet run --project src/SpellChecker -- input.txt output.txt
```

## Design

The solution is intentionally small:

- `DictionaryIndex` provides case-insensitive exact lookup while preserving dictionary order and casing.
- `CorrectionFinder` stores the dictionary in a Trie and searches at most two edits from the input word.
- `SpellChecker` applies the assignment's output rules.
- `InputReader` validates the input format and streams the text section line by line.
- `OutputWriter` isolates output handling.
- `SpellCheckerService` coordinates the application flow.

## Correction search

Only insertion and deletion are allowed. A search state contains the Trie node, input position, number of edits, and previous edit type. A second consecutive insertion or deletion is rejected, which enforces the assignment's adjacency restriction for the two-edit search.

The algorithm first searches for one-edit corrections. Two-edit corrections are searched only when no one-edit correction exists, matching the required priority rule.

The search memoizes visited states to avoid exploring the same `(Trie node, input position, edits, previous edit)` state more than once.

## Input validation

The reader rejects:

- missing dictionary or text terminators;
- words longer than 50 characters;
- non-letter characters inside words.

Text whitespace is preserved in the output.
