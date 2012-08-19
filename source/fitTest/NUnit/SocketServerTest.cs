// Copyright © 2012 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Reflection;
using fit.Runner;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Fixtures;
using fitSharp.IO;
using fitSharp.Samples;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class SocketServerTest {
        private string resultTables;

        [SetUp] public void SetUp() {
            resultTables = string.Empty;
        }

        [Test] public void StoryTestIsExecuted() {
            var service = new Service.Service();
            service.AddNamespace("fitlibrary");
            service.ApplicationUnderTest.AddAssembly("fit.dll");
            RunTest(service, "<table><tr><td>do</td></tr></table><table><tr><td>do</td></tr></table>");
            Assert.AreEqual("<table><tr><td><span class=\"fit_interpreter\">do</span></td></tr></table><table><tr><td><span class=\"fit_interpreter\">do</span></td></tr></table>", resultTables);
        }

        [Test] public void RegularTestUseCopyOfMemory() {
            var service = new Service.Service();
            RunMemoryTest(service, "Regular");
            Assert.IsFalse(service.Memory.HasItem<TestItem>());
        }

        private void RunMemoryTest(Service.Service service, string leader) {
            service.ApplicationUnderTest.AddAssembly(Assembly.GetExecutingAssembly().CodeBase);
            string tables = leader + "<table><tr><td>configure</td><td>fit.Test.NUnit.TestItem</td><td>Method</td></tr></table>";
            RunTest(service, tables);
        }

        [Test] public void SuiteSetUpUpdatesMemory() {
            var service = new Service.Service();
            RunMemoryTest(service, "SuiteSetUp");
            Assert.IsTrue(service.Memory.HasItem<TestItem>());
        }

        [Test] public void ParseExceptionIsRecorded() {
            var service = new Service.Service();
            const string tables = "<table>garbage</table>";
            RunTest(service, tables);
            Assert.IsTrue(resultTables.Contains("class=\"error\""), resultTables);
            Assert.IsTrue(resultTables.Contains("Unable to parse input. Input ignored."), resultTables);
        }

        private void RunTest(CellProcessor service, string tables) {
            var socket = new TestSocket();
            socket.PutByteString(Protocol.FormatInteger(tables.Length));
            socket.PutByteString(tables);
            socket.PutByteString(Protocol.FormatInteger(0));
            var server = new SocketServer(new FitSocket(socket, new NullReporter()), service, new NullReporter(), false);
            server.ProcessTestDocuments(new StoryTestStringWriter(service).ForTables(s => resultTables += s));
            Assert.IsFalse(socket.isOpen);
        }
    }

    public class TestItem {
        public void Method() {}
    }
}
