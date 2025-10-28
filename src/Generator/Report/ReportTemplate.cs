// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

using Fluid;

namespace Generator.Report;

internal sealed class ReportTemplate
{
    private static readonly FluidParser Parser = new();
    private readonly IFluidTemplate _template;

    private ReportTemplate(IFluidTemplate template)
    {
        _template = template;
    }

    public static ReportTemplate Create(string templateContent)
    {
        return Parser.TryParse(templateContent, out var template, out var error)
            ? new ReportTemplate(template)
            : throw new InvalidOperationException($"Failed to parse template: {error}");
    }

    public string Render(ReportTemplateData data)
    {
        var context = new TemplateContext(data);
        return _template.Render(context);
    }
}
