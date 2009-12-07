// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture] public class InterpreterTest {
        [Test] public void MultipleStepsAreExecuted() {
            var instructions = new Instructions()
                .MakeSampleClass()
                .ExecuteMethod("samplemethod")
                .ExecuteMethod("samplemethod");
            SampleClass.MethodCount = 0;
            instructions.Execute();
            Assert.AreEqual(2, SampleClass.MethodCount);
        }

        [Test] public void StopTestExceptionSkipsRemainingSteps() {
            var instructions = new Instructions()
                .MakeSampleClass()
                .ExecuteMethod("samplemethod")
                .ExecuteAbortTest()
                .ExecuteMethod("samplemethod");
            SampleClass.MethodCount = 0;
            instructions.Execute();
            Assert.AreEqual(1, SampleClass.MethodCount);
        }

        [Test] public void EmptyInstructionReturnEmptyList() {
            Assert.AreEqual("Slim -- V0.0\n000009:[000000:]", TestInterpreter.ExecuteInstructions("[000000:]"));
        }

    }
}
