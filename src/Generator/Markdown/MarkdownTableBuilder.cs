// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

using System.Linq;
using System.Text;

namespace Generator.Markdown;

/// <inheritdoc/>
internal sealed class MarkdownTableBuilder : IMarkdownTableBuilder
{
    private readonly List<TableColumn> _columns = [];
    private readonly List<IEnumerable<string>> _rows = [];

    /// <inheritdoc/>
    public IMarkdownTableBuilder WithColumns(params IEnumerable<TableColumn> columns)
    {
        if (columns is null)
        {
            return this;
        }

        foreach (var column in columns)
        {
            if (column is not null)
            {
                _columns.Add(column);
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public IMarkdownTableBuilder AddRows(params IEnumerable<IEnumerable<string>> rows)
    {
        _rows.AddRange(rows);
        return this;
    }

    /// <inheritdoc/>
    public IMarkdownTableBuilder AddRow(params IEnumerable<string> values)
    {
        _rows.Add(values);
        return this;
    }

    /// <inheritdoc/>
    public string Build()
    {
        var normalizedRows = _rows
            .Select(row => row.Select(cell => cell ?? string.Empty).ToArray() ?? Array.Empty<string>())
            .ToList();

        var headers = _columns.Select(static column => column.Header).ToList();
        var alignments = _columns.Select(static column => column.Alignment).ToList();

        var maxRowWidth = normalizedRows.Count == 0 ? 0 : normalizedRows.Max(static row => row.Length);
        var columnCount = Math.Max(headers.Count, maxRowWidth);

        if (columnCount == 0)
        {
            return string.Empty;
        }

        if (headers.Count < columnCount)
        {
            headers.AddRange(Enumerable.Repeat(string.Empty, columnCount - headers.Count));
            alignments.AddRange(Enumerable.Repeat(Alignment.None, columnCount - alignments.Count));
        }

        var columnWidths = Enumerable.Range(0, columnCount)
            .Select(index =>
            {
                var headerWidth = headers[index].Length;
                var maxCellWidth = normalizedRows
                    .Select(row => index < row.Length ? row[index] : string.Empty)
                    .DefaultIfEmpty(string.Empty)
                    .Max(static cell => cell.Length);

                return Math.Max(headerWidth, maxCellWidth);
            })
            .Select(width => Math.Max(width, 0))
            .ToArray();

        var builder = new StringBuilder();

        AppendRow(index => headers[index]);
        AppendAlignmentRow();

        foreach (var row in normalizedRows)
        {
            AppendRow(index => index < row.Length ? row[index] : string.Empty);
        }

        return builder.ToString().TrimEnd('\r', '\n');

        string FormatCell(string content, int width, Alignment alignment) => alignment switch
        {
            Alignment.Right => content.PadLeft(width),
            Alignment.Center => Center(content, width),
            _ => content.PadRight(width),
        };

        string Center(string content, int width)
        {
            var padding = width - content.Length;
            if (padding <= 0)
            {
                return content;
            }

            var padLeft = padding / 2;
            var padRight = padding - padLeft;
            return string.Create(content.Length + padding, (content, padLeft, padRight), static (span, state) =>
            {
                span[..state.padLeft].Fill(' ');
                state.content.AsSpan().CopyTo(span[state.padLeft..(state.padLeft + state.content.Length)]);
                span[(state.padLeft + state.content.Length)..].Fill(' ');
            });
        }

        string BuildAlignmentCell(int width, Alignment alignment)
        {
            var hyphenCount = Math.Max(3, width);

            return alignment switch
            {
                Alignment.Left => $":{new string('-', hyphenCount - 1)}",
                Alignment.Center => $":{new string('-', Math.Max(1, hyphenCount - 2))}:",
                Alignment.Right => $"{new string('-', hyphenCount - 1)}:",
                _ => new string('-', hyphenCount),
            };
        }

        void AppendRow(Func<int, string> valueSelector)
        {
            builder.Append('|');

            for (var index = 0; index < columnCount; index++)
            {
                var value = valueSelector(index);
                var formatted = FormatCell(value, columnWidths[index], alignments[index]);

                builder.Append(' ');
                builder.Append(formatted);
                builder.Append(' ');
                builder.Append('|');
            }

            builder.AppendLine();
        }

        void AppendAlignmentRow()
        {
            builder.Append('|');

            for (var index = 0; index < columnCount; index++)
            {
                var alignmentCell = BuildAlignmentCell(columnWidths[index], alignments[index]);

                builder.Append(' ');
                builder.Append(alignmentCell);
                builder.Append(' ');
                builder.Append('|');
            }

            builder.AppendLine();
        }
    }
}
