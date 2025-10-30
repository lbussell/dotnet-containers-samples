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

    private IEnumerable<SampleDefinition> AllSamples => CreateSampleDefinitions();

    private IEnumerable<SampleDefinition> CreateSampleDefinitions()
    {
        string[] templates = [ConsoleTemplate, WebApiTemplate];
        PublishType[] publishTypes = [PublishType.FrameworkDependent, PublishType.SelfContained, PublishType.NativeAot];
        bool[] distroless = [false, true];
        bool[] globalization = [false, true];

        return from template in templates
               from publishType in publishTypes
               from isDistroless in distroless
               from hasIcu in globalization
               let name = $"{(template == WebApiTemplate ? "WebApi" : "ConsoleApp")}{(publishType == PublishType.FrameworkDependent ? "" : publishType.ToString())}{(isDistroless ? "Distroless" : "")}{(hasIcu ? "Icu" : "")}"
               select new SampleDefinition(SamplesDir, template, name, publishType, isDistroless, hasIcu, "");
    }

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
