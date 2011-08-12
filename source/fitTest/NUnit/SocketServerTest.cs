// Copyright © 2010 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Runner;
using fitSharp.Fit.Model;
using fitSharp.IO;
using fitSharp.Test.Double;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class SocketServerTest {
        private string resultTables;
        private TestCounts resultCounts;

        [SetUp] public void SetUp() {
            resultTables = string.Empty;
        }

        [Test] public void StoryTestIsExecuted() {
            var service = new Service.Service();
            service.AddNamespace("fitlibrary");
            service.ApplicationUnderTest.AddAssembly("fit.dll");
            const string tables = "<table><tr><td>do</td></tr></table><table><tr><td>do</td></tr></table>";
            var socket = new TestSocket();
            socket.PutByteString("0000000070");
            socket.PutByteString(tables);
            socket.PutByteString("0000000000");
            var server = new SocketServer(new FitSocket(socket, new NullReporter()), service, new NullReporter(), false);
            server.ProcessTestDocuments(WriteResult);
            Assert.AreEqual(tables, resultTables);
        }

        [Test] public void ParseExceptionIsRecorded() {
            var service = new Service.Service();
            const string tables = "<table>garbage</table>";
            var socket = new TestSocket();
            socket.PutByteString("0000000022");
            socket.PutByteString(tables);
            socket.PutByteString("0000000000");
            var server = new SocketServer(new FitSocket(socket, new NullReporter()), service, new NullReporter(), false);
            server.ProcessTestDocuments(WriteResult);
            Assert.IsTrue(resultTables.Contains("class=\"error\""), resultTables);
            Assert.IsTrue(resultTables.Contains("Unable to parse input. Input ignored."), resultTables);
        }

        private void WriteResult(string tables, TestCounts counts) {
            resultTables += tables;
            resultCounts = counts;
        }
    }
}
