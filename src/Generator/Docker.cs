// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

using System.Text.Json.Nodes;

using static Generator.Exec.Exec;

namespace Generator;

internal sealed class Docker
{
    public static async Task<DockerBuildResult> Build(DirectoryInfo contextDir, IEnumerable<string> tags, bool push = false)
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

        var buildResult = await GetBuildResultAsync(tags.First());
        return buildResult;
    }

    public static async Task<DockerBuildResult> GetBuildResultAsync(string remoteImageTag)
    {
        var manifest = await GetManifestAsync(remoteImageTag);
        var imageLayers = manifest["layers"]?.AsArray() ?? [];
        long totalSize = imageLayers.Sum(layer => layer?["size"]?.GetValue<long>() ?? 0);
        var digest = manifest["config"]?["digest"]?.GetValue<string>() ?? "";
        return new DockerBuildResult(digest, new ImageSize(totalSize));
    }

    private static async Task<JsonNode> GetManifestAsync(string imageTag)
    {
        var result = await RunAsync("docker", ["buildx", "imagetools", "inspect", "--raw", imageTag]);
        var output = JsonNode.Parse(result.StandardOutput)
            ?? throw new Exception("Failed to parse image manifest as JSON.");
        return output;
    }
}
