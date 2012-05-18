// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Runner;
using fitSharp.IO;
using fitSharp.Machine.Engine;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class HttpRunnerTest {
        [Test] public void ExecutesAPlainTextTest() {
            var server = new Server {Request = "fitLibrary.DoFixture\n"};
            var runner = new HttpRunner(server);
            runner.Run(null, new TypeDictionary(), null);
            Assert.AreEqual("test@<br /><div><table><tr><td><span class=\"fit_interpreter\">fitLibrary.DoFixture</span></td> </tr></table></div>", server.Reply);
        }

        [Test] public void DoesntExecuteAnExitCommand() {
            var server = new Server {Request = "exit"};
            var runner = new HttpRunner(server);
            runner.Run(null, new TypeDictionary(), null);
            Assert.AreEqual("Bye", server.Reply);
        }

        class Server: RequestReplyServer {
            public string Request;
            public string Reply;
            public void Run(Func<string, string> processRequest) {
                Reply = processRequest(Request);
            }
        }
    }
}
