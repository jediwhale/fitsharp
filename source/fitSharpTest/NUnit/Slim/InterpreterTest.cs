// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Text;
using fitSharp.IO;
using fitSharp.Machine.Model;
using fitSharp.Slim.Model;
using fitSharp.Slim.Service;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture] public class InterpreterTest {
        [Test] public void MultipleStepsAreExecuted() {
            var instructions = new SlimTree()
                .AddBranch(Instructions.MakeSampleClass())
                .AddBranch(Instructions.ExecuteMethod("samplemethod"))
                .AddBranch(Instructions.ExecuteMethod("samplemethod"));
            SampleClass.MethodCount = 0;
            ExecuteInstructions(instructions);
            Assert.AreEqual(2, SampleClass.MethodCount);
        }

        [Test] public void StopTestExceptionSkipsRemainingSteps() {
            var instructions = new SlimTree()
                .AddBranch(Instructions.MakeSampleClass())
                .AddBranch(Instructions.ExecuteMethod("samplemethod"))
                .AddBranch(Instructions.ExecuteAbortTest())
                .AddBranch(Instructions.ExecuteMethod("samplemethod"));
            SampleClass.MethodCount = 0;
            ExecuteInstructions(instructions);
            Assert.AreEqual(1, SampleClass.MethodCount);
        }

        [Test] public void EmptyInstructionReturnEmptyList() {
            Assert.AreEqual("Slim -- V0.0\n000009:[000000:]", ExecuteInstructions("[000000:]"));
        }

        private static void ExecuteInstructions(Tree<string> instructions) {
            string instructionString = new fitSharp.Slim.Service.Document(instructions).ToString();
            ExecuteInstructions(instructionString);
        }

        private static string ExecuteInstructions(string instructionString) {
            var testSocket = new TestSocket(string.Format("{0:000000}:{1}", instructionString.Length, instructionString));
            var messenger = new Messenger(testSocket);
            var interpreter = new Interpreter(messenger, string.Empty, new Service());
            interpreter.ProcessInstructions();
            return testSocket.Output;
        }

        private class TestSocket: SocketModel {
            private byte[] input;
            private int next;
            public string Output = string.Empty;

            public TestSocket(string input) { this.input = Encoding.UTF8.GetBytes(input); }

            public int Receive(byte[] buffer, int offset, int bytesToRead) {
                if (next >= input.Length) {
                    next = 0;
                    input = Encoding.UTF8.GetBytes("000003:bye");
                }
                int bytesRead = Math.Min(bytesToRead, input.Length - next);
                Array.Copy(input, next, buffer, offset, bytesRead);
                next += bytesRead;
                return bytesRead;
            }

            public void Send(byte[] buffer) {
			    var characters = new char[buffer.Length];
			    int charCount = Encoding.UTF8.GetDecoder().GetChars(buffer, 0, buffer.Length, characters, 0);
			    Output += new StringBuilder(charCount).Append(characters, 0, charCount).ToString();
            }

            public void Close() {}
        }

    }
}
