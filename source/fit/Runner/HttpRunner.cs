// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using fitSharp.Fit.Fixtures;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fit.Runner {
    public class HttpRunner: Runnable {
        public HttpRunner(): this(new HttpServer(exitRequest)) {}

        public HttpRunner(RequestReplyServer server) {
            this.server = server;
        }

        public int Run(IList<string> commandLineArguments, Memory memory, ProgressReporter reporter) {
            server.Run(s => ProcessRequest(s, memory));
            return 0;
        }

        static string ProcessRequest(string request, Memory memory) {
            if (request == exitRequest) return exitReply;
            var service = new Service.Service(memory);
            var writer = new StoryTestStringWriter(service);
            var storyTest = new StoryTest(service, writer).WithInput("test@\n" + request);
            storyTest.Execute();
            return writer.Tables;
        }

        const string exitRequest = "exit";
        const string exitReply = "Bye";
        readonly RequestReplyServer server;
    }
}
