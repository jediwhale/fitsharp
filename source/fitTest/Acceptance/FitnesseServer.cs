// Copyright © 2020 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Runner;
using fitSharp.Fit.Exception;
using fitSharp.Fit.Fixtures;
using fitSharp.IO;
using fitSharp.Samples;

namespace fit.Test.Acceptance {

    public class FitnesseServer {

        public void AddPage(string[] lines) {
            var document = String.Join(Environment.NewLine, lines) + Environment.NewLine;
            port.AddInput(Protocol.FormatInteger(document.Length));
            port.AddInput(document);
        }
        
        public void Run() {
            var service = new Service.Service();
            service.ApplicationUnderTest.AddAssembly(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fit.dll"));
            port.AddInput(Protocol.FormatInteger(0));
            var server = new SocketServer(new FitSocket(new MessageChannel(port), new NullReporter()), service, new NullReporter(), false);
            try {
                server.ProcessTestDocuments(new StoryTestStringWriter().ForTables(s => Results += s)
                    .ForCounts(_ => Results += " (Test completed)"));
            }
            catch (AbandonSuiteException) {
                Results += " (Suite abandoned)";
            }
        }
        
        public string Results;
        readonly TestPort port = new TestPort();
    }
}