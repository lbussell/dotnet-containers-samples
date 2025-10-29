// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

#:package ConsoleAppFramework@5.6.1
#:project src/Generator/Generator.csproj

using ConsoleAppFramework;
using static Generator.Exec.Exec;

const string RegistryName = "dotnet-samples-registry";
const string RegistryPort = "5001";
const string Registry = $"localhost:{RegistryPort}";
const string RegistryImage = "ghcr.io/project-zot/zot";
const string SamplesDir = "samples";
const string ReportTemplateFile = "src/Templates/Report.md";
const string ReportFile = "README.Report.md";

var app = ConsoleApp.Create();

// Install the sample Dockerfile app templates
app.Add("install", async () =>
    await RunAsync("dotnet new install ./src/Templates/ConsoleApp/ --force"));

// Remove all generated sample templates
app.Add("clean-samples", async () =>
    await RunAsync($"get-childitem {SamplesDir} | foreach-object {{ remove-item -r -fo $_.fullname }}"));

// Generate samples from templates
app.Add("generate-samples", async () =>
{
    await RunAsync($"dotnet new container-console --force                    -o {SamplesDir}/ConsoleApp");
    await RunAsync($"dotnet new container-console --force --distroless       -o {SamplesDir}/ConsoleAppDistroless");
    await RunAsync($"dotnet new container-console --force --self-contained   -o {SamplesDir}/ConsoleAppSelfContained");
    await RunAsync($"dotnet new container-console --force --aot              -o {SamplesDir}/ConsoleAppNativeAot");
    await RunAsync($"dotnet new container-console --force --distroless --aot -o {SamplesDir}/ConsoleAppDistrolessAot");
});

// Build all samples
app.Add("build-samples", async () =>
    await RunAsync($"dotnet run --project src/Generator -- build-samples --samples-dir {SamplesDir} --registry {Registry}"));

// Generate markdown report of samples that were already built
app.Add("generate-markdown", async () =>
    await RunAsync($"dotnet run --project src/Generator -- generate-report --samples-dir {SamplesDir} --registry {Registry} --template-file {ReportTemplateFile} --output {ReportFile}"));

// Create a local container registry that images will be pushed to
app.Add("start-registry", async () =>
    await RunAsync($"docker run --rm -d -p {RegistryPort}:5000 --name {RegistryName} {RegistryImage}"));

// Stop the local registry container
app.Add("stop-registry", async () =>
    await RunAsync($"docker stop {RegistryName}"));

app.Run(args);
