// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using Microsoft.PSharp.LanguageServices.Parsing;

namespace Microsoft.PSharp.LanguageServices.Syntax
{
    /// <summary>
    /// P# syntax node.
    /// </summary>
    internal abstract class PSharpSyntaxNode
    {
        /// <summary>
        /// The program this node belongs to.
        /// </summary>
        protected IPSharpProgram Program;

        /// <summary>
        /// The text unit.
        /// </summary>
        internal protected TextUnit TextUnit
        {
            get; protected set;
        }

        protected const int SpacesPerIndent = 4;
        protected static string OneIndent = new string(' ', SpacesPerIndent);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="program">Program</param>
        protected PSharpSyntaxNode(IPSharpProgram program)
        {
            this.Program = program;
        }

        /// <summary>
        /// Creates a string to be used for the specified level of indentation.
        /// </summary>
        protected string GetIndent(int indentLevel)
        {
            return indentLevel == 0
                ? string.Empty
                : new System.Text.StringBuilder().Insert(0, OneIndent, indentLevel).ToString();
        }

        /// <summary>
        /// Rewrites the syntax node declaration to the intermediate C#
        /// representation.
        /// </summary>
        internal abstract void Rewrite(int indentLevel);
    }
}
