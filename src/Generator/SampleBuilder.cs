// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

namespace Generator;

internal sealed class SampleBuilder
{
    public async Task<SampleImage> BuildAsync(SampleAppInfo sampleAppInfo, bool forceBuild = false)
    {
        Console.WriteLine($"""

            Building sample: {sampleAppInfo.Name}
            """);

        var buildResult = await Docker.Build(
            contextDir: sampleAppInfo.Directory,
            tags: [sampleAppInfo.ImageTag],
            push: true,
            forceBuild: forceBuild);

        return new SampleImage(
            AppInfo: sampleAppInfo,
            Digest: buildResult.Digest,
            CompressedSize: buildResult.CompressedSize);
    }
}
