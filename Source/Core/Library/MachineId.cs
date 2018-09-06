// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using System.Threading;

namespace Microsoft.PSharp
{
    /// <summary>
    /// Unique machine id.
    /// </summary>
    [DataContract]
    public sealed class MachineId
    {
        /// <summary>
        /// The P# runtime that executes the machine with this id.
        /// </summary>
        public PSharpRuntime Runtime { get; private set; }

        /// <summary>
        /// Name of the machine.
        /// </summary>
        [DataMember]
        public readonly string Name;

        /// <summary>
        /// Optional friendly name of the machine.
        /// </summary>
        [DataMember]
        private readonly string FriendlyName;

        /// <summary>
        /// Type of the machine with this id.
        /// </summary>
        [DataMember]
        public readonly string Type;

        /// <summary>
        /// Unique id value.
        /// </summary>
        [DataMember]
        public readonly ulong Value;

        /// <summary>
        /// Generation of the runtime that created this machine id.
        /// </summary>
        [DataMember]
        public readonly ulong Generation;

        /// <summary>
        /// Endpoint.
        /// </summary>
        [DataMember]
        public readonly string Endpoint;

        /// <summary>
        /// Creates a new machine id.
        /// </summary>
        /// <param name="type">Machine type</param>
        /// <param name="friendlyName">Friendly machine name</param>
        /// <param name="runtime">PSharpRuntime</param>
        internal MachineId(Type type, string friendlyName, PSharpRuntime runtime)
        {
            this.FriendlyName = friendlyName;
            this.Runtime = runtime;
            this.Endpoint = this.Runtime.NetworkProvider.GetLocalEndpoint();

            // Atomically increments and safely wraps into an unsigned long.
            this.Value = (ulong)Interlocked.Increment(ref runtime.MachineIdCounter) - 1;

            // Checks for overflow.
            this.Runtime.Assert(this.Value != ulong.MaxValue, "Detected MachineId overflow.");

            this.Generation = runtime.Configuration.RuntimeGeneration;

            this.Type = type.FullName;
            if (friendlyName != null && friendlyName.Length > 0)
            {
                this.Name = string.Format("{0}({1})", friendlyName, this.Value);
            }
            else
            {
                this.Name = string.Format("{0}({1})", this.Type, this.Value);
            }
        }

        /// <summary>
        /// Create a fresh MachineId borrowing information from a given id.
        /// </summary>
        /// <param name="mid">MachineId</param>
        internal MachineId(MachineId mid)
        {
            this.Runtime = mid.Runtime;
            this.Endpoint = mid.Endpoint;

            // Atomically increments and safely wraps into an unsigned long.
            this.Value = (ulong)Interlocked.Increment(ref this.Runtime.MachineIdCounter) - 1;

            // Checks for overflow.
            this.Runtime.Assert(this.Value != ulong.MaxValue, "Detected MachineId overflow.");

            this.Generation = mid.Generation;
            this.Type = mid.Type;

            if (this.FriendlyName != null && this.FriendlyName.Length > 0)
            {
                this.Name = string.Format("{0}({1})", this.FriendlyName, this.Value);
            }
            else
            {
                this.Name = string.Format("{0}({1})", this.Type, this.Value);
            }
        }

        /// <summary>
        /// Bind the machine id.
        /// </summary>
        /// <param name="runtime">PSharpRuntime</param>
        internal void Bind(PSharpRuntime runtime)
        {
            this.Runtime = runtime;
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal
        /// to the current System.Object.
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            MachineId mid = obj as MachineId;
            if (mid == null)
            {
                return false;
            }

            return this.Value == mid.Value && this.Generation == mid.Generation;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>int</returns>
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + this.Value.GetHashCode();
            hash = hash * 23 + this.Generation.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Returns a string that represents the current machine id.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
