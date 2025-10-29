// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

namespace Generator;

public record SampleDefinition(string Name, PublishType PublishType, bool Distroless, bool Globalization, string Description)
{
    public IEnumerable<string> GetOptions(string samplesDir)
    {
        List<string> options = [];
        options.AddRange("-o", $"{samplesDir}/{Name}");

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
