// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

using Generator;
using Generator.Markdown;
using Generator.Report;

public class GeneratorApp
{
    public async Task BuildSamples(IEnumerable<SampleDefinition> samples, string registry, bool noCache = false)
    {
        Console.WriteLine("\nSamples to build:");
        Console.WriteLine(string.Join(Environment.NewLine, samples));

        var sampleBuilder = new SampleBuilder();
        var builtSamples = new List<SampleImage>();
        foreach (var sample in samples)
        {
            var builtSample = await sampleBuilder.BuildAsync(sample, registry, noCache);
            builtSamples.Add(builtSample);
        }

        Console.WriteLine("\nBuilt samples:");
        Console.WriteLine(string.Join(Environment.NewLine, builtSamples));
    }

    public async Task GenerateReport(IEnumerable<SampleDefinition> samples, string registry, string templateFile, string output)
    {
        var reportTemplateContent = File.ReadAllText(templateFile);
        var reportTemplate = ReportTemplate.Create(reportTemplateContent);

        Console.WriteLine("\nFound samples:");
        Console.WriteLine(string.Join(Environment.NewLine, samples));

        var sampleManifests = new List<RemoteManifestInfo>();
        foreach (var sample in samples)
        {
            var imageName = sample.GetFullImageName(registry);
            RemoteManifestInfo sampleManifest = await Docker.GetRemoteManifestAsync(imageName);
            sampleManifests.Add(sampleManifest);
        }

        IEnumerable<(SampleDefinition AppInfo, RemoteManifestInfo Manifest)> sampleImages = samples.Zip(sampleManifests);

        IEnumerable<TableColumn> columns =
        [
            new("Name"),
            new("Publish Type"),
            new("Distroless"),
            new("Globalization"),
            new("Compressed Size", Alignment.Right)
        ];

        const string yes = "✅ Yes";
        const string no = "✖️ No";

        var rows = sampleImages.Select(sample => new[]
        {
            sample.AppInfo.Name,
            sample.AppInfo.PublishTypeLink,
            sample.AppInfo.Distroless ? yes : no,
            sample.AppInfo.Globalization ? yes : no,
            sample.Manifest.CompressedSize.ToString()
        });

        var tableBuilder = new MarkdownTableBuilder()
            .WithColumns(columns)
            .AddRows(rows);

        var markdownTable = tableBuilder.Build();
        var reportData = new ReportTemplateData(Table: markdownTable);
        var reportContent = reportTemplate.Render(data: reportData);
        File.WriteAllText(output, reportContent);

        Console.WriteLine($"\nWrote report to {output}");
    }
}
