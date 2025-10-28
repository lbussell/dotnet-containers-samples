// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

namespace Generator;

internal sealed record SampleAppInfo(string Name, DirectoryInfo Directory, string ImageTag)
{
    private const string DotNetVersion = "10.0";

    public static SampleAppInfo FromDirectory(DirectoryInfo directoryInfo, string registry = "")
    {
        var sampleName = directoryInfo.Name;
        var imageTag = $"{registry}/dotnet-containers-samples/{sampleName.FromPascalCaseToKebabCase()}:{DotNetVersion}";
        return new SampleAppInfo(sampleName, directoryInfo, imageTag);
    }
}
