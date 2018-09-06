// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.CodeAnalysis.CSharp.DataFlowAnalysis
{
    /// <summary>
    /// Interface of a generic analysis pass.
    /// </summary>
    public interface IAnalysisPass
    {
        /// <summary>
        /// Runs the analysis.
        /// </summary>
        void Run();
    }
}
