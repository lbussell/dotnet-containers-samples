// SPDX-FileCopyrightText: Copyright (c) 2025 Logan Bussell
// SPDX-License-Identifier: MIT

namespace Generator.Exec;

public sealed record ProcessResult(int ExitCode, TimeSpan ElapsedTime, string StandardOutput, string StandardError);
