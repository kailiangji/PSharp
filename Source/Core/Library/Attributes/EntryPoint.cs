// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See License.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using System;

namespace Microsoft.PSharp
{
    /// <summary>
    /// Attribute for declaring the entry point to a P# program.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class EntryPoint : Attribute { }
}
