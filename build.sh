#!/usr/bin/env bash
set -euo pipefail

dotnet restore fitSharp.linux.slnf
dotnet build fitSharp.linux.slnf -c Release --no-restore
dotnet test fitSharp.linux.slnf -c Release --no-build -m:1
