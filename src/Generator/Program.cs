// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

using Generator;
using Generator.Markdown;
using Generator.Report;

const string ReportFile = "README.Report.md";
const string ReportTemplateFilePath = "src/Templates/Report.md";
const bool ForceBuild = false;

if (args.Length == 0)
{
    const string usage = """

        Usage: Generator <registry>

        <registry> - the container registry to which images will be pushed.
            It's needed in order to calculate the compressed image size of each sample image.
            You must be authenticated to the registry before running this tool.

        """;

    Console.WriteLine(usage);
    Environment.Exit(1);
}

var registry = args[0];

// Load template from file early to fail fast if it doesn't exist
var reportTemplateContent = File.ReadAllText(ReportTemplateFilePath);
var reportTemplate = ReportTemplate.Create(reportTemplateContent);

// Find all samples in the samples/ directory
var samplesToBuild =
    Directory.GetDirectories("samples")
        .Select(directory => new DirectoryInfo(directory))
        .Select(directoryInfo => SampleAppInfo.FromDirectory(directoryInfo, registry));

Console.WriteLine();
Console.WriteLine("Samples to build:");
Console.WriteLine(string.Join(Environment.NewLine, samplesToBuild));

var sampleBuilder = new SampleBuilder();
var builtSamples = new List<SampleImage>();
foreach (var sample in samplesToBuild)
{
    var builtSample = await sampleBuilder.BuildAsync(sample, forceBuild: ForceBuild);
    builtSamples.Add(builtSample);
}

Console.WriteLine();
Console.WriteLine("Built samples:");
Console.WriteLine(string.Join(Environment.NewLine, builtSamples));

IEnumerable<TableColumn> columns = [new("Name"), new("Compressed Size", Alignment.Right)];
var rows = builtSamples.Select(sample => new[] { sample.AppInfo.Name, sample.CompressedSize.ToString() });
var tableBuilder = new MarkdownTableBuilder()
    .WithColumns(columns)
    .AddRows(rows);

var markdownTable = tableBuilder.Build();
var reportData = new ReportTemplateData(Table: markdownTable);
var reportContent = reportTemplate.Render(data: reportData);
File.WriteAllText(ReportFile, reportContent);

Console.WriteLine();
Console.WriteLine($"Wrote report to {ReportFile}");
