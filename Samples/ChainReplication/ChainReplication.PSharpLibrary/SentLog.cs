// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using Microsoft.PSharp;

namespace ChainReplication.PSharpLibrary
{
    public class SentLog
    {
        public int NextSeqId;
        public MachineId Client;
        public int Key;
        public int Value;

        public SentLog(int nextSeqId, MachineId client, int key, int val)
        {
            this.NextSeqId = nextSeqId;
            this.Client = client;
            this.Key = key;
            this.Value = val;
        }
    }
}
