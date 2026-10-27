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

The build runs on Windows. Linux builds the libraries and runners without the
Windows desktop runner. Targeted frameworks:

- .NET Framework 4.8 (Windows only)
- .NET 10
- .NET 11

Install the .NET Framework 4.8 Developer Pack on Windows, and the .NET 10 and
.NET 11 SDKs. The repository's [`global.json`](global.json) pins the SDK. The
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

To run a local FitNesse server, fetch the current standalone jar into the
ignored `.fitnesse` folder:

```powershell
powershell -File tools\fitnesse\fetch-fitnesse.ps1
```

```bash
./tools/fitnesse/fetch-fitnesse.sh
```

Then start it with `java -jar .fitnesse/fitnesse-<version>-standalone.jar`. The
automated story suite does not need FitNesse; it runs the folder runner
directly.

## License

Work derived from the original Fit, FitLibrary, and DBFit projects is covered by
the [GNU General Public License](GPLicense.txt). This includes `fit.dll` and
`dbfit*.dll`.

Original work on fitSharp is covered by the
[Common Public License](CPLicense.txt). This includes the other executables.
