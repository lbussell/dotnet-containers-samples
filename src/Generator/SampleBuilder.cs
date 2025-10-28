// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

namespace Generator;

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
