﻿// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.PSharp.IO;
using Microsoft.PSharp.Runtime;
using Microsoft.PSharp.TestingServices.SchedulingStrategies;

namespace Microsoft.PSharp.TestingServices.Runtime
{
    /// <summary>
    /// Runtime for executing machines in bug-finding mode.
    /// </summary>
    internal sealed class TestingRuntime : BaseTestingRuntime
    {
        /// <summary>
        /// The base machine types that can execute on this runtime.
        /// </summary>
        private readonly HashSet<Type> SupportedBaseMachineTypes;

        /// <summary>
        /// Creates a P# runtime that executes in bug-finding mode.
        /// </summary>
        /// <param name="strategy">The scheduling strategy to use during exploration.</param>
        /// <param name="reporter">Reporter to register runtime operations.</param>
        /// <param name="configuration">The configuration to use during runtime.</param>
        /// <returns>The P# testing runtime.</returns>
        [TestRuntimeCreate]
        internal static TestingRuntime Create(ISchedulingStrategy strategy, IRegisterRuntimeOperation reporter, Configuration configuration)
        {
            return new TestingRuntime(strategy, reporter, configuration);
        }

        /// <summary>
        /// Returns the type of the bug-finding runtime.
        /// </summary>
        /// <returns></returns>
        [TestRuntimeGetType]
        internal static Type GetRuntimeType() => typeof(IPSharpRuntime);

        /// <summary>
        /// Constructor.
        /// <param name="strategy">The scheduling strategy to use during exploration.</param>
        /// <param name="reporter">Reporter to register runtime operations.</param>
        /// <param name="configuration">The configuration to use during runtime.</param>
        /// </summary>
        private TestingRuntime(ISchedulingStrategy strategy, IRegisterRuntimeOperation reporter, Configuration configuration)
            : base(strategy, reporter, new ConsoleLogger(), configuration)
        {
            this.SupportedBaseMachineTypes = new HashSet<Type> { typeof(Machine), typeof(TestHarnessMachine) };
        }

        #region machine creation and execution

        /// <summary>
        /// Checks if the specified type is a machine that can execute on this runtime.
        /// </summary>
        /// <returns>True if the type is supported, else false.</returns>
        protected override bool IsSupportedMachineType(Type type) =>
            this.SupportedBaseMachineTypes.Any(machineType => type.IsSubclassOf(machineType));

        /// <summary>
        /// Checks if the constructor of the machine constructor for the
        /// specified machine type exists in the cache.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Boolean</returns>
        protected override bool IsMachineConstructorCached(Type type) => MachineFactory.IsCached(type);

        #endregion

        #region timers

        /// <summary>
        /// Return the timer machine type
        /// </summary>
        /// <returns></returns>
        public override Type GetTimerMachineType()
        {
            var timerType = base.GetTimerMachineType();
            if (timerType == null)
            {
                return typeof(Timers.ModelTimerMachine);
            }

            return timerType;
        }

        #endregion
    }
}