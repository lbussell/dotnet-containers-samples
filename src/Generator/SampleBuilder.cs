// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

namespace Generator;

internal sealed class SampleBuilder
{
    public async Task<SampleImage> BuildAsync(SampleDefinition sample, string registry, bool noCache = false)
    {
        Console.WriteLine();
        Console.WriteLine($"Building sample: {sample.Name}");

        var buildResult = await Docker.Build(
            contextDir: new DirectoryInfo(sample.OutputPath),
            tags: [sample.GetFullImageName(registry)],
            push: true,
            noCache: noCache);

        return new SampleImage(
            AppInfo: sample,
            Digest: buildResult.Digest,
            CompressedSize: buildResult.CompressedSize);
    }
}
