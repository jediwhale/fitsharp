// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fitSharp.Fit.Runner {
    public class FolderRunner: Runnable {

        public int Run(IList<string> commandLineArguments, Memory memory, ProgressReporter reporter) {
            return new SuiteRunnerShell(memory, reporter, m => new CellProcessorBase(m, m.GetItem<CellOperators>())).Run(commandLineArguments, memory);
        }
    }
}
