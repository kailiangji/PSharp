// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See License.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Raft.PSharpLibrary
{
    internal class Log
    {
        public readonly int Term;
        public readonly int Command;

        public Log(int term, int command)
        {
            this.Term = term;
            this.Command = command;
        }
    }
}
