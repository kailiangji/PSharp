// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.PSharp.StaticAnalysis
{
    /// <summary>
    /// Class implementing an error trace step.
    /// </summary>
    internal class ErrorTraceStep
    {
        /// <summary>
        /// The expression.
        /// </summary>
        internal readonly string Expression;

        /// <summary>
        /// The file name.
        /// </summary>
        internal readonly string File;

        /// <summary>
        /// The line number.
        /// </summary>
        internal readonly int Line;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="expr">Expression</param>
        /// <param name="file">File</param>
        /// <param name="line">Line</param>
        internal ErrorTraceStep(string expr, string file, int line)
        {
            this.Expression = expr;
            this.File = file;
            this.Line = line;
        }
    }
}
