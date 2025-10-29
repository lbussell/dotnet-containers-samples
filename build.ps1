#!/usr/bin/env pwsh
param([string]$Command = "--help")

dotnet build ./Build.cs
dotnet run ./Build.cs -- $Command
