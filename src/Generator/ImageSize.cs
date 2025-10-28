// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

namespace Generator;

internal readonly record struct ImageSize(long Bytes)
{
    public override string ToString()
    {
        const long KB = 1024;
        const long MB = KB * 1024;
        const long GB = MB * 1024;

        return Bytes switch
        {
            >= GB => $"{Bytes / (double)GB:F2} GB",
            >= MB => $"{Bytes / (double)MB:F2} MB",
            >= KB => $"{Bytes / (double)KB:F2} KB",
            _ => $"{Bytes} B"
        };
    }
}
