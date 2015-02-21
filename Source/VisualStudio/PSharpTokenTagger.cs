﻿//-----------------------------------------------------------------------
// <copyright file="PSharpTokenTagger.cs">
//      Copyright (c) 2015 Pantazis Deligiannis (p.deligiannis@imperial.ac.uk)
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//      IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//      CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//      TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//      SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.PSharp.Parsing;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace Microsoft.PSharp.VisualStudio
{
    /// <summary>
    /// The P# token tagger.
    /// </summary>
    internal sealed class PSharpTokenTagger : ITagger<PSharpTokenTag>
    {
        ITextBuffer Buffer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="buffer">ITextBuffer</param>
        internal PSharpTokenTagger(ITextBuffer buffer)
        {
            this.Buffer = buffer;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }

        public IEnumerable<ITagSpan<PSharpTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
            {
                yield break;
            }

            foreach (var span in spans)
            {
                var snapshot = span.Snapshot;
                var currSpan = span;
                var containingLine = currSpan.Start.GetContainingLine();

                var tokens = new PSharpLexer().Tokenize(containingLine.GetText());
                this.DetectLineCommentIfAny(tokens);

                var currLoc = containingLine.Start.Position;
                foreach (var token in tokens)
                {
                    var tokenSpan = new SnapshotSpan(currSpan.Snapshot, new Span(currLoc, token.Text.Length));
                    yield return new TagSpan<PSharpTokenTag>(tokenSpan, new PSharpTokenTag(token.Type));
                    currLoc += token.Text.Length;
                }
            }
        }

        /// <summary>
        /// Detects a (multi-)line comment, if there are any.
        /// </summary>
        /// <param name="tokens">List of tokens</param>
        /// <returns>True if multi-line comment</returns>
        private bool DetectLineCommentIfAny(List<Token> tokens)
        {
            bool isLineCommentEnabled = false;
            bool isMultiLineCommentEnabled = false;

            for (int idx = 0; idx < tokens.Count; idx++)
            {
                if (isLineCommentEnabled && tokens[idx].Type == TokenType.NewLine)
                {
                    isLineCommentEnabled = false;
                }
                else if (isMultiLineCommentEnabled && tokens[idx].Type == TokenType.CommentEnd)
                {
                    isMultiLineCommentEnabled = false;
                }
                else if (isLineCommentEnabled /*|| isMultiLineCommentEnabled*/)
                {
                    tokens[idx] = new Token(tokens[idx].Text, tokens[idx].Line, TokenType.Comment);
                }

                if (!isMultiLineCommentEnabled && tokens[idx].Type == TokenType.Comment)
                {
                    isLineCommentEnabled = true;
                }
                else if (tokens[idx].Type == TokenType.CommentStart)
                {
                    isMultiLineCommentEnabled = true;
                }
            }

            return isMultiLineCommentEnabled;
        }
    }
}
