// Copyright © 2016 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fit.Runner {
    public class FolderRunner: Runnable {

        public FolderRunner() {
            runner = new fitSharp.Fit.Runner.SuiteRunnerShell();
        }

        public int Run(IList<string> commandLineArguments, Memory memory, ProgressReporter reporter) {
            return runner.Run(commandLineArguments, memory, reporter, m => new Service.Service(m));
        }

        public string Results { get { return runner.Results; } }

        readonly fitSharp.Fit.Runner.SuiteRunnerShell runner;
    }
}
