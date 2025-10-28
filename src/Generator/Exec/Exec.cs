// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

using System.Diagnostics;
using System.Text;

namespace Generator.Exec;

public static class Exec
{
    public static async Task<ProcessResult> RunAsync(
        string fileName,
        IEnumerable<string>? arguments = null,
        Action<string?>? onStandardOutput = null,
        Action<string?>? onStandardError = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        arguments ??= [];

        var processStartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var argument in arguments ?? [])
        {
            processStartInfo.ArgumentList.Add(argument);
        }

        var standardOutputBuilder = new StringBuilder();
        var standardErrorBuilder = new StringBuilder();

        using var process = new Process
        {
            StartInfo = processStartInfo,
        };

        process.OutputDataReceived += (_, dataReceivedEvent) =>
        {
            var data = dataReceivedEvent.Data;
            standardOutputBuilder.AppendLine(data);
            onStandardOutput?.Invoke(data);
        };

        process.ErrorDataReceived += (_, dataReceivedEvent) =>
        {
            var data = dataReceivedEvent.Data;
            standardErrorBuilder.AppendLine(data);
            onStandardError?.Invoke(data);
        };

        var startTimestamp = Stopwatch.GetTimestamp();
        var started = process.Start();

        if (!started)
        {
            throw new InvalidOperationException($"Failed to start process {fileName}");
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();

        var elapsed = Stopwatch.GetElapsedTime(startTimestamp);
        var standardOutput = standardOutputBuilder.ToString();
        var standardError = standardErrorBuilder.ToString();
        return new ProcessResult(process.ExitCode, elapsed, standardOutput, standardError);
    }
}
