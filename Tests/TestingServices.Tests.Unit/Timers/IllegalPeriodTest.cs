// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PSharp.Timers;
using Xunit;

namespace Microsoft.PSharp.TestingServices.Tests.Unit
{
    public class IllegalPeriodTest : BaseTest
    {
        private class T4 : TimedMachine
		{
            object payload = new object();

            [Start]
			[OnEntry(nameof(Initialize))]
			class Init : MachineState { }

            async Task Initialize()
			{
				// Incorrect period, will throw assertion violation
				TimerId tid = this.StartTimer(payload, -1, true);
				await this.StopTimer(tid, flush: true);
			}
        }

        [Fact]
		public void IllegalTimerStopTest()
		{
			var config = Configuration.Create().WithNumberOfIterations(1000);
			config.MaxSchedulingSteps = 200;

			var test = new Action<PSharpRuntime>((r) => {
				r.CreateMachine(typeof(T4));
			});
			base.AssertFailed(test, 1, true);
		}
    }
}
