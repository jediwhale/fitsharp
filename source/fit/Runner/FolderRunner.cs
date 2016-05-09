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

        public int Run(IList<string> commandLineArguments, Memory memory, ProgressReporter reporter) {
            runner = new fitSharp.Fit.Runner.SuiteRunnerShell(memory, reporter, m => new Service.Service(m));
            return runner.Run(commandLineArguments, memory);
        }

        public string Results { get { return runner.Results; } }

        fitSharp.Fit.Runner.SuiteRunnerShell runner;
    }
}
