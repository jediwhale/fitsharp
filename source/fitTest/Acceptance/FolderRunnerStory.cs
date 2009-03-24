// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Runner;
using fitSharp.Machine.Application;
using fitSharp.Machine.Model;

namespace fit.Test.Acceptance {
    public class FolderRunnerStory: DomainAdapter {
        private readonly Shell shell;

        public object SystemUnderTest { get { return shell; } }

        public FolderRunnerStory() {
            shell = new Shell(new NullProgressReporter());
        }

        public void Run(string[] theArguments) {
            Clock.Instance = new TestClock();
            shell.Run(theArguments);
            Clock.Instance = new Clock();
        }

        public string Results { get { return ((FolderRunner) shell.Runner).Results; }}

        private class NullProgressReporter: ProgressReporter {
            public void Write(string theMessage) {}
        }

        private class TestClock: TimeKeeper {
            public DateTime Now {
                get { return new DateTime(2006, 12, 6, 13, 14, 15); }
            }
            public DateTime UtcNow {
                get { return new DateTime(2006, 12, 6, 13, 14, 15); }
            }
        }
    }
}