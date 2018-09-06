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
    public class BasicPeriodicTimeoutTest : BaseTest
    {
        private class T1 : TimedMachine
		{
            TimerId tid;
			object payload = new object();
			int count;

            [Start]
			[OnEntry(nameof(InitOnEntry))]
			[OnEventDoAction(typeof(TimerElapsedEvent), nameof(HandleTimeout))]
			class Init : MachineState { }

            void InitOnEntry()
			{
				count = 0;

				// Start a periodic timer
				tid = StartTimer(payload, 10, true);

			}

			async Task HandleTimeout()
			{
				count++;
				this.Assert(count <= 10);

				if(count == 10)
				{
					await StopTimer(tid, flush: true);
				}
			}
        }

        [Fact]
		public void PeriodicTimeoutTest()
		{
			var config = Configuration.Create().WithNumberOfIterations(1000);
            ModelTimerMachine.NumStepsToSkip = 1;

			var test = new Action<PSharpRuntime>((r) => {
				r.CreateMachine(typeof(T1));
			});
			base.AssertSucceeded(test);
		}
    }
}
