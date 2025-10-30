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
        var allBuiltSamples = new List<SampleImage>();
        foreach (var sample in samples)
        {
            var builtSamples = await sampleBuilder.BuildAsync(sample, registry, noCache);
            allBuiltSamples.AddRange(builtSamples);
        }

        Console.WriteLine("\nBuilt samples:");
        Console.WriteLine(string.Join(Environment.NewLine, allBuiltSamples));
    }

    public async Task GenerateReport(IEnumerable<SampleDefinition> samples, string registry, string templateFile, string output)
    {
        var reportTemplateContent = File.ReadAllText(templateFile);
        var reportTemplate = ReportTemplate.Create(reportTemplateContent);

        Console.WriteLine("\nFound samples:");
        Console.WriteLine(string.Join(Environment.NewLine, samples));

        var sampleImages = new List<(SampleDefinition Definition, SampleDockerfileInfo Dockerfile, RemoteManifestInfo Manifest)>();
        foreach (var sample in samples)
        {
            var dockerfileInfos = sample.GetDockerfileInfos(registry);
            foreach (var dockerfile in dockerfileInfos)
            {
                RemoteManifestInfo manifest = await Docker.GetRemoteManifestAsync(dockerfile.FullImageName);
                sampleImages.Add((sample, dockerfile, manifest));
            }
        }

        IEnumerable<TableColumn> columns =
        [
            new("Name"),
            new("OS"),
            new("Publish Type"),
            new("Distroless"),
            new("Globalization"),
            new("Compressed Size", Alignment.Right)
        ];

        const string yes = "✅ Yes";
        const string no = "✖️ No";

        var rows = sampleImages
            .OrderByDescending(sample => sample.Dockerfile.BaseOS)
            .Select(sample => new[]
            {
                sample.Definition.Name,
                sample.Dockerfile.BaseOS,
                sample.Definition.PublishTypeLink,
                sample.Definition.Distroless ? yes : no,
                sample.Definition.Globalization ? yes : no,
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
