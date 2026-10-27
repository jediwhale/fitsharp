@echo off
setlocal EnableDelayedExpansion

dotnet restore fitSharp.sln || exit /b !errorlevel!
dotnet build fitSharp.sln -c Release --no-restore || exit /b !errorlevel!
dotnet test fitSharp.sln -c Release --no-build -m:1 || exit /b !errorlevel!
dotnet pack packaging\FitSharp.Package.csproj -c Release --no-build -o artifacts || exit /b !errorlevel!

endlocal
