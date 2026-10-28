#!/usr/bin/env bash
set -euo pipefail

dotnet restore fitSharp.linux.slnx
dotnet build fitSharp.linux.slnx -c Release --no-restore
dotnet test fitSharp.linux.slnx -c Release --no-build -m:1
