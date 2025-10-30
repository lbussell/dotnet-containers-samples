// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

namespace Generator;

internal sealed class SampleBuilder
{
    public async Task<IEnumerable<SampleImage>> BuildAsync(SampleDefinition sample, string registry, bool noCache = false)
    {
        Console.WriteLine();
        Console.WriteLine($"Building sample: {sample.Name}");

        List<SampleImage> images = [];
        foreach (var buildInfo in sample.GetDockerfileInfos(registry))
        {
            Console.WriteLine($"Building image: {buildInfo.FullImageName} (Dockerfile: {buildInfo.DockerfilePath})");
            var buildResult = await Docker.Build(
                contextDir: new DirectoryInfo(sample.OutputPath),
                dockerfilePath: buildInfo.DockerfilePath,
                tags: [buildInfo.FullImageName],
                push: true,
                noCache: noCache);

            images.Add(
                new SampleImage(
                    AppInfo: sample,
                    Digest: buildResult.Digest,
                    CompressedSize: buildResult.CompressedSize));
        }

        return images;
    }
}
