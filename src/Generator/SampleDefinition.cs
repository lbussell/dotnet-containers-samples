// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

namespace Generator;

public record SampleDefinition(string ParentDirectory, string Name, PublishType PublishType, bool Distroless, bool Globalization, string Description)
{
    private const string DotNetVersion = "10.0";

    public string ImageRepo => $"dotnet-containers-samples/{Name.FromPascalCaseToKebabCase()}";
    public string ImageTag => DotNetVersion;
    public string OutputPath => Path.Combine(ParentDirectory, Name);
    public string ConfigPath => Path.Combine(OutputPath, "config.json");
    public string GetFullImageName(string registry) => $"{registry}/{ImageRepo}:{ImageTag}";

    public IEnumerable<string> GetOptions(string samplesDir)
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

        options.AddRange("--description", Description);
        return options;
    }
};
