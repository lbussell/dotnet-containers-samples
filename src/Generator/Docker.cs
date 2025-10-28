// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

namespace Generator;

using static Generator.Exec.Exec;

class Docker
{
    public static async Task Build(DirectoryInfo contextDir, IEnumerable<string> tags, bool push = false)
    {
        IEnumerable<string> tagArgs = tags.SelectMany<string, string>(tag => ["-t", tag]);

        var result = await RunAsync(
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
