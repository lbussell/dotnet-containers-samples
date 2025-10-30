// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

namespace Generator;

public record SampleDockerfileInfo(string DockerfilePath, string BaseOS, string FullImageName);

public record SampleDefinition(string ParentDirectory, string TemplateName, string Name, PublishType PublishType, bool Distroless, bool Globalization, string Description)
{
    private const string DotNetVersion = "10.0";

    public string ImageRepo => $"dotnet-containers-samples/{Name.FromPascalCaseToKebabCase()}";
    public string ImageTag => DotNetVersion;
    public string OutputPath => Path.Combine(ParentDirectory, Name);

    public string PublishTypeLink => PublishType switch
    {
        PublishType.FrameworkDependent => "[Framework-dependent]",
        PublishType.SelfContained => "[Self-contained]",
        PublishType.NativeAot => "[Native AOT]",
        _ => throw new ArgumentOutOfRangeException()
    };

    public IEnumerable<SampleDockerfileInfo> GetDockerfileInfos(string registry)
    {
        List<SampleDockerfileInfo> buildInfos = new(2)
        {
            new SampleDockerfileInfo(
                DockerfilePath: Path.Combine(OutputPath, "Dockerfile"),
                BaseOS: "Ubuntu",
                FullImageName: GetFullImageName(registry)),
        };

        if (!Distroless)
        {
            buildInfos.Add(
                new SampleDockerfileInfo(
                    DockerfilePath: Path.Combine(OutputPath, "Dockerfile.alpine"),
                    BaseOS: "Alpine",
                    FullImageName: GetFullImageName(registry) + "-alpine"));
        }

        return buildInfos;
    }

    private string GetFullImageName(string registry) => $"{registry}/{ImageRepo}:{ImageTag}";

    public IEnumerable<string> GetOptions()
    {
        List<string> options = [];
        options.AddRange("-o", OutputPath);

        if (PublishType == PublishType.NativeAot)
        {
            options.Add("--aot");
        }
        else if (PublishType == PublishType.SelfContained)
        {
            options.Add("--self-contained");
        }

        if (Distroless)
        {
            options.Add("--distroless");
        }

        if (Globalization)
        {
            options.Add("--globalization");
        }

        options.AddRange("--description", Description);
        return options;
    }
};
