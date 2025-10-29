// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

using System.Text.Json.Nodes;

using Generator.Exec;

using static Generator.Exec.Exec;

namespace Generator;

internal sealed class Docker
{
    const string DockerRuntime = "docker";

    public static async Task<RemoteManifestInfo> Build(DirectoryInfo contextDir, IEnumerable<string> tags, bool push = false, bool noCache = false)
    {
        IEnumerable<string> tagArgs = tags.SelectMany<string, string>(tag => ["-t", tag]);

        List<string> arguments = ["build", "--progress=plain"];
        if (push) arguments.Add("--push");
        if (noCache) arguments.Add("--no-cache");
        arguments.AddRange(tagArgs);
        arguments.Add(contextDir.FullName);

        var result = await RunDockerAsync(arguments);
        if (result.ExitCode != 0)
        {
            throw new Exception($"Docker build failed with exit code {result.ExitCode}");
        }

        var buildResult = await GetRemoteManifestAsync(tags.First());
        return buildResult;
    }

    public static async Task<RemoteManifestInfo> GetRemoteManifestAsync(string remoteImageTag)
    {
        var manifestJson = await FetchManifestJson(remoteImageTag);
        var mediaType = manifestJson["mediaType"]?.GetValue<string>() ?? "";

        // If the manifest is an index, find the first available platform-specific manifest
        if (mediaType == OciMediaTypes.ImageIndex)
        {
            var manifests = manifestJson["manifests"]?.AsArray() ?? [];
            var platformManifest = manifests.First(manifest =>
                manifest?["platform"]?["architecture"]?.GetValue<string>() != "unknown");
            var platformDigest = platformManifest?["digest"]?.GetValue<string>() ?? "";
            manifestJson = await FetchManifestJson($"{remoteImageTag}@{platformDigest}");
        }

        var imageLayers = manifestJson["layers"]?.AsArray() ?? [];
        long totalSize = imageLayers.Sum(layer => layer?["size"]?.GetValue<long>() ?? 0);
        var digest = manifestJson["config"]?["digest"]?.GetValue<string>() ?? "";
        return new RemoteManifestInfo(digest, new ImageSize(totalSize));
    }

    private static async Task<JsonNode> FetchManifestJson(string imageTag)
    {
        var result = await RunDockerAsync(["buildx", "imagetools", "inspect", "--raw", imageTag]);
        var manifestJson = JsonNode.Parse(result.StandardOutput);
        return manifestJson
            ?? throw new InvalidOperationException($"Failed to parse manifest content for image '{imageTag}'");
    }

    private static Task<ProcessResult> RunDockerAsync(IEnumerable<string> arguments, bool suppressConsoleOutput = false)
    {
        return RunAsync(
            fileName: DockerRuntime,
            arguments: arguments,
            onStandardOutput: suppressConsoleOutput ? null : Console.WriteLine,
            onStandardError: suppressConsoleOutput ? null : Console.Error.WriteLine,
            logCommand: cmd => Console.WriteLine($"Executing: `{cmd}`"));
    }
}
