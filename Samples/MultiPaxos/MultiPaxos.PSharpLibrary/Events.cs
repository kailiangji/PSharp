// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See License.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using Microsoft.PSharp;

namespace MultiPaxos.PSharpLibrary
{
    #region Events
    
    class local : Event { }
    class success : Event { }
    class goPropose : Event { }
    class response : Event { }

    #endregion
}
