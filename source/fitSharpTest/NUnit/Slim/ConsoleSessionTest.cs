// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.IO;
using fitSharp.Slim.Service;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture]
    public class ConsoleSessionTest {

        [SetUp]
        public void SetUp() {
            saveOut = Console.Out;
            saveIn = Console.In;
            saveError = Console.Error;
            testOut = new StringWriter();
            Console.SetOut(testOut);
            testError = new StringWriter();
            Console.SetError(testError);
            session = new ConsoleSession();
        }

        [TearDown]
        public void TearDown() {
            Console.SetOut(saveOut);
            Console.SetIn(saveIn);
            Console.SetError(saveError);
        }

        [Test]
        public void WritesToConsole() {
            session.Write("hey\u1234\u0000");
            Assert.AreEqual("hey\u1234\u0000", testOut.ToString());
        }

        [Test]
        public void ReadsFromConsole() {
            var testIn = new StringReader("in\r\n\u1234\u0000it");
            Console.SetIn(testIn);
            var firstInput = session.Read(4);
            var secondInput = session.Read(2);
            Assert.AreEqual("in\r\n", firstInput);
            Assert.AreEqual("\u1234\u0000", secondInput);
        }

        [Test]
        public void WritesEncodedOutputBeforeWrite() {
            Console.WriteLine("between");
            Console.Error.WriteLine("you");
            WriteOutput();
            Assert.AreEqual("SOUT :between\r\nSERR :you\r\n", testError.ToString());
        }

        [Test]
        public void EncodesEachLineOfOutput() {
            Console.WriteLine("this is");
            Console.WriteLine("between");
            Console.Error.WriteLine("you");
            Console.Error.WriteLine("and me");
            WriteOutput();
            Assert.AreEqual("SOUT :this is\r\nSOUT :between\r\nSERR :you\r\nSERR :and me\r\n", testError.ToString());
        }

        [Test]
        public void OutputNotEncodedAfterClose() {
            session.Close();
            Console.WriteLine("between");
            Console.Error.WriteLine("you");
            Assert.AreEqual("between\r\n", testOut.ToString());
            Assert.AreEqual("you\r\n", testError.ToString());
        }

        void WriteOutput() {
            session.Write("something");
        }
        
        TextWriter saveOut;
        TextReader saveIn;
        StringWriter testOut;
        ConsoleSession session;
        TextWriter saveError;
        StringWriter testError;
    }
}
