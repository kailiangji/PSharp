﻿// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------------------------------------------

using System;
using Microsoft.PSharp.Timers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.PSharp.TestingServices.Tests
{
    public class BasicTimerTest : BaseTest
    {
        public BasicTimerTest(ITestOutputHelper output)
            : base(output)
        { }

        private class T1 : Machine
        {
            int Count;

            [Start]
            [OnEntry(nameof(InitOnEntry))]
            [OnEventDoAction(typeof(TimerElapsedEvent), nameof(HandleTimeout))]
            class Init : MachineState { }

            void InitOnEntry()
            {
                this.Count = 0;

                // Start a regular timer.
                this.StartTimer(TimeSpan.FromMilliseconds(10));
            }

            void HandleTimeout()
            {
                this.Count++;
                this.Assert(this.Count == 1);
            }
        }

        [Fact]
        public void TestBasicTimerOperation()
        {
            var configuration = Configuration.Create().WithNumberOfIterations(1000);
            configuration.MaxSchedulingSteps = 200;

            var test = new Action<PSharpRuntime>((r) => {
                r.CreateMachine(typeof(T1));
            });

            base.AssertSucceeded(configuration, test);
        }

        private class T2 : Machine
        {
            TimerInfo Timer;
            int Count;

            [Start]
            [OnEntry(nameof(InitOnEntry))]
            [OnEventDoAction(typeof(TimerElapsedEvent), nameof(HandleTimeout))]
            class Init : MachineState { }
            void InitOnEntry()
            {
                this.Count = 0;

                // Start a periodic timer.
                this.Timer = this.StartPeriodicTimer(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10));
            }

            void HandleTimeout()
            {
                this.Count++;
                this.Assert(this.Count <= 10);

                if (this.Count == 10)
                {
                    this.StopTimer(this.Timer);
                }
            }
        }

        [Fact]
        public void TestBasicPeriodicTimerOperation()
        {
            var configuration = Configuration.Create().WithNumberOfIterations(1000);
            var test = new Action<PSharpRuntime>((r) => {
                r.CreateMachine(typeof(T2));
            });

            base.AssertSucceeded(configuration, test);
        }

        class T3 : Machine
        {
            TimerInfo PingTimer;
            TimerInfo PongTimer;

            /// <summary>
            /// Start the PingTimer and start handling the timeout events from it.
            /// After handling 10 events, stop the timer and move to the Pong state.
            /// </summary>
            [Start]
            [OnEntry(nameof(DoPing))]
            [IgnoreEvents(typeof(TimerElapsedEvent))]
            class Ping : MachineState { }

            /// <summary>
            /// Start the PongTimer and start handling the timeout events from it.
            /// After handling 10 events, stop the timer and move to the Ping state.
            /// </summary>
            [OnEntry(nameof(DoPong))]
            [OnEventDoAction(typeof(TimerElapsedEvent), nameof(HandleTimeout))]
            class Pong : MachineState { }

            private void DoPing()
            {
                this.PingTimer = this.StartPeriodicTimer(TimeSpan.FromMilliseconds(5), TimeSpan.FromMilliseconds(5));
                this.StopTimer(this.PingTimer);

                this.Goto<Pong>();
            }

            private void DoPong()
            {
                this.PongTimer = this.StartPeriodicTimer(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(50));
            }

            private void HandleTimeout()
            {
                var timeout = (this.ReceivedEvent as TimerElapsedEvent);
                this.Assert(timeout.Info == this.PongTimer);
            }
        }

        [Fact]
        public void TestDropTimeoutsAfterTimerDisposal()
        {
            var configuration = Configuration.Create().WithNumberOfIterations(100);
            configuration.MaxSchedulingSteps = 200;

            var test = new Action<PSharpRuntime>((r) => {
                r.CreateMachine(typeof(T3));
            });

            base.AssertSucceeded(configuration, test);
        }

        class T4 : Machine
        {
            [Start]
            [OnEntry(nameof(Initialize))]
            class Init : MachineState { }

            void Initialize()
            {
                this.StartTimer(TimeSpan.FromSeconds(-1));
            }
        }

        [Fact]
        public void TestIllegalDueTimeSpecification()
        {
            var configuration = Configuration.Create().WithNumberOfIterations(1000);
            configuration.MaxSchedulingSteps = 200;

            var test = new Action<PSharpRuntime>((r) => {
                r.CreateMachine(typeof(T4));
            });

            base.AssertFailed(configuration, test, 1, true);
        }

        class T5 : Machine
        {
            [Start]
            [OnEntry(nameof(Initialize))]
            class Init : MachineState { }

            void Initialize()
            {
                this.StartPeriodicTimer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(-1));
            }
        }

        [Fact]
        public void TestIllegalPeriodSpecification()
        {
            var configuration = Configuration.Create().WithNumberOfIterations(1000);
            configuration.MaxSchedulingSteps = 200;

            var test = new Action<PSharpRuntime>((r) => {
                r.CreateMachine(typeof(T5));
            });

            base.AssertFailed(configuration, test, 1, true);
        }

        private class TransferTimerEvent : Event
        {
            public TimerInfo Timer;

            public TransferTimerEvent(TimerInfo timer)
            {
                this.Timer = timer;
            }
        }

        private class T6 : Machine
        {
            [Start]
            [OnEntry(nameof(Initialize))]
            [IgnoreEvents(typeof(TimerElapsedEvent))]
            class Init : MachineState { }

            void Initialize()
            {
                var timer = this.StartPeriodicTimer(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10));
                this.CreateMachine(typeof(T7), new TransferTimerEvent(timer));
            }
        }

        private class T7 : Machine
        {
            [Start]
            [OnEntry(nameof(Initialize))]
            class Init : MachineState { }

            void Initialize()
            {
                var timer = (this.ReceivedEvent as TransferTimerEvent).Timer;
                this.StopTimer(timer);
            }
        }

        [Fact]
        public void TestTimerDisposedByNonOwner()
        {
            var configuration = Configuration.Create().WithNumberOfIterations(1000);
            configuration.MaxSchedulingSteps = 200;

            var test = new Action<PSharpRuntime>((r) => {
                r.CreateMachine(typeof(T6));
            });

            string bugReport = "Machine 'T7()' is not allowed to dispose timer '', which is owned by machine 'T6()'.";
            base.AssertFailed(configuration, test, bugReport, true);
        }
    }
}
