# Data Extractor

A .NET 10 console application that reads a bank's CSV file and writes our own CSV
containing only the fields we need. Each bank is described by one small profile class,
so supporting a new bank means adding one class and changing nothing else.
```
DataExtraction <bank name> <input file>
```
Example:
```
DataExtraction Barclays barclays_input.csv
```
The result is written next to the input file as `<input name>_output.csv` (here: `barclays_input_output.csv`).

## Getting started

**Requirements:** the [.NET 10 SDK](https://dotnet.microsoft.com/download) (pinned in `global.json`).

```bash
# run the tests
dotnet test

# run the application on the sample file
dotnet run --project src/DataExtractor.Console -- Barclays samples/barclays_input.csv
```

| Situation | Result | Exit code |
|---|---|---|
| Success | `Wrote <path>` | 0 |
| Wrong number of arguments | Usage line | 1 |
| Unknown bank | Message listing the known banks | 1 |
| Missing file or missing column | Short error message, partial output deleted | 1 |
| Malformed CSV row | Short error message, partial output deleted | 1 |

To get a standalone executable:

```bash
dotnet publish src/DataExtractor.Console -c Release -o publish
./publish/DataExtraction Barclays samples/barclays_input.csv # DataExtraction.exe on Windows
```

## What it does for Barclays

- Skips the junk first line (`TimeZone=UTC`) so the real header is read.
- Copies `ISIN`, `CFICode` and `Venue` unchanged.
- Extracts `ContractSize` from the `PriceMultiplier` key inside `AlgoParams`
(for example `...PriceMultiplier:25.0|;...` gives `25.0`).
- Removes repeated output rows.

```
ISIN,CFICode,Venue,ContractSize
DE000C4SA5W8,FFICSX,XEUR,25.0
O:EVH\U20\12.5,OPASPS,XCBO,100.0
```

The result matches `samples/barclays_expected_output.csv` row for row (an end-to-end test
compares them).

## Assumptions

- **Repeated output rows are removed.** The sample input has 98 data rows and the sample
output has 53, and the difference is exactly the repeated rows. The first occurrence is
kept and the original order is preserved.
- **The output header is `ContractSize`**, matching the example output file (the brief text
says "Contract Size"). It is a one-line change in `BarclaysProfile` if you prefer the
other spelling.
- **The output file name is not specified**, so it is written next to the input as
`<input name>_output.csv`.
- **The Barclays input has a leading `TimeZone=UTC` line.** The Barclays profile skips the
first line before parsing so the real header is read.
- **Missing data:** if a row has no `PriceMultiplier`, its `ContractSize` is left empty. If
a column the profile needs is missing from the file, the application stops with a message
naming the column.


## Architecture (Hexagonal / Ports and Adapters)

The business logic sits in the middle and depends on nothing but its own interfaces
(ports). Everything else plugs in from outside through adapters. Dependencies point inward.

```
+------------------------ Core ------------------------+
Console ---> | ExtractionService BankProfileRegistry |
(Program.cs) | SimpleFieldExtractor KeyValueFieldExtractor |
| |
| Ports: IRecordReader <---- CsvHelperRecordReader |
| IRecordWriter <---- CsvHelperRecordWriter |
| IBankProfile <---- BarclaysProfile |
+------------------------------------------------------+
(adapters live in Infrastructure)
```

| Project | Role | References |
|---|---|---|
| `DataExtractor.Core` | The hexagon: ports, use case, extractors. No NuGet packages | nothing |
| `DataExtractor.Infrastructure` | Adapters: CsvHelper reader and writer, bank profiles, DI wiring | Core |
| `DataExtractor.Console` | Single entry point (`Program.cs`) | Infrastructure |
| `DataExtractor.Tests` | xUnit tests | Infrastructure (and Core) |

Because Core has no references, it cannot depend on CsvHelper or the file system. The
`ExtractionService` tests use hand-written fakes for the reader and writer, so the whole core is tested with no files and no CSV library.

### Ports

| Port | Implemented by | Purpose |
|---|---|---|
| `IRecordReader` | `CsvHelperRecordReader` | Turns CSV text into rows keyed by header name |
| `IRecordWriter` | `CsvHelperRecordWriter` | Writes headers and rows as CSV |
| `IBankProfile` | one class per bank | Bank name, output fields, optional file pre-processing |
| `IFieldExtractor` | `SimpleFieldExtractor`, `KeyValueFieldExtractor` | Produces one output column from one input row |

### How one run flows

1. `Program` reads the two arguments and builds the DI container.
2. `BankProfileRegistry` finds the bank's profile by name (case-insensitive).
3. The profile's `PreProcess` repairs the file (Barclays drops its first line).
4. The reader streams rows as dictionaries.
5. Each field extractor picks or parses its column.
6. Repeated output rows are dropped.
7. The writer writes the output file.

Rows are streamed from reader to writer, so memory use stays low. Only the set of unique
rows seen is held in memory.

## Adding a new bank

1. Add one class in `src/DataExtractor.Infrastructure/Banks/` that implements `IBankProfile`.
2. List its output fields with the existing extractors.
3. Override `PreProcess` only if the bank's input requires preprocessing before CSV parsing.

That is all. DI scans the assembly and registers every `IBankProfile`, and the registry finds
it by name. No existing file changes. Example (an invented bank with different column names)


## Design decisions

- **CsvHelper** reads and writes CSV, so there is no manual CSV parsing. Backslashes and colons
in ISINs pass through unchanged. CsvHelper errors are translated into
`InvalidDataException`, so a malformed file gives a clear message instead of a stack trace.
- **Regex** extracts a value from `Key:Value|;` pairs. A look-behind ignores junk digits
glued to the first key and avoids matching a key that merely ends with the same name.
- **Contract size is kept as text**, so values such as `25.0` appear exactly as in the source,
with no rounding or culture effects.
- **One profile class per bank** (not a config file) because a bank may need bespoke
pre-processing. A profile could still read simple settings from configuration later.
- **No switch statement:** the registry is a dictionary lookup over the profiles that DI
discovers.

## Testing

```bash
dotnet test
```

- Unit tests for each extractor, the registry, the CSV adapters and `BarclaysProfile`.
- Service tests using fakes for the ports.
- End-to-end tests through DI with the real adapters, including one that runs
`samples/barclays_input.csv` and compares the whole result, header included, with
`samples/barclays_expected_output.csv`.
- A test shows that a brand-new bank can be plugged in without changing the core.

Test names follow `Method_ExpectedBehaviour`, with `_WhenCondition` where it helps. Core
features were built test-first, with Red and Green commits on `feature/*` branches; see
`git log --graph`.

## Known limitations and possible improvements

- The output path is fixed. An optional third argument would make it configurable.
- Removing repeated rows is always on for every bank. A bank that needs repeated rows kept
would need a per-profile switch.
- `Program.cs` has no unit test of its own; moving its body into a small testable method
would fix that.
- One CSV reader serves all banks, so a bank with a different delimiter or a non-CSV file
would need a per-profile reader.
- `Program` calls `ExtractionService` directly. A strict hexagon would put an
`IExtractionService` port in front of it.
- On failure the partly written output file is deleted. Writing to a temporary file and
renaming it on success would avoid ever touching an older output.
- Possible additions: structured logging, a rejects file for bad rows, and a CI pipeline.



## Repository layout

```
README.md
global.json pins the .NET SDK
Directory.Build.props shared project settings
DataExtractor.slnx
samples/ barclays_input.csv, barclays_expected_output.csv
src/
DataExtractor.Core/ Ports/, Extractors/, ExtractionService, BankProfileRegistry
DataExtractor.Infrastructure/ Banks/, CsvHelper adapters, DependencyInjection
DataExtractor.Console/ Program.cs
tests/
DataExtractor.Tests/
```
