﻿// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See License.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.PSharp
{
    /// <summary>
    /// Defines an action binding.
    /// </summary>
    internal sealed class ActionBinding : EventActionHandler
    {
        /// <summary>
        /// Name of the action.
        /// </summary>
        public string Name;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActionBinding(string ActionName)
        {
            Name = ActionName;
        }
    }
}