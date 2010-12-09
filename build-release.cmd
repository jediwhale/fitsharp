@echo off

REM This build script is not necessarily for general consumption.
REM We use it internally to build fitSharp for .NET 3.5 and 4.0, in Release and targetted for x86, which
REM happens to be a local requirement for our system under test.

call build.cmd /p:TargetFrameworkVersion=v3.5;PlatformTarget=x86;Config=Release
call build.cmd /p:TargetFrameworkVersion=v4.0;PlatformTarget=x86;Config=Release
