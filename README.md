# fitSharp

[![Build](https://github.com/jediwhale/fitsharp/actions/workflows/test.yml/badge.svg)](https://github.com/jediwhale/fitsharp/actions/workflows/test.yml)
[![NuGet](https://img.shields.io/nuget/v/FitSharp.svg)](https://www.nuget.org/packages/FitSharp)

fitSharp is a set of open-source functional testing tools for .NET, inspired by
the Framework for Integrated Test (Fit). It provides Fit, Slim, and DbFit
support and can run .NET fixtures from [FitNesse](https://fitnesse.org/FitNesse).

- [fitSharp documentation](https://fitsharp.github.io/)
- [FitNesse download](https://fitnesse.org/FitNesseDownload.html)
- [FitNesse user guide](https://fitnesse.org/FitNesse/UserGuide.html)
- [FitSharp on NuGet](https://www.nuget.org/packages/FitSharp)

## Install

Add the package to a .NET project:

```powershell
dotnet add package FitSharp
```

The package contains the fitSharp libraries and runners. `lib` holds the
compile references; `tools` holds the runners that FitNesse starts by path:

| Group | `lib/` | `tools/` |
|---|---|---|
| `net48` | `fitSharp`, `fit` | `Runner.exe`, `RunnerW.exe`, `fitSharp`, `fit` |
| `net10.0`, `net11.0` | `fitSharp`, `fit` | `Runner`, `fitSharp`, `fit` |
| `net10.0-windows7.0`, `net11.0-windows7.0` | `fitSharp`, `fit` | `Runner`, `RunnerW`, `fitSharp`, `fit` |

Runners in the `*-windows7.0` groups start on the Windows desktop runtime.

## Use with FitNesse

Configure FitNesse with a command pattern, a test runner, and a fitSharp suite
configuration file. For modern .NET:

```text
!define COMMAND_PATTERN {%m path\to\Runner.dll -c path\to\suite.config.xml %p}
!define TEST_RUNNER {dotnet}
```

For .NET Framework 4.8:

```text
!define COMMAND_PATTERN {%m -c path\to\suite.config.xml %p}
!define TEST_RUNNER {path\to\Runner.exe}
```

The suite configuration selects the Fit or Slim runner and identifies fixture
assemblies and namespaces. See
[Run tests with FitNesse](https://fitsharp.github.io/FitSharp/RunTestsWithFitnesse.html)
for the complete setup.

### WPF and WinForms fixtures

A fixture assembly that references WPF or WinForms only loads on the Windows
desktop runtime. Point the command pattern at `tools/net10.0-windows7.0/Runner.dll`
or the `net11.0-windows7.0` equivalent. The runner in the plain `tools/net10.0`
or `tools/net11.0` group starts on the console runtime and cannot load those
assemblies. `RunnerW` is the GUI alternative for debugging.

### Linux

The libraries and the console runner run on Linux; the Windows GUI runner does
not. Use the runner in `tools/net10.0` or `tools/net11.0`. The build and the full
story suite run on Linux in CI.

## Compatibility

Supported targets: .NET Framework 4.8 (Windows only), .NET 10 and .NET 11. The
package no longer contains .NET 8 or .NET 9 assets. The previous parallel Linux
projects and solution are removed; Linux builds from the main projects.

## Prerequisites

The build runs on Windows and Linux. Packaging requires Windows because the
desktop runner is Windows-only.

- .NET Framework 4.8 Developer Pack (Windows)
- .NET 10 SDK
- .NET 11 SDK

The repository's [`global.json`](global.json) pins the SDK, and the
[CI workflow](.github/workflows/test.yml) is the reference environment.

## Build and test

Run the build script from the repository root. It restores, builds in Release,
runs all unit and story tests, and, on Windows, creates the package:

```powershell
.\build.cmd
```

```bash
./build.sh
```

CI runs the same scripts. To work on a single project, use `dotnet build` or
`dotnet test` on that project directly.

## Package

The version comes from [`Directory.Build.props`](Directory.Build.props). The
build script writes `artifacts\FitSharp.<version>.nupkg` and a matching symbols
package. To pack without the full build:

```powershell
dotnet pack packaging\FitSharp.Package.csproj -c Release -o artifacts
```

## Release

Set the version in [`Directory.Build.props`](Directory.Build.props), commit,
and push a tag with the same value:

```bash
git tag 2025.11.17
git push origin 2025.11.17
```

The publish workflow builds, tests, packs and pushes the package and its
symbols to NuGet. It stops if the tag and the version do not match. Running the
workflow manually builds and packs without publishing.

If automated publishing is unavailable, pack locally and push manually:

```powershell
.\build.cmd
dotnet nuget push artifacts\FitSharp.*.nupkg --api-key <key> --source https://api.nuget.org/v3/index.json
```

## Develop

Open [`fitSharp.slnx`](fitSharp.slnx) in Visual Studio or use the commands above.
The main projects are:

- `source\fitSharp`: shared runtime and infrastructure
- `source\fit`: Fit and DbFit implementation
- `source\Runner`: command-line runner
- `source\RunnerW`: Windows GUI runner
- `source\fitSharpTest`, `source\fitTest`, and `source\StoryTest`: automated tests
- `source\TestTargetWpf` and `source\RunnerDesktopTest`: prove that the Windows runner loads WPF-dependent fixtures
- `packaging\FitSharp.Package.csproj`: package definition
- `tools\fitnesse`: fetch a local FitNesse server

To debug Slim fixtures launched by FitNesse, set `TEST_RUNNER` to `RunnerW.exe`,
start the test, attach the debugger while RunnerW waits, and then select **Go**.
Use a `SLIM_PORT` other than the default value `1`. See
[Debugging fixtures](https://fitsharp.github.io/Slim/DebuggingFixtures.html)
for details.

To run a local FitNesse server, fetch the current standalone jar into the
ignored `.fitnesse` folder:

```powershell
powershell -File tools/fitnesse/fetch-fitnesse.ps1
```

```bash
./tools/fitnesse/fetch-fitnesse.sh
```

Then start it with `java -jar .fitnesse/fitnesse-<version>-standalone.jar`. Both
scripts verify the published checksum; pass a version to pin one:

```bash
./tools/fitnesse/fetch-fitnesse.sh .fitnesse 20260313
```

```powershell
powershell -File tools/fitnesse/fetch-fitnesse.ps1 -Version 20260313
```

The automated story suite does not need FitNesse; it runs the folder runner
directly.

## License

Work derived from the original Fit, FitLibrary, and DBFit projects is covered by
the [GNU General Public License](GPLicense.txt). This includes `fit.dll` and
`dbfit*.dll`.

Original work on fitSharp is covered by the
[Common Public License](CPLicense.txt). This includes the other executables.
