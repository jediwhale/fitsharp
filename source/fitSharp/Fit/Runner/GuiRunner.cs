// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Machine.Engine;

namespace fitSharp.Fit.Runner {
    public class GuiRunner {
        public void Run(IList<string> commandLineArguments) {
            var memory = new TypeDictionary();
            var reporter = new CollectingReporter();
            var runner = new SuiteRunnerShell();
            runner.Run(commandLineArguments, memory, reporter, NewService);
        }

        public void Poll(Action<CellProcessor> poll) {
            lock (this) {
                poll(processor);
            }
        }

        CellProcessor NewService(Memory memory) {
            lock (this) {
                processor = new CellProcessorBase(memory, memory.GetItem<CellOperators>());
                return processor;
            }
        }

        CellProcessor processor;
    }
}
