// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddHealthChecks();

var app = builder.Build();
app.MapHealthChecks("/healthz");
app.MapGet("/releases", async () => await ReleaseReport.Generator.MakeReportAsync());
app.Run();
