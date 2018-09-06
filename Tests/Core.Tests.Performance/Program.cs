// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using System.Reflection;

using BenchmarkDotNet.Running;

namespace Microsoft.PSharp.Core.Tests.Performance
{
    /// <summary>
    /// The P# performance test runner.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //new CreateMachinesTest().CreateMachines();
            //new MailboxTest().SendMessages();
            BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly).Run(args);
        }
    }
}
