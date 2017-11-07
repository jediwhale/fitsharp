// Copyright © 2017 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Runner;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Samples;

namespace fit.Test.Acceptance {
    public class FolderRunnerStory {

        public FolderRunnerStory() {
            reporter = new CollectingReporter();
        }

        public void Run(string[] theArguments) {
            TestClock.Instance.Now = new DateTime(2006, 12, 6, 13, 14, 15);
            TestClock.Instance.UtcNow = new DateTime(2006, 12, 6, 13, 14, 15);
            Clock.Instance = TestClock.Instance;
            var shell = new Shell(reporter, new FileSystemModel(), theArguments);
            shell.Run();
            Results = ((FolderRunner) shell.Runner).Results;
            Clock.Instance = new Clock();
        }

        public string Results { get; private set; }

        public string[] ConsoleOutput {
            get {
                return Output.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string Output { get { return reporter.Output; } }

        readonly CollectingReporter reporter;
    }
}
