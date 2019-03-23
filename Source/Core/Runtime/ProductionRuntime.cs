// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.PSharp.Deprecated.Timers;
using Microsoft.PSharp.Timers;

namespace Microsoft.PSharp.Runtime
{
    /// <summary>
    /// Runtime for executing machines in production.
    /// </summary>
    internal sealed class ProductionRuntime : BaseRuntime
    {
        /// <summary>
        /// List of monitors in the program.
        /// </summary>
        private readonly List<Monitor> Monitors;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal ProductionRuntime()
            : this(Configuration.Create())
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configuration">Configuration</param>
        internal ProductionRuntime(Configuration configuration)
            : base(configuration)
        {
            this.Monitors = new List<Monitor>();
        }

        #region runtime interface

        /// <summary>
        /// Creates a machine id that is uniquely tied to the specified unique name. The
        /// returned machine id can either be a fresh id (not yet bound to any machine),
        /// or it can be bound to a previously created machine. In the second case, this
        /// machine id can be directly used to communicate with the corresponding machine.
        /// </summary>
        /// <param name="type">Type of the machine.</param>
        /// <param name="machineName">Unique name used to create or get the machine id.</param>
        /// <returns>The result is the <see cref="MachineId"/>.</returns>
        public override MachineId CreateMachineIdFromName(Type type, string machineName) => new MachineId(type, machineName, this, true);

        /// <summary>
        /// Creates a new machine of the specified <see cref="Type"/> and with
        /// the specified optional <see cref="Event"/>. This event can only be
        /// used to access its payload, and cannot be handled.
        /// </summary>
        /// <param name="type">Type of the machine.</param>
        /// <param name="e">Optional event used during initialization.</param>
        /// <param name="operationGroupId">Optional operation group id.</param>
        /// <returns>The result is the <see cref="MachineId"/>.</returns>
        public override MachineId CreateMachine(Type type, Event e = null, Guid? operationGroupId = null) =>
            this.CreateMachine(null, type, null, e, null, operationGroupId);

        /// <summary>
        /// Creates a new machine of the specified <see cref="Type"/> and name, and
        /// with the specified optional <see cref="Event"/>. This event can only be
        /// used to access its payload, and cannot be handled.
        /// </summary>
        /// <param name="type">Type of the machine.</param>
        /// <param name="machineName">Optional machine name used for logging.</param>
        /// <param name="e">Optional event used during initialization.</param>
        /// <param name="operationGroupId">Optional operation group id.</param>
        /// <returns>The result is the <see cref="MachineId"/>.</returns>
        public override MachineId CreateMachine(Type type, string machineName, Event e = null, Guid? operationGroupId = null) =>
            this.CreateMachine(null, type, machineName, e, null, operationGroupId);

        /// <summary>
        /// Creates a new machine of the specified type, using the specified <see cref="MachineId"/>.
        /// This method optionally passes an <see cref="Event"/> to the new machine, which can only
        /// be used to access its payload, and cannot be handled.
        /// </summary>
        /// <param name="mid">Unbound machine id.</param>
        /// <param name="type">Type of the machine.</param>
        /// <param name="e">Optional event used during initialization.</param>
        /// <param name="operationGroupId">Optional operation group id.</param>
        /// <returns>The result is the <see cref="MachineId"/>.</returns>
        public override MachineId CreateMachine(MachineId mid, Type type, Event e = null, Guid? operationGroupId = null) =>
            this.CreateMachine(mid, type, null, e, null, operationGroupId);

        /// <summary>
        /// Creates a new machine of the specified <see cref="Type"/> and with
        /// the specified optional <see cref="Event"/>. This event can only be
        /// used to access its payload, and cannot be handled.
        /// </summary>
        /// <param name="type">Type of the machine.</param>
        /// <param name="e">Optional event used during initialization.</param>
        /// <param name="operationGroupId">Optional operation group id.</param>
        /// <returns>Task that represents the asynchronous operation. The task result is the <see cref="MachineId"/>.</returns>
        public override Task<MachineId> CreateMachineAsync(Type type, Event e = null, Guid? operationGroupId = null) =>
            Task.FromResult(this.CreateMachine(null, type, null, e, null, operationGroupId));

