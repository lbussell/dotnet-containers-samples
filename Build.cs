// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

#:package ConsoleAppFramework@5.6.1
#:project src/Generator/Generator.csproj

using System.Text.Json;
using System.Text.Json.Serialization;

using ConsoleAppFramework;
using Generator;
using static Generator.Exec.Exec;

var app = ConsoleApp.Create();
app.Add<Build>();
app.Run(args);

class Build
{
    private const string RegistryName = "dotnet-samples-registry";
    private const string RegistryPort = "5001";
    private const string Registry = $"localhost:{RegistryPort}";
    private const string RegistryImage = "ghcr.io/project-zot/zot";
    private const string SamplesDir = "samples";
    private const string ReportTemplateFile = "src/Templates/Report.md";
    private const string ReportFile = "README.Report.md";

    private readonly SampleDefinition[] _samples = [
        new(ParentDirectory: SamplesDir, TemplateName: "container-console", Name: "ConsoleApp",              PublishType.FrameworkDependent, Distroless: false,  Globalization: false, Description: "Framework-dependent console app"),
        new(ParentDirectory: SamplesDir, TemplateName: "container-console", Name: "ConsoleAppDistroless",    PublishType.FrameworkDependent, Distroless: true,   Globalization: false, Description: "Framework-dependent console app with distroless base image"),
        new(ParentDirectory: SamplesDir, TemplateName: "container-console", Name: "ConsoleAppSelfContained", PublishType.SelfContained,      Distroless: false,  Globalization: false, Description: "Self-contained console app with trimming and ReadyToRun"),
        new(ParentDirectory: SamplesDir, TemplateName: "container-console", Name: "ConsoleAppNativeAot",     PublishType.NativeAot,          Distroless: false,  Globalization: false, Description: "Native AOT console app"),
        new(ParentDirectory: SamplesDir, TemplateName: "container-console", Name: "ConsoleAppDistrolessAot", PublishType.NativeAot,          Distroless: true,   Globalization: false, Description: "Distroless native AOT console app"),
    ];

    public async Task Install() => await RunAsync("dotnet new install ./src/Templates/ConsoleApp/ --force");

    public async Task StartRegistry() => await RunAsync($"docker run --rm -d -p {RegistryPort}:5000 --name {RegistryName} {RegistryImage}");

    public async Task StopRegistry() => await RunAsync($"docker stop {RegistryName}");

    public async Task BuildSamples(bool noCache = false)
    {
        var generator = new GeneratorApp();
        await generator.BuildSamples(_samples, Registry, noCache);
    }

    public async Task GenerateMarkdown()
    {
        var generator = new GeneratorApp();
        await generator.GenerateReport(_samples, Registry, ReportTemplateFile, ReportFile);
    }

    public async Task GenerateSamples()
    {
        foreach (var sample in _samples)
        {
            await RunAsync("dotnet", ["new", sample.TemplateName, "--force", ..sample.GetOptions()],
                onStandardOutput: Console.WriteLine,
                onStandardError: Console.Error.WriteLine,
                logCommand: cmd => Console.WriteLine($"Running command: {cmd}"));
        }
    }
}
