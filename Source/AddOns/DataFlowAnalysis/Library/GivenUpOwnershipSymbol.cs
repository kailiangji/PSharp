// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.CodeAnalysis.CSharp.DataFlowAnalysis
{
    /// <summary>
    /// Class implementing a symbol with given-up
    /// ownership symbol.
    /// </summary>
    public class GivenUpOwnershipSymbol
    {
        #region fields

        /// <summary>
        /// Containing symbol.
        /// </summary>
        public ISymbol ContainingSymbol { get; }

        /// <summary>
        /// Statement where the ownership is given up.
        /// </summary>
        public Statement Statement { get; }

        #endregion

        #region constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">ISymbol</param>
        /// <param name="statement">Statement</param>
        internal GivenUpOwnershipSymbol(ISymbol symbol,
            Statement statement)
        {
            this.ContainingSymbol = symbol;
            this.Statement = statement;
        }

        #endregion
    }
}