        /// <summary>
        /// Creates a new machine of the specified <see cref="Type"/> and name, and
        /// with the specified optional <see cref="Event"/>. This event can only be
        /// used to access its payload, and cannot be handled.
        /// </summary>
        /// <param name="type">Type of the machine.</param>
        /// <param name="machineName">Optional machine name used for logging.</param>
        /// <param name="e">Optional event used during initialization.</param>
        /// <param name="operationGroupId">Optional operation group id.</param>
        /// <returns>Task that represents the asynchronous operation. The task result is the <see cref="MachineId"/>.</returns>
        public override Task<MachineId> CreateMachineAsync(Type type, string machineName, Event e = null, Guid? operationGroupId = null) =>
            Task.FromResult(this.CreateMachine(null, type, machineName, e, null, operationGroupId));

        /// <summary>
        /// Creates a new machine of the specified type, using the specified <see cref="MachineId"/>.
        /// This method optionally passes an <see cref="Event"/> to the new machine, which can only
        /// be used to access its payload, and cannot be handled.
        /// </summary>
        /// <param name="mid">Unbound machine id.</param>
        /// <param name="type">Type of the machine.</param>
        /// <param name="e">Optional event used during initialization.</param>
        /// <param name="operationGroupId">Optional operation group id.</param>
        /// <returns>Task that represents the asynchronous operation. The task result is the <see cref="MachineId"/>.</returns>
        public override Task<MachineId> CreateMachineAsync(MachineId mid, Type type, Event e = null, Guid? operationGroupId = null) =>
            Task.FromResult(this.CreateMachine(mid, type, null, e, null, operationGroupId));

        /// <summary>
        /// Creates a new machine of the specified <see cref="Type"/> and with the
        /// specified optional <see cref="Event"/>. This event can only be used to
        /// access its payload, and cannot be handled. The method returns only when
        /// the machine is initialized and the <see cref="Event"/> (if any) is handled.
        /// </summary>
        /// <param name="type">Type of the machine.</param>
        /// <param name="e">Optional event used during initialization.</param>
        /// <param name="operationGroupId">Optional operation group id.</param>
        /// <returns>Task that represents the asynchronous operation. The task result is the <see cref="MachineId"/>.</returns>
        public override Task<MachineId> CreateMachineAndExecuteAsync(Type type, Event e = null, Guid? operationGroupId = null) =>
            this.CreateMachineAndExecute(null, type, null, e, null, operationGroupId);

        /// <summary>
        /// Creates a new machine of the specified <see cref="Type"/> and name, and with
        /// the specified optional <see cref="Event"/>. This event can only be used to
        /// access its payload, and cannot be handled. The method returns only when the
        /// machine is initialized and the <see cref="Event"/> (if any) is handled.
        /// </summary>
        /// <param name="type">Type of the machine.</param>
        /// <param name="machineName">Optional machine name used for logging.</param>
        /// <param name="e">Optional event used during initialization.</param>
        /// <param name="operationGroupId">Optional operation group id.</param>
        /// <returns>Task that represents the asynchronous operation. The task result is the <see cref="MachineId"/>.</returns>
        public override Task<MachineId> CreateMachineAndExecuteAsync(Type type, string machineName, Event e = null, Guid? operationGroupId = null) =>
            this.CreateMachineAndExecute(null, type, machineName, e, null, operationGroupId);

        /// <summary>
        /// Creates a new machine of the specified <see cref="Type"/>, using the specified
        /// unbound machine id, and passes the specified optional <see cref="Event"/>. This
        /// event can only be used to access its payload, and cannot be handled. The method
        /// returns only when the machine is initialized and the <see cref="Event"/> (if any)
        /// is handled.
        /// </summary>
        /// <param name="mid">Unbound machine id.</param>
        /// <param name="type">Type of the machine.</param>
        /// <param name="e">Optional event used during initialization.</param>
        /// <param name="operationGroupId">Optional operation group id.</param>
        /// <returns>Task that represents the asynchronous operation. The task result is the <see cref="MachineId"/>.</returns>
        public override Task<MachineId> CreateMachineAndExecuteAsync(MachineId mid, Type type, Event e = null, Guid? operationGroupId = null) =>
            this.CreateMachineAndExecute(mid, type, null, e, null, operationGroupId);

