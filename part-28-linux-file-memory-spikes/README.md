# Part 28: Diagnosing Linux File Operation Memory Spikes

Article draft: `28-modernizing-net-linux-file-memory-spikes.md`

This sample demonstrates the practical code side of the draft article: compare a naive recursive backup approach that loads whole files into memory with an optimized approach that excludes rebuildable folders, streams file contents, and limits parallelism.

## Sample Focus

- Generate a realistic test tree with many small files and a few larger files.
- Run a naive backup that uses `File.ReadAllBytes` / `File.WriteAllBytes`.
- Run an optimized backup that skips rebuildable folders and uses streaming copy.
- Capture elapsed time, files copied, bytes copied, and peak managed memory.

## Implementation Notes

- The sample does not try to emulate CIFS/SMB `serverino` behavior locally.
- It focuses on the code changes you can control in the app layer.
- The excluded folders mirror the article: `PromptMetadata`, `TempCache`, and `RebuildableData`.
- The optimized backup uses bounded parallelism to reduce memory pressure.

## Structure

```text
src/LinuxFileMemorySpikesSample
tests/LinuxFileMemorySpikesSample.Tests
```

## Run

Generate sample data:

```powershell
dotnet run --project .\src\LinuxFileMemorySpikesSample -- generate
```

Run the naive backup:

```powershell
dotnet run --project .\src\LinuxFileMemorySpikesSample -- backup-naive
```

Run the optimized backup:

```powershell
dotnet run --project .\src\LinuxFileMemorySpikesSample -- backup-optimized
```

Run both and compare:

```powershell
dotnet run --project .\src\LinuxFileMemorySpikesSample -- compare
```

## Notes

- This sample is intentionally synthetic, but it demonstrates the same failure pattern: whole-file buffering and oversized backup scope create more allocation pressure than necessary.
- The infrastructure recommendation from the article still matters in production: if you are using SMB/CIFS on Linux, verify mount options such as `serverino` separately.
