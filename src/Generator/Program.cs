// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

using Generator;

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

readonly record struct ImageSize(long Bytes);
