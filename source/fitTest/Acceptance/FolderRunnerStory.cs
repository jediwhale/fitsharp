// Copyright © 2010 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Runner;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Model;
using fitSharp.Test.Double;

namespace fit.Test.Acceptance {
    public class FolderRunnerStory: DomainAdapter {
        private readonly Shell shell;

        public object SystemUnderTest { get { return shell; } }

        public FolderRunnerStory() {
            shell = new Shell(new NullReporter(), new FileSystemModel());
        }

        public void Run(string[] theArguments) {
            TestClock.Instance.Now = new DateTime(2006, 12, 6, 13, 14, 15);
            TestClock.Instance.UtcNow = new DateTime(2006, 12, 6, 13, 14, 15);
            Clock.Instance = TestClock.Instance;
            shell.Run(theArguments);
            Clock.Instance = new Clock();
        }

        public string Results { get { return ((FolderRunner) shell.Runner).Results; }}
    }
}