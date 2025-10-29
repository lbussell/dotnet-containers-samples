// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

#if (aot)
using System.Text.Json.Serialization;

#endif
var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddHealthChecks();

#if (aot)
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

#endif
var app = builder.Build();
app.MapHealthChecks("/healthz");
app.MapGet("/releases", async () => await ReleaseReport.Generator.MakeReportAsync());
app.Run();
#if (aot)

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.KebabCaseLower)]
[JsonSerializable(typeof(ReportJson.Report))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
