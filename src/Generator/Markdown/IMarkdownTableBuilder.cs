// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

namespace Generator.Markdown;

internal interface IMarkdownTableBuilder
{
    /// <summary>
    /// Adds columns to the table. If columns were already defined, more columns are added.
    /// </summary>
    IMarkdownTableBuilder WithColumns(params IEnumerable<TableColumn> columns);

    /// <summary>
    /// Adds a row to the table. If the number of values does not match the number of columns,
    /// nothing special happens - the remaining cells will be empty.
    /// </summary>
    IMarkdownTableBuilder AddRow(params IEnumerable<string> values);

    /// <summary>
    /// Adds multiple rows to the table.
    /// </summary>
    IMarkdownTableBuilder AddRows(params IEnumerable<IEnumerable<string>> values);

    /// <summary>
    /// Renders the markdown table to a string. The table will be as wide as the longest row.
    /// If there are fewer values in a row than there are columns, the remaining cells in that row
    /// will be empty.
    /// </summary>
    string Build();
}
