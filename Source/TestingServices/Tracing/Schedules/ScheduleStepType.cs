// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See License.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.PSharp.TestingServices.Tracing.Schedule
{
    /// <summary>
    /// The schedule step type.
    /// </summary>
    internal enum ScheduleStepType
    {
        SchedulingChoice = 0,
        NondeterministicChoice,
        FairNondeterministicChoice
    }
}
