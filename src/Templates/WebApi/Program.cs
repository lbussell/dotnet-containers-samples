// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

#if (stjSourceGen)
using System.Text.Json.Serialization;

#endif
var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddHealthChecks();

#if (stjSourceGen)
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

#endif
var app = builder.Build();
app.MapHealthChecks("/healthz");
app.MapGet("/releases", async () => await ReleaseReport.Generator.MakeReportAsync());
app.Run();
#if (stjSourceGen)

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.KebabCaseLower)]
[JsonSerializable(typeof(ReportJson.Report))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
