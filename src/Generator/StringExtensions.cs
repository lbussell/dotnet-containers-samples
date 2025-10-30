// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

using System.Text.RegularExpressions;

namespace Generator;

public static partial class StringExtensions
{
    public static string FromPascalCaseToKebabCase(this string value)
    {
        return string.IsNullOrEmpty(value)
            ? value
            : PascalToKebabCaseRegex.Replace(value, "-$1").Trim().ToLower();
    }

    public static string FirstCharToUpper(this string value)
    {
        return string.IsNullOrEmpty(value)
            ? value
            : char.ToUpper(value[0]) + value[1..];
    }

    [GeneratedRegex("(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z0-9])", RegexOptions.Compiled)]
    private static partial Regex PascalToKebabCaseRegex { get; }
}
