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
        new("ConsoleApp",              PublishType.FrameworkDependent, Distroless: false,  Globalization: false, Description: "Framework-dependent console app"),
        new("ConsoleAppDistroless",    PublishType.FrameworkDependent, Distroless: true,   Globalization: false, Description: "Framework-dependent console app with distroless base image"),
        new("ConsoleAppSelfContained", PublishType.SelfContained,      Distroless: false,  Globalization: false, Description: "Self-contained console app with trimming and ReadyToRun"),
        new("ConsoleAppNativeAot",     PublishType.NativeAot,          Distroless: false,  Globalization: false, Description: "Native AOT console app"),
        new("ConsoleAppDistrolessAot", PublishType.NativeAot,          Distroless: true,   Globalization: false, Description: "Distroless native AOT console app"),
    ];

    public async Task Install() => await RunAsync("dotnet new install ./src/Templates/ConsoleApp/ --force");

    public async Task StartRegistry() => await RunAsync($"docker run --rm -d -p {RegistryPort}:5000 --name {RegistryName} {RegistryImage}");

    public async Task StopRegistry() => await RunAsync($"docker stop {RegistryName}");

    public async Task BuildSamples()
    {
        var generator = new GeneratorApp();
        await generator.BuildSamples(SamplesDir, Registry);
    }

    public async Task GenerateMarkdown()
    {
        var generator = new GeneratorApp();
        await generator.GenerateReport(SamplesDir, Registry, ReportTemplateFile, ReportFile);
    }

    public async Task GenerateSamples()
    {
        foreach (var sample in _samples)
        {
            await RunAsync("dotnet", ["new", "container-console", "--force", .. sample.GetOptions(SamplesDir)],
                onStandardOutput: Console.WriteLine,
                onStandardError: Console.Error.WriteLine,
                logCommand: cmd => Console.WriteLine($"Running command: {cmd}"));

            var configPath = Path.Combine(sample.GetOutputPath(SamplesDir), "config.json");
            var configContent = JsonSerializer.Serialize(sample, typeof(SampleDefinition), context: AppJsonSerializerContext.Default);
            File.WriteAllText(configPath, configContent);
        }
    }
}

[JsonSourceGenerationOptions(WriteIndented = true, UseStringEnumConverter = true)]
[JsonSerializable(typeof(SampleDefinition))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