        /// <summary>
        /// Creates a new machine of the specified <see cref="Type"/> and with the
        /// specified optional <see cref="Event"/>. This event can only be used to
        /// access its payload, and cannot be handled. The method returns only when
        /// the machine is initialized and the <see cref="Event"/> (if any) is handled.
        /// </summary>
        /// <param name="type">Type of the machine.</param>
        /// <param name="e">Optional event used during initialization.</param>
        /// <param name="operationGroupId">Optional operation group id.</param>
        /// <returns>Task that represents the asynchronous operation. The task result is the <see cref="MachineId"/>.</returns>
        public override Task<MachineId> CreateMachineAndExecute(Type type, Event e = null, Guid? operationGroupId = null) =>
            this.CreateMachineAndExecute(null, type, null, e, null, operationGroupId);

        /// <summary>
        /// Creates a new machine of the specified <see cref="Type"/> and name, and with
        /// the specified optional <see cref="Event"/>. This event can only be used to
        /// access its payload, and cannot be handled. The method returns only when the
        /// machine is initialized and the <see cref="Event"/> (if any) is handled.
        /// </summary>
        /// <param name="type">Type of the machine.</param>
        /// <param name="machineName">Optional machine name used for logging.</param>
        /// <param name="e">Optional event used during initialization.</param>
        /// <param name="operationGroupId">Optional operation group id.</param>
        /// <returns>Task that represents the asynchronous operation. The task result is the <see cref="MachineId"/>.</returns>
        public override Task<MachineId> CreateMachineAndExecute(Type type, string machineName, Event e = null, Guid? operationGroupId = null) =>
            this.CreateMachineAndExecute(null, type, machineName, e, null, operationGroupId);

        /// <summary>
        /// Creates a new machine of the specified <see cref="Type"/>, using the specified
        /// unbound machine id, and passes the specified optional <see cref="Event"/>. This
        /// event can only be used to access its payload, and cannot be handled. The method
        /// returns only when the machine is initialized and the <see cref="Event"/> (if any)
        /// is handled.
        /// </summary>
        /// <param name="mid">Unbound machine id.</param>
        /// <param name="type">Type of the machine.</param>
        /// <param name="e">Optional event used during initialization.</param>
        /// <param name="operationGroupId">Optional operation group id.</param>
        /// <returns>Task that represents the asynchronous operation. The task result is the <see cref="MachineId"/>.</returns>
        public override Task<MachineId> CreateMachineAndExecute(MachineId mid, Type type, Event e = null, Guid? operationGroupId = null) =>
            this.CreateMachineAndExecute(mid, type, null, e, null, operationGroupId);

        /// <summary>
        /// Sends an asynchronous <see cref="Event"/> to a machine.
        /// </summary>
        /// <param name="target">Target machine id</param>
        /// <param name="e">The event to send.</param>
        /// <param name="options">Optional parameters of a send operation.</param>
        public override void SendEvent(MachineId target, Event e, SendOptions options = null)
        {
            // If the target machine is null then report an error and exit.
            base.Assert(target != null, "Cannot send to a null machine.");
            // If the event is null then report an error and exit.
            base.Assert(e != null, "Cannot send a null event.");
            this.SendEvent(target, e, null, options);
        }

