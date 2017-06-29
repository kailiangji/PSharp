﻿//-----------------------------------------------------------------------
// <copyright file="DPORTests.cs">
//      Copyright (c) Microsoft Corporation. All rights reserved.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//      IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//      CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//      TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//      SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

using System;

using Microsoft.PSharp.Utilities;

using Xunit;

namespace Microsoft.PSharp.TestingServices.Tests.Integration
{
    public class DPORTests : BaseTest
    {
        class Ping : Event { }

        class SenderInitEvent : Event
        {
            public MachineId WaiterMachineId;

            public SenderInitEvent(MachineId waiter)
            {
                WaiterMachineId = waiter;
            }
        }

        class Waiter : Machine
        {
            [Start]
            [OnEventDoAction(typeof(Ping), nameof(Nothing))]
            private class Init : MachineState
            {
            }

            private void Nothing()
            {

            }

        }

        class Sender : Machine
        {
            private MachineId WaiterMachineId;
            [Start]
            [OnEntry(nameof(Initialize))]
            [OnEventDoAction(typeof(Ping), nameof(SendPing))]
            private class Init : MachineState
            {
            }

            private void Initialize()
            {
                WaiterMachineId = ((SenderInitEvent)ReceivedEvent).WaiterMachineId;

            }

            private void SendPing()
            {
//                 Send(WaiterMachineId, new Ping());
            }

        }

        [Fact]
        public void TestDPOR1Reduces()
        {
            var test = new Action<PSharpRuntime>((r) =>
            {
                MachineId waiter = r.CreateMachine(typeof(Waiter));
                MachineId sender1 = r.CreateMachine(typeof(Sender), new SenderInitEvent(waiter));
                MachineId sender2 = r.CreateMachine(typeof(Sender), new SenderInitEvent(waiter));
                MachineId sender3 = r.CreateMachine(typeof(Sender), new SenderInitEvent(waiter));
                r.SendEvent(sender1, new Ping());
                r.SendEvent(sender2, new Ping());
                r.SendEvent(sender3, new Ping());
            });


            var configuration = GetConfiguration();
            configuration.SchedulingIterations = 10;

            // DPOR: 1 schedule.
            configuration.SchedulingStrategy = SchedulingStrategy.DPOR;
            var runtime = AssertSucceeded(configuration, test);
            Assert.Equal(1, runtime.TestReport.NumOfExploredUnfairSchedules);

            // DFS: at least 6 schedules.
            configuration.SchedulingStrategy = SchedulingStrategy.DFS;
            runtime = AssertSucceeded(configuration, test);
            Assert.True(runtime.TestReport.NumOfExploredUnfairSchedules >= 6);

        }
    }
}