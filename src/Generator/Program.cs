// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

using ConsoleAppFramework;

using Generator;
using Generator.Markdown;
using Generator.Report;

var app = ConsoleApp.Create();
app.Add<GeneratorApp>();
app.Run(args);

class GeneratorApp
{
    /// <summary>
    /// Builds all sample container images
    /// </summary>
    /// <param name="samplesDir">Directory where samples are located</param>
    /// <param name="registry">Sample images will be pushed to this registry</param>
    /// <param name="noCache">Pass --no-cache argument to Docker builds</param>
    public async Task BuildSamples(string samplesDir, string registry, bool noCache = false)
    {
        var samplesToBuild = FindLocalSamples(samplesDir, registry);

        Console.WriteLine("\nSamples to build:");
        Console.WriteLine(string.Join(Environment.NewLine, samplesToBuild));

        var sampleBuilder = new SampleBuilder();
        var builtSamples = new List<SampleImage>();
        foreach (var sample in samplesToBuild)
        {
            var builtSample = await sampleBuilder.BuildAsync(sample, noCache);
            builtSamples.Add(builtSample);
        }

        Console.WriteLine("\nBuilt samples:");
        Console.WriteLine(string.Join(Environment.NewLine, builtSamples));
    }

    /// <summary>
    /// Fetches built image manifests from the registry and generates a markdown report.
    /// </summary>
    /// <param name="samplesDir">Directory where samples are located</param>
    /// <param name="registry">Registry where sample images are located</param>
    /// <param name="templateFile">Path to the report template file</param>
    /// <param name="output">The generated report will be written to this file</param>
    public async Task GenerateReport(string samplesDir, string registry, string templateFile, string output)
    {
        var reportTemplateContent = File.ReadAllText(templateFile);
        var reportTemplate = ReportTemplate.Create(reportTemplateContent);

        var samplesToQuery = FindLocalSamples(samplesDir, registry);
        Console.WriteLine("\nFound samples:");
        Console.WriteLine(string.Join(Environment.NewLine, samplesToQuery));

        var sampleManifests = new List<RemoteManifestInfo>();
        foreach (var sample in samplesToQuery)
        {
            RemoteManifestInfo sampleManifest = await Docker.GetRemoteManifestAsync(sample.ImageTag);
            sampleManifests.Add(sampleManifest);
        }

        IEnumerable<(SampleAppInfo AppInfo, RemoteManifestInfo Manifest)> sampleImages = samplesToQuery.Zip(sampleManifests);

        IEnumerable<TableColumn> columns = [new("Name"), new("Compressed Size", Alignment.Right)];
        var rows = sampleImages.Select(sample => new[] { sample.AppInfo.Name, sample.Manifest.CompressedSize.ToString() });
        var tableBuilder = new MarkdownTableBuilder()
            .WithColumns(columns)
            .AddRows(rows);

        var markdownTable = tableBuilder.Build();
        var reportData = new ReportTemplateData(Table: markdownTable);
        var reportContent = reportTemplate.Render(data: reportData);
        File.WriteAllText(output, reportContent);

        Console.WriteLine($"\nWrote report to {output}");
    }

    private IEnumerable<SampleAppInfo> FindLocalSamples(string samplesDir, string registry)
    {
        return Directory.GetDirectories(samplesDir)
            .Select(directory => new DirectoryInfo(directory))
            .Select(directoryInfo => SampleAppInfo.FromDirectory(directoryInfo, registry));

    }
}
