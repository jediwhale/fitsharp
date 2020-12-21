// Copyright © 2020 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.IO;
using fitSharp.Samples;
using fitSharp.Slim.Service;
using fitSharp.Test.Double.Slim;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture] public class InterpreterTest {

        [SetUp]
        public void SetUp() {
            port = new TestPort();
            interpreter = new Interpreter(new Messenger(new MessageChannel(port)), string.Empty, Builder.Service());
        }

        [Test] public void MultipleStepsAreExecuted() {
            var instructions = new Instructions()
                .MakeVariable("variable", typeof (SampleClass))
                .ExecuteMethod("samplemethod")
                .ExecuteMethod("samplemethod");
            SampleClass.MethodCount = 0;
            Execute(instructions.InstructionString);
            Assert.AreEqual(2, SampleClass.MethodCount);
        }

        [Test] public void StopTestExceptionSkipsRemainingSteps() {
            var instructions = new Instructions()
                .MakeVariable("variable", typeof (SampleClass))
                .ExecuteMethod("samplemethod")
                .ExecuteAbortTest()
                .ExecuteMethod("samplemethod");
            SampleClass.MethodCount = 0;
            Execute(instructions.InstructionString);
            Assert.AreEqual(1, SampleClass.MethodCount);
        }

        [Test] public void StopSuiteExceptionSkipsRemainingSteps() {
            var instructions = new Instructions()
                .MakeVariable("variable", typeof (SampleClass))
                .ExecuteMethod("samplemethod")
                .ExecuteAbortSuite()
                .ExecuteMethod("samplemethod");
            SampleClass.MethodCount = 0;
            Execute(instructions.InstructionString);
            Assert.AreEqual(1, SampleClass.MethodCount);
        }

        [Test] public void EmptyInstructionReturnEmptyList() {
            Assert.AreEqual("Slim -- V0.5\n000009:[000000:]", Execute("[000000:]"));
        }

        [Test] public void ExecutesMethodOnLibraryInstance() {
            var instructions = new Instructions()
                .MakeVariable("libraryvariable", typeof (SampleClass))
                .MakeVariable("variable", typeof (DummyClass))
                .ExecuteMethod("samplemethod");
            SampleClass.MethodCount = 0;
            Execute(instructions.InstructionString);
            Assert.AreEqual(1, SampleClass.MethodCount);
        }

        string Execute(string instructionString) {
            port.AddInput($"{instructionString.Length:000000}:{instructionString}" + "000003:bye");
            interpreter.ProcessInstructions();
            return port.Output;
        }

        Interpreter interpreter;
        TestPort port;
    }

    public class DummyClass {}
}
