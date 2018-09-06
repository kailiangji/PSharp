// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using System;

namespace Microsoft.PSharp
{
    /// <summary>
    /// Attribute for declaring that a state of a machine
    /// is the start one.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class Start : Attribute { }
}
