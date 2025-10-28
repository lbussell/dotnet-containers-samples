// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

using System.Text.Json;
using System.Text.Json.Nodes;

using static Generator.Exec.Exec;

namespace Generator;

internal sealed class Docker
{
    public static async Task<DockerBuildResult> Build(DirectoryInfo contextDir, IEnumerable<string> tags, bool push = false, bool forceBuild = false)
    {
        var existingManifestResult = await GetManifestAsync(tags.First());
        if (existingManifestResult is { Status: ManifestStatus.Found } && !forceBuild)
        {
            Console.WriteLine($"Image '{tags.First()}' already exists in the registry. Skipping build.");
            var existingBuildResult = await GetBuildResultAsync(tags.First());
            return existingBuildResult;
        }

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
        var manifestResult = await GetManifestAsync(remoteImageTag);
        if (manifestResult.Status != ManifestStatus.Found)
        {
            throw new InvalidOperationException($"Failed to retrieve manifest for image '{remoteImageTag}': {manifestResult.Status}");
        }

        var manifest = manifestResult.Manifest;
        var imageLayers = manifest["layers"]?.AsArray() ?? [];
        long totalSize = imageLayers.Sum(layer => layer?["size"]?.GetValue<long>() ?? 0);
        var digest = manifest["config"]?["digest"]?.GetValue<string>() ?? "";
        return new DockerBuildResult(digest, new ImageSize(totalSize));
    }

    private static async Task<ManifestResponse> GetManifestAsync(string imageTag)
    {
        var result = await RunAsync("docker", ["buildx", "imagetools", "inspect", "--raw", imageTag]);

        if (result.StandardOutput.Contains("not found"))
        {
            return new ManifestResponse(new JsonObject(), ManifestStatus.NotFound);
        }

        var manifestJson = JsonNode.Parse(result.StandardOutput);
        return manifestJson is null
            ? new ManifestResponse(new JsonObject(), ManifestStatus.Error)
            : new ManifestResponse(manifestJson, ManifestStatus.Found);
    }

    private sealed record ManifestResponse(JsonNode Manifest, ManifestStatus Status);

    private enum ManifestStatus
    {
        Found,
        NotFound,
        Error,
    }
}
