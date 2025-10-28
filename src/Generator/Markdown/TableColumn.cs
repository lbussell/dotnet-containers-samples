// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

namespace Generator.Markdown;

internal sealed record TableColumn(string Header, Alignment Alignment = Alignment.None);
