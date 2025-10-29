// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

namespace Generator;

internal sealed record SampleImage(SampleDefinition AppInfo, string Digest, ImageSize CompressedSize);
