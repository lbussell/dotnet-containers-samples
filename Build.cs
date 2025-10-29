// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

#:package ConsoleAppFramework@5.6.1
#:project src/Generator/Generator.csproj

using ConsoleAppFramework;
using static Generator.Exec.Exec;

const string RegistryName = "dotnet-samples-registry";
const string RegistryPort = "5001";
const string Registry = $"localhost:{RegistryPort}";
const string RegistryImage = "ghcr.io/project-zot/zot";
const string SamplesDir = "samples";
const string ReportTemplateFile = "src/Templates/Report.md";
const string ReportFile = "README.Report.md";

var app = ConsoleApp.Create();

// Install the sample Dockerfile app templates
app.Add("install", async () =>
    await RunAsync("dotnet new install ./src/Templates/ConsoleApp/ --force"));

// Remove all generated sample templates
app.Add("clean-samples", async () =>
    await RunAsync($"pwsh -c 'get-childitem {SamplesDir} | foreach-object {{ remove-item -r -fo $_.fullname }}'"));

// Generate samples from templates
app.Add("generate-samples", async () =>
{
    IEnumerable<Sample> samples = [
        new("ConsoleApp",              PublishType.FrameworkDependent, Distroless: false,  Globalization: false, Description: "Framework-dependent console app"),
        new("ConsoleAppDistroless",    PublishType.FrameworkDependent, Distroless: true,   Globalization: false, Description: "Framework-dependent console app with distroless base image"),
        new("ConsoleAppSelfContained", PublishType.SelfContained,      Distroless: false,  Globalization: false, Description: "Self-contained console app with trimming and ReadyToRun"),
        new("ConsoleAppNativeAot",     PublishType.NativeAot,          Distroless: false,  Globalization: false, Description: "Native AOT console app"),
        new("ConsoleAppDistrolessAot", PublishType.NativeAot,          Distroless: true,   Globalization: false, Description: "Distroless native AOT console app"),
    ];

    var command = "dotnet";
    IEnumerable<string> args = ["new", "container-console", "--force"];
    foreach (var sample in samples)
    {
        await RunAsync(command, [.. args, .. sample.GetOptions(SamplesDir)],
            onStandardOutput: Console.WriteLine,
            onStandardError: Console.Error.WriteLine,
            logCommand: cmd => Console.WriteLine($"Running command: {cmd}"));
    }
});

// Build all samples
app.Add("build-samples", async () =>
    await RunAsync($"dotnet run --project src/Generator -- build-samples --samples-dir {SamplesDir} --registry {Registry}"));

// Generate markdown report of samples that were already built
app.Add("generate-markdown", async () =>
    await RunAsync($"dotnet run --project src/Generator -- generate-report --samples-dir {SamplesDir} --registry {Registry} --template-file {ReportTemplateFile} --output {ReportFile}"));

// Create a local container registry that images will be pushed to
app.Add("start-registry", async () =>
    await RunAsync($"docker run --rm -d -p {RegistryPort}:5000 --name {RegistryName} {RegistryImage}"));

// Stop the local registry container
app.Add("stop-registry", async () =>
    await RunAsync($"docker stop {RegistryName}"));

app.Run(args);

record Sample(string Name, PublishType PublishType, bool Distroless, bool Globalization, string Description)
{
    public IEnumerable<string> GetOptions(string samplesDir)
    {
        List<string> options = [];
        options.AddRange("-o", $"{samplesDir}/{Name}");

        if (PublishType == PublishType.NativeAot)
        {
            options.Add("--aot");
        }
        else if (PublishType == PublishType.SelfContained)
        {
            options.Add("--self-contained");
        }

        if (Distroless)
        {
            options.Add("--distroless");
        }

        options.AddRange("--description", Description);
        return options;
    }
};

enum PublishType
{
    FrameworkDependent,
    SelfContained,
    NativeAot,
}
