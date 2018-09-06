// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See License.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.PSharp.TestingServices.SchedulingStrategies
{
    /// <summary>
    /// The target of an operation used during scheduling.
    /// </summary>
    public enum OperationTargetType
    {
        /// <summary>
        /// The target of the operation is an <see cref="ISchedulable"/>.
        /// For example, 'Create', 'Start' and 'Stop' are operations that
        /// act upon an <see cref="ISchedulable"/>.
        /// </summary>
        Schedulable = 0,
        /// <summary>
        /// The target of the operation is the inbox of an <see cref="ISchedulable"/>.
        /// For example, 'Send' and 'Receive' are operations that act upon the
        /// inbox of an <see cref="ISchedulable"/>.
        /// </summary>
        Inbox
    }
}