        /// <summary>
        /// Sends an asynchronous <see cref="Event"/> to a machine.
        /// </summary>
        /// <param name="target">Target machine id</param>
        /// <param name="e">The event to send.</param>
        /// <param name="options">Optional parameters of a send operation.</param>
        /// <returns>Task that represents the asynchronous operation.</returns>
        public override Task SendEventAsync(MachineId target, Event e, SendOptions options = null)
        {
            this.SendEvent(target, e, options);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sends an <see cref="Event"/> to a machine. Returns immediately if the target machine was already
        /// running. Otherwise blocks until the machine handles the event and reaches quiescense again.
        /// </summary>
        /// <param name="target">The id of the target machine.</param>
        /// <param name="e">The event to send.</param>
        /// <param name="options">Optional parameters of a send operation.</param>
        /// <returns>Task that represents the asynchronous operation. The task result is true if
        /// the event was handled, false if the event was only enqueued.</returns>
        public override Task<bool> SendEventAndExecuteAsync(MachineId target, Event e, SendOptions options = null)
        {
            // If the target machine is null then report an error and exit.
            base.Assert(target != null, "Cannot send to a null machine.");
            // If the event is null then report an error and exit.
            base.Assert(e != null, "Cannot send a null event.");
            return this.SendEventAndExecute(target, e, null, options);
        }

        /// <summary>
        /// Sends an <see cref="Event"/> to a machine. Returns immediately if the target machine was already
        /// running. Otherwise blocks until the machine handles the event and reaches quiescense again.
        /// </summary>
        /// <param name="target">The id of the target machine.</param>
        /// <param name="e">The event to send.</param>
        /// <param name="options">Optional parameters of a send operation.</param>
        /// <returns>Task that represents the asynchronous operation. The task result is true if
        /// the event was handled, false if the event was only enqueued.</returns>
        public override Task<bool> SendEventAndExecute(MachineId target, Event e, SendOptions options = null) =>
            this.SendEventAndExecuteAsync(target, e, options);

        /// <summary>
        /// Returns the operation group id of the specified machine. Returns <see cref="Guid.Empty"/>
        /// if the id is not set, or if the <see cref="MachineId"/> is not associated with this runtime.
        /// During testing, the runtime asserts that the specified machine is currently executing.
        /// </summary>
        /// <param name="currentMachine">MachineId of the currently executing machine.</param>
        /// <returns>The unique identifier.</returns>
        public override Guid GetCurrentOperationGroupId(MachineId currentMachine)
        {
            if (!this.MachineMap.TryGetValue(currentMachine, out Machine machine))
            {
                return Guid.Empty;
            }

            return machine.Info.OperationGroupId;
        }

        #endregion

        #region state-machine execution

        /// <summary>
        /// Creates a new <see cref="Machine"/> of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="mid">Unbound machine id.</param>
        /// <param name="type">Type of the machine.</param>
        /// <param name="machineName">Machine name used for logging.</param>
        /// <param name="e">The event used during initialization.</param>
        /// <param name="creator">The creator machine.</param>
        /// <param name="operationGroupId">The operation group id.</param>
        /// <returns>The result is the <see cref="MachineId"/>.</returns>
        internal override MachineId CreateMachine(MachineId mid, Type type, string machineName, Event e, Machine creator, Guid? operationGroupId)
        {
            Machine machine = this.CreateMachine(mid, type, machineName);
            base.Logger.OnCreateMachine(machine.Id, creator?.Id);
            this.SetOperationGroupIdForMachine(machine, creator, operationGroupId);
            this.RunMachineEventHandler(machine, e, true);
            return machine.Id;
        }

        /// <summary>
        /// Creates a new <see cref="Machine"/> of the specified <see cref="Type"/>. The
        /// method returns only when the created machine reaches quiescence
        /// </summary>
        /// <param name="mid">Unbound machine id.</param>
        /// <param name="type">Type of the machine.</param>
        /// <param name="machineName">Machine name used for logging.</param>
        /// <param name="e">The event used during initialization.</param>
        /// <param name="creator">The creator machine.</param>
        /// <param name="operationGroupId">The operation group id.</param>
        /// <returns>The result is the <see cref="MachineId"/>.</returns>
        internal override async Task<MachineId> CreateMachineAndExecute(MachineId mid, Type type, string machineName, Event e,
            Machine creator, Guid? operationGroupId)
        {
            Machine machine = this.CreateMachine(mid, type, machineName);
            base.Logger.OnCreateMachine(machine.Id, creator?.Id);
            this.SetOperationGroupIdForMachine(machine, creator, operationGroupId);
            await this.RunMachineEventHandlerAsync(machine, e, true);
            return machine.Id;
        }

        /// <summary>
        /// Creates a new <see cref="Machine"/> of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="mid">Unbound machine id.</param>
        /// <param name="type">Type of the machine.</param>
        /// <param name="machineName">Machine name used for logging.</param>
        /// <returns>Machine</returns>
        private Machine CreateMachine(MachineId mid, Type type, string machineName)
        {
            this.Assert(type.IsSubclassOf(typeof(Machine)), "Type '{0}' is not a machine.", type.Name);

            if (mid == null)
            {
                mid = new MachineId(type, machineName, this);
            }
            else
            {
                this.Assert(mid.Runtime == null || mid.Runtime == this, "Unbound machine id '{0}' was created by another runtime.", mid.Value);
                this.Assert(mid.Type == type.FullName, "Cannot bind machine id '{0}' of type '{1}' to a machine of type '{2}'.",
                    mid.Value, mid.Type, type.FullName);
                mid.Bind(this);
            }

            Machine machine = MachineFactory.Create(type);

            machine.Initialize(this, mid, new MachineInfo(mid));
            machine.InitializeStateInformation();

            bool result = this.MachineMap.TryAdd(mid, machine);
            this.Assert(result, "Machine with id '{0}' was already created in generation '{1}'. This typically occurs " +
                "either if the machine id was created by another runtime instance, or if a machine id from a previous " +
                "runtime generation was deserialized, but the current runtime has not increased its generation value.",
                mid.Value, mid.Generation);

            return machine;
        }

        /// <summary>
        /// Sends an asynchronous <see cref="Event"/> to a machine.
        /// </summary>
        /// <param name="target">The id of the target machine.</param>
        /// <param name="e">The event to send.</param>
        /// <param name="sender">The sender machine.</param>
        /// <param name="options">Optional parameters of a send operation.</param>
        internal override void SendEvent(MachineId target, Event e, BaseMachine sender, SendOptions options)
        {
            var operationGroupId = base.GetNewOperationGroupId(sender, options?.OperationGroupId);
            if (!base.GetTargetMachine(target, e, sender, operationGroupId, out Machine machine))
            {
                this.TryHandleDroppedEvent(e, target);
                return;
            }

            bool runNewHandler = false;
            this.EnqueueEvent(machine, e, sender, operationGroupId, ref runNewHandler);
            if (runNewHandler)
            {
                this.RunMachineEventHandler(machine, null, false);
            }
        }

        /// <summary>
        /// Sends an asynchronous <see cref="Event"/> to a machine. Returns immediately
        /// if the target machine was already running. Otherwise blocks until the machine handles
        /// the event and reaches quiescense again.
        /// </summary>
        /// <param name="target">The id of the target machine.</param>
        /// <param name="e">The event to send.</param>
        /// <param name="sender">The sender machine.</param>
        /// <param name="options">Optional parameters of a send operation.</param>
        /// <returns>True if event was handled, false if the event was only enqueued.</returns>
        internal override async Task<bool> SendEventAndExecute(MachineId target, Event e, BaseMachine sender, SendOptions options)
        {
            var operationGroupId = base.GetNewOperationGroupId(sender, options?.OperationGroupId);
            if (!base.GetTargetMachine(target, e, sender, operationGroupId, out Machine machine))
            {
                this.TryHandleDroppedEvent(e, target);
                return true;
            }

            bool runNewHandler = false;
            this.EnqueueEvent(machine, e, sender, operationGroupId, ref runNewHandler);
            if (runNewHandler)
            {
                await this.RunMachineEventHandlerAsync(machine, null, false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Enqueues an asynchronous <see cref="Event"/> to a machine.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="e">Event</param>
        /// <param name="sender">The sender machine.</param>
        /// <param name="operationGroupId">Operation group id</param>
        /// <param name="runNewHandler">Run a new handler</param>
        private void EnqueueEvent(Machine machine, Event e, BaseMachine sender, Guid operationGroupId, ref bool runNewHandler)
        {
            EventInfo eventInfo = new EventInfo(e, null);
            eventInfo.SetOperationGroupId(operationGroupId);

            var senderState = (sender as Machine)?.CurrentStateName ?? string.Empty;
            base.Logger.OnSend(machine.Id, sender?.Id, senderState,
                e.GetType().FullName, operationGroupId, isTargetHalted: false);

            machine.Enqueue(eventInfo, ref runNewHandler);
        }

        /// <summary>
        /// Runs a new asynchronous machine event handler.
        /// This is a fire and forget invocation.
        /// </summary>
        /// <param name="machine">Machine that executes this event handler.</param>
        /// <param name="initialEvent">Event for initializing the machine.</param>
        /// <param name="isFresh">If true, then this is a new machine.</param>
        private void RunMachineEventHandler(Machine machine, Event initialEvent, bool isFresh)
        {
            Task.Run(async () =>
            {
                try
                {
                    if (isFresh)
                    {
                        await machine.GotoStartState(initialEvent);
                    }

                    await machine.RunEventHandler();
                }
                catch (Exception ex)
                {
                    base.IsRunning = false;
                    base.RaiseOnFailureEvent(ex);
                }
            });
        }

        /// <summary>
        /// Runs a new asynchronous machine event handler.
        /// </summary>
        /// <param name="machine">Machine that executes this event handler.</param>
        /// <param name="initialEvent">Event for initializing the machine.</param>
        /// <param name="isFresh">If true, then this is a new machine.</param>
        private async Task RunMachineEventHandlerAsync(Machine machine, Event initialEvent, bool isFresh)
        {
            try
            {
                if (isFresh)
                {
                    await machine.GotoStartState(initialEvent);
                }

                await machine.RunEventHandler();
            }
            catch (Exception ex)
            {
                base.IsRunning = false;
                base.RaiseOnFailureEvent(ex);
                return;
            }
        }

        #endregion

        #region timers

        /// <summary>
        /// Creates a new timer that sends a <see cref="Timers.TimerElapsedEvent"/> to its owner machine.
        /// </summary>
        /// <param name="info">Stores information about the timer.</param>
        /// <param name="owner">The owner machine.</param>
        /// <returns>The machine timer.</returns>
        internal override IMachineTimer CreateMachineTimer(TimerInfo info, Machine owner) => new MachineTimer(info, owner);

        /// <summary>
        /// Returns the timer machine type.
        /// </summary>
        internal override Type GetTimerMachineType()
        {
            return typeof(ProductionTimerMachine);
        }

        #endregion

        #region specifications and error checking

        /// <summary>
        /// Tries to create a new <see cref="PSharp.Monitor"/> of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">Type of the monitor.</param>
        internal override void TryCreateMonitor(Type type)
        {
            // Check if monitors are enabled in production.
            if (!base.Configuration.EnableMonitorsInProduction)
            {
                return;
            }

            lock (this.Monitors)
            {
                if (this.Monitors.Any(m => m.GetType() == type))
                {
                    // Idempotence: only one monitor per type can exist.
                    return;
                }
            }

            base.Assert(type.IsSubclassOf(typeof(Monitor)), $"Type '{type.Name}' " +
                "is not a subclass of Monitor.\n");

            MachineId mid = new MachineId(type, null, this);
            Monitor monitor = (Monitor)Activator.CreateInstance(type);

            monitor.Initialize(this, mid);
            monitor.InitializeStateInformation();

            lock (this.Monitors)
            {
                this.Monitors.Add(monitor);
            }

            base.Logger.OnCreateMonitor(type.Name, monitor.Id);

            monitor.GotoStartState();
        }

        /// <summary>
        /// Invokes the specified <see cref="PSharp.Monitor"/> with the specified <see cref="Event"/>.
        /// </summary>
        /// <param name="sender">The sender machine.</param>
        /// <param name="type">Type of the monitor.</param>
        /// <param name="e">The event to monitor.</param>
        internal override void Monitor(Type type, BaseMachine sender, Event e)
        {
            // Check if monitors are enabled in production.
            if (!base.Configuration.EnableMonitorsInProduction)
            {
                return;
            }

            Monitor monitor = null;

            lock (this.Monitors)
            {
                foreach (var m in this.Monitors)
                {
                    if (m.GetType() == type)
                    {
                        monitor = m;
                        break;
                    }
                }
            }

            if (monitor != null)
            {
                lock (monitor)
                {
                    monitor.MonitorEvent(e);
                }
            }
        }

        #endregion

        #region nondeterministic choices

        /// <summary>
        /// Returns a nondeterministic boolean choice, that can be
        /// controlled during analysis or testing.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="maxValue">The max value.</param>
        /// <returns>Boolean</returns>
        internal override bool GetNondeterministicBooleanChoice(BaseMachine machine, int maxValue)
        {
            Random random = new Random(DateTime.Now.Millisecond);

            bool result = false;
            if (random.Next(maxValue) == 0)
            {
                result = true;
            }

            base.Logger.OnRandom(machine?.Id, result);

            return result;
        }

        /// <summary>
        /// Returns a fair nondeterministic boolean choice, that can be
        /// controlled during analysis or testing.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="uniqueId">Unique id</param>
        /// <returns>Boolean</returns>
        internal override bool GetFairNondeterministicBooleanChoice(BaseMachine machine, string uniqueId)
        {
            return this.GetNondeterministicBooleanChoice(machine, 2);
        }

        /// <summary>
        /// Returns a nondeterministic integer choice, that can be
        /// controlled during analysis or testing.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="maxValue">The max value.</param>
        /// <returns>Integer</returns>
        internal override int GetNondeterministicIntegerChoice(BaseMachine machine, int maxValue)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            var result = random.Next(maxValue);

            base.Logger.OnRandom(machine?.Id, result);

            return result;
        }

        #endregion

        #region notifications

        /// <summary>
        /// Notifies that a machine entered a state.
        /// </summary>
        /// <param name="machine">Machine</param>
        internal override void NotifyEnteredState(Machine machine)
        {
            if (base.Configuration.Verbose <= 1)
            {
                return;
            }

            base.Logger.OnMachineState(machine.Id, machine.CurrentStateName, isEntry:true);
        }

        /// <summary>
        /// Notifies that a monitor entered a state.
        /// </summary>
        /// <param name="monitor">Monitor</param>
        internal override void NotifyEnteredState(Monitor monitor)
        {
            if (base.Configuration.Verbose <= 1)
            {
                return;
            }

            string monitorState = monitor.CurrentStateNameWithTemperature;
            base.Logger.OnMonitorState(monitor.GetType().Name, monitor.Id, monitorState, true, monitor.GetHotState());
        }

        /// <summary>
        /// Notifies that a machine exited a state.
        /// </summary>
        /// <param name="machine">Machine</param>
        internal override void NotifyExitedState(Machine machine)
        {
            if (base.Configuration.Verbose <= 1)
            {
                return;
            }

            base.Logger.OnMachineState(machine.Id, machine.CurrentStateName, isEntry: false);
        }

        /// <summary>
        /// Notifies that a monitor exited a state.
        /// </summary>
        /// <param name="monitor">Monitor</param>
        internal override void NotifyExitedState(Monitor monitor)
        {
            if (base.Configuration.Verbose <= 1)
            {
                return;
            }

            string monitorState = monitor.CurrentStateNameWithTemperature;
            base.Logger.OnMonitorState(monitor.GetType().Name,monitor.Id, monitorState, false, monitor.GetHotState());
        }

        /// <summary>
        /// Notifies that a machine invoked an action.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="action">Action</param>
        /// <param name="receivedEvent">Event</param>
        internal override void NotifyInvokedAction(Machine machine, MethodInfo action, Event receivedEvent)
        {
            if (base.Configuration.Verbose <= 1)
            {
                return;
            }

            base.Logger.OnMachineAction(machine.Id, machine.CurrentStateName, action.Name);
        }

        /// <summary>
        /// Notifies that a monitor invoked an action.
        /// </summary>
        /// <param name="monitor">Monitor</param>
        /// <param name="action">Action</param>
        /// <param name="receivedEvent">Event</param>
        internal override void NotifyInvokedAction(Monitor monitor, MethodInfo action, Event receivedEvent)
        {
            if (base.Configuration.Verbose <= 1)
            {
                return;
            }

            base.Logger.OnMonitorAction(monitor.GetType().Name, monitor.Id, action.Name, monitor.CurrentStateName);
        }

        /// <summary>
        /// Notifies that a machine raised an <see cref="Event"/>.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="eventInfo">EventInfo</param>
        internal override void NotifyRaisedEvent(Machine machine, EventInfo eventInfo)
        {
            eventInfo.SetOperationGroupId(base.GetNewOperationGroupId(machine, null));

            if (base.Configuration.Verbose <= 1)
            {
                return;
            }

            base.Logger.OnMachineEvent(machine.Id, machine.CurrentStateName, eventInfo.EventName);
        }

        /// <summary>
        /// Notifies that a monitor raised an <see cref="Event"/>.
        /// </summary>
        /// <param name="monitor">Monitor</param>
        /// <param name="eventInfo">EventInfo</param>
        internal override void NotifyRaisedEvent(Monitor monitor, EventInfo eventInfo)
        {
            if (base.Configuration.Verbose <= 1)
            {
                return;
            }

            base.Logger.OnMonitorEvent(monitor.GetType().Name, monitor.Id, monitor.CurrentStateName,
                eventInfo.EventName, isProcessing:false);
        }

        /// <summary>
        /// Notifies that a machine dequeued an <see cref="Event"/>.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="eventInfo">EventInfo</param>
        internal override void NotifyDequeuedEvent(Machine machine, EventInfo eventInfo)
        {
            // The machine inherits the operation group id of the dequeued event.
            machine.Info.OperationGroupId = eventInfo.OperationGroupId;

            base.Logger.OnDequeue(machine.Id, machine.CurrentStateName, eventInfo.EventName);
        }

        /// <summary>
        /// Notifies that a machine is waiting to receive one or more events.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="eventInfoInInbox">The event info if it is in the inbox, else null</param>
        internal override void NotifyWaitEvents(Machine machine, EventInfo eventInfoInInbox)
        {
            if (eventInfoInInbox == null)
            {
                base.Logger.OnWait(machine.Id, machine.CurrentStateName, string.Empty);
                machine.Info.IsWaitingToReceive = true;
            }
        }

        /// <summary>
        /// Notifies that a machine received an <see cref="Event"/> that it was waiting for.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="eventInfo">EventInfo</param>
        internal override void NotifyReceivedEvent(Machine machine, EventInfo eventInfo)
        {
            base.Logger.OnReceive(machine.Id, machine.CurrentStateName, eventInfo.EventName, wasBlocked:true);

            lock (machine)
            {
                System.Threading.Monitor.Pulse(machine);
                machine.Info.IsWaitingToReceive = false;
            }
        }

        /// <summary>
        /// Notifies that a machine has halted.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="inbox">Machine inbox.</param>
        internal override void NotifyHalted(Machine machine, LinkedList<EventInfo> inbox)
        {
            base.Logger.OnHalt(machine.Id, inbox.Count);
            this.MachineMap.TryRemove(machine.Id, out machine);

            if (this.IsOnEventDroppedHandlerRegistered())
            {
                foreach (var evinfo in inbox)
                {
                    this.TryHandleDroppedEvent(evinfo.Event, machine.Id);
                }
            }
        }

        #endregion

        #region cleanup

        /// <summary>
        /// Disposes runtime resources.
        /// </summary>
        public override void Dispose()
        {
            this.Monitors.Clear();
            this.MachineMap.Clear();
            base.Dispose();
        }

        #endregion
    }
}
