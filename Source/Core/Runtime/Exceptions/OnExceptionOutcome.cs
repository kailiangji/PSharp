// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.PSharp
{
    /// <summary>
    /// Outcome of Machine.OnException
    /// </summary>
    public enum OnExceptionOutcome
    {
        /// <summary>
        /// Throw the exception causing the runtime to fail
        /// </summary>
        ThrowException = 0,

        /// <summary>
        /// The exception was handled and Machine should continue execution
        /// </summary>
        HandledException = 1,

        /// <summary>
        /// Halt the machine (do not throw the exception)
        /// </summary>
        HaltMachine = 2
    }
}