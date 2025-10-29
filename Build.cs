// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

#:package ConsoleAppFramework@5.6.1
#:project src/Generator/Generator.csproj

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
    private const string ConsoleTemplate = "container-app-console";
    private const string WebApiTemplate = "container-app-webapi";

    private readonly IEnumerable<SampleDefinition> _consoleSamples = [
        new(ParentDirectory: SamplesDir, TemplateName: ConsoleTemplate, Name: "ConsoleApp",                        PublishType.FrameworkDependent, Distroless: false,  Globalization: false, Description: "Framework-dependent console app"),
        new(ParentDirectory: SamplesDir, TemplateName: ConsoleTemplate, Name: "ConsoleAppDistroless",              PublishType.FrameworkDependent, Distroless: true,   Globalization: false, Description: "Framework-dependent console app with distroless base image"),
        new(ParentDirectory: SamplesDir, TemplateName: ConsoleTemplate, Name: "ConsoleAppSelfContained",           PublishType.SelfContained,      Distroless: false,  Globalization: false, Description: "Self-contained console app with trimming and ReadyToRun"),
        new(ParentDirectory: SamplesDir, TemplateName: ConsoleTemplate, Name: "ConsoleAppSelfContainedDistroless", PublishType.SelfContained,      Distroless: true,   Globalization: false, Description: "Self-contained distroless console app"),
        new(ParentDirectory: SamplesDir, TemplateName: ConsoleTemplate, Name: "ConsoleAppNativeAot",               PublishType.NativeAot,          Distroless: false,  Globalization: false, Description: "Native AOT console app"),
        new(ParentDirectory: SamplesDir, TemplateName: ConsoleTemplate, Name: "ConsoleAppDistrolessAot",           PublishType.NativeAot,          Distroless: true,   Globalization: false, Description: "Distroless native AOT console app"),
    ];

    private readonly IEnumerable<SampleDefinition> _webSamples = [
        new(ParentDirectory: SamplesDir, TemplateName: WebApiTemplate, Name: "WebApi",                        PublishType.FrameworkDependent, Distroless: false, Globalization: false, Description: "Framework-dependent web API"),
        new(ParentDirectory: SamplesDir, TemplateName: WebApiTemplate, Name: "WebApiDistroless",              PublishType.FrameworkDependent, Distroless: true,  Globalization: false, Description: ""),
        new(ParentDirectory: SamplesDir, TemplateName: WebApiTemplate, Name: "WebApiSelfContained",           PublishType.SelfContained,      Distroless: false, Globalization: false, Description: ""),
        new(ParentDirectory: SamplesDir, TemplateName: WebApiTemplate, Name: "WebApiSelfContainedDistroless", PublishType.SelfContained,      Distroless: true,  Globalization: false, Description: ""),
        new(ParentDirectory: SamplesDir, TemplateName: WebApiTemplate, Name: "WebApiNativeAot",               PublishType.NativeAot,          Distroless: false, Globalization: false, Description: ""),
        new(ParentDirectory: SamplesDir, TemplateName: WebApiTemplate, Name: "WebApiNativeAotDistroless",     PublishType.NativeAot,          Distroless: true,  Globalization: false, Description: ""),
    ];

    private IEnumerable<SampleDefinition> AllSamples => [.._consoleSamples, .._webSamples];

    public async Task Install()
    {
        await RunAsync("dotnet new install ./src/Templates/ConsoleApp/ --force");
        await RunAsync("dotnet new install ./src/Templates/WebApi/ --force");
    }

    public async Task StartRegistry() => await RunAsync($"docker run --rm -d -p {RegistryPort}:5000 --name {RegistryName} {RegistryImage}");

    public async Task StopRegistry() => await RunAsync($"docker stop {RegistryName}");

    public async Task BuildSamples(bool noCache = false)
    {
        var generator = new GeneratorApp();
        await generator.BuildSamples(AllSamples, Registry, noCache);
    }

    public async Task GenerateMarkdown()
    {
        var generator = new GeneratorApp();
        await generator.GenerateReport(AllSamples, Registry, ReportTemplateFile, ReportFile);
    }

    public async Task GenerateSamples()
    {
        foreach (var sample in AllSamples)
        {
            await RunAsync("dotnet", ["new", sample.TemplateName, "--force", ..sample.GetOptions()],
                onStandardOutput: Console.WriteLine,
                onStandardError: Console.Error.WriteLine,
                logCommand: cmd => Console.WriteLine($"Running command: {cmd}"));
        }
    }

    public async Task GenerateAll()
    {
        await Install();
        await GenerateSamples();
        await BuildSamples();
        await GenerateMarkdown();
    }
}
