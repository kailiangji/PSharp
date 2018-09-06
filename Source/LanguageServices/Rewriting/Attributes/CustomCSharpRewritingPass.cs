// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See License.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using System;

namespace Microsoft.PSharp.LanguageServices.Rewriting.CSharp
{
    /// <summary>
    /// Attribute for custom C# rewriting pass.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CustomCSharpRewritingPass : Attribute { }
}
