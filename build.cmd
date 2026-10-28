@echo off
setlocal EnableDelayedExpansion

dotnet restore fitSharp.slnx || exit /b !errorlevel!
dotnet build fitSharp.slnx -c Release --no-restore || exit /b !errorlevel!
dotnet test fitSharp.slnx -c Release --no-build -m:1 || exit /b !errorlevel!
dotnet pack packaging\FitSharp.Package.csproj -c Release --no-build -o artifacts || exit /b !errorlevel!

endlocal
