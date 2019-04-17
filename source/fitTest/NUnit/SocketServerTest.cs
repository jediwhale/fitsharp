// Copyright © 2019 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
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
            service.ApplicationUnderTest.AddAssembly(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fit.dll"));
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
            var port = new TestPort();
            port.Input = Protocol.FormatInteger(tables.Length);
            port.Input = tables;
            port.Input = Protocol.FormatInteger(0);
            var server = new SocketServer(new FitSocket(new MessageChannel(port), new NullReporter()), service, new NullReporter(), false);
            server.ProcessTestDocuments(new StoryTestStringWriter().ForTables(s => resultTables += s));
            Assert.IsFalse(port.IsOpen);
        }
    }

    public class TestItem {
        public void Method() {}
    }
}
