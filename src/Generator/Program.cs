// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

using Generator;
using Generator.Exec;

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
    var builtSample = await sampleBuilder.Build(sample);
    builtSamples.Add(builtSample);
}

Console.WriteLine();
Console.WriteLine("Built samples:");
Console.WriteLine(string.Join(Environment.NewLine, builtSamples));


class SampleBuilder
{
    public async Task<SampleImage> Build(SampleAppInfo sampleAppInfo)
    {
        Console.WriteLine($"Building sample: {sampleAppInfo.Name}");

        await Docker.Build(
            contextDir: sampleAppInfo.Directory,
            tags: [sampleAppInfo.ImageTag],
            push: true);

        return new SampleImage(
            AppInfo: sampleAppInfo,
            Digest: "unknown",
            UncompressedSize: new ImageSize(0));
    }
}

class Docker
{
    public static async Task Build(DirectoryInfo contextDir, IEnumerable<string> tags, bool push = false)
    {
        IEnumerable<string> tagArgs = tags.SelectMany<string, string>(tag => ["-t", tag]);

        var result = await Exec.RunAsync(
            fileName: "docker",
            arguments: ["build", "--push", "--progress=plain", .. tagArgs, contextDir.FullName],
            onStandardOutput: line => Console.WriteLine($"[{contextDir.Name}] {line}"),
            onStandardError: line => Console.Error.WriteLine($"[{contextDir.Name}] {line}"));

        if (result.ExitCode != 0)
        {
            throw new Exception($"Docker build failed with exit code {result.ExitCode}");
        }
    }

    public record DockerBuildResult(string Digest, ImageSize UncompressedSize);
}

record SampleAppInfo(string Name, DirectoryInfo Directory, string ImageTag)
{
    private const string DotNetVersion = "10.0";

    public static SampleAppInfo FromDirectory(DirectoryInfo directoryInfo, string registry = "")
    {
        var sampleName = directoryInfo.Name;
        var imageTag = $"{registry}/dotnet-containers-samples/{sampleName.FromPascalCaseToKebabCase()}:{DotNetVersion}";
        return new SampleAppInfo(sampleName, directoryInfo, imageTag);
    }
}

record SampleImage(SampleAppInfo AppInfo, string Digest, ImageSize UncompressedSize);

readonly record struct ImageSize(long Bytes);
