// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using Xunit;

namespace Microsoft.PSharp.StaticAnalysis.Tests.Unit
{
    public class BasicLoopTest : BaseTest
    {
        [Fact]
        public void TestBasicLoop()
        {
            var test = @"
using Microsoft.PSharp;

namespace Foo {
class M : Machine
{
 [Start]
 [OnEntry(nameof(FirstOnEntryAction))]
 class First : MachineState { }

 void FirstOnEntryAction()
 {
  int k = 10;
  for (int i = 0; i < k; i++) { k = 2; }
  k = 3;
 }
}
}";
            base.AssertSucceeded(test, isPSharpProgram: false);
        }
    }
}
