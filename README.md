# fitSharp

[![Tests](https://github.com/jediwhale/fitsharp/actions/workflows/test.yml/badge.svg)](https://github.com/jediwhale/fitsharp/actions/workflows/test.yml)
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

The package contains the fitSharp libraries and runners. FitNesse starts the
appropriate runner to execute fixtures in your .NET assemblies.

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

## Prerequisites

The canonical build runs on Windows and targets:

- .NET Framework 4.8
- .NET 8+
- Windows-specific variants of .NET 8+

Install the .NET Framework 4.8 Developer Pack and the .NET 8 through 11 SDKs.
The repository's [`global.json`](global.json) currently pins a prerelease .NET
11 SDK and allows roll-forward to a later major prerelease SDK. The
[CI workflow](.github/workflows/test.yml) is the reference environment.

## Build and test

Run commands from the repository root:

```powershell
# Restore
dotnet msbuild -target:restore .\build.proj

# Build the solution in Debug configuration
dotnet msbuild -target:solution .\build.proj

# Run unit tests
dotnet msbuild -target:unittests .\build.proj

# Run story tests
dotnet msbuild -target:storytests .\build.proj
```

The test targets build the solution before running their tests. Each test
command uses `dotnet test -m:1` to match CI.

## Package

Release versions are maintained manually in
[`nuget\FitSharp.nuspec`](nuget/FitSharp.nuspec). Update the version there,
then create the release build and NuGet package with:

```powershell
dotnet msbuild -target:package .\build.proj
```

The package target restores and builds the solution in Release configuration,
recreates the package staging directory, validates the required build outputs,
and writes `nuget\FitSharp.<version>.nupkg`.

Deploying creates the package and copies it to `binary`:

```powershell
dotnet msbuild -target:deploy .\build.proj
```

## Develop

Open [`fitSharp.sln`](fitSharp.sln) in Visual Studio or use the commands above.
The main projects are:

- `source\fitSharp`: shared runtime and infrastructure
- `source\fit`: Fit and DbFit implementation
- `source\Runner`: command-line runner
- `source\RunnerW`: Windows GUI runner
- `source\fitSharpTest`, `source\fitTest`, and `source\StoryTest`: automated tests

To debug Slim fixtures launched by FitNesse, set `TEST_RUNNER` to `RunnerW.exe`,
start the test, attach the debugger while RunnerW waits, and then select **Go**.
Use a `SLIM_PORT` other than the default value `1`. See
[Debugging fixtures](https://fitsharp.github.io/Slim/DebuggingFixtures.html)
for details.

## License

Work derived from the original Fit, FitLibrary, and DBFit projects is covered by
the [GNU General Public License](GPLicense.txt). This includes `fit.dll` and
`dbfit*.dll`.

Original work on fitSharp is covered by the
[Common Public License](CPLicense.txt). This includes the other executables.
