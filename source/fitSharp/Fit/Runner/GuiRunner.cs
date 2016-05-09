// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Machine.Engine;

namespace fitSharp.Fit.Runner {
    public class GuiRunner {
        public delegate void ProcessorCountsChangedHandler(CellProcessor processor);
        public event ProcessorCountsChangedHandler ProcessorCountsChanged;

        public delegate void StartTestHandler(string testName);
        public event StartTestHandler StartTest;

        public delegate void EndTestHandler(string testName, TestCounts counts);
        public event EndTestHandler EndTest;

        public void Run(IList<string> commandLineArguments) {
            var memory = new TypeDictionary();
            var reporter = new CollectingReporter();
            var runner = new SuiteRunnerShell(memory, reporter, NewService);
            runner.SuiteRunner.StartTest += OnStartTest;
            runner.SuiteRunner.EndTest += OnEndTest;
            runner.Run(commandLineArguments, memory);
        }

        CellProcessor NewService(Memory memory) {
            processor = new CellProcessorBase(memory, memory.GetItem<CellOperators>());
            processor.TestStatus.TestCountChanged += OnTestCountChanged;
            return processor;
        }

        void OnTestCountChanged(TestCounts counts) {
            if (ProcessorCountsChanged != null) ProcessorCountsChanged(processor);
        }

        void OnStartTest(string testName) {
            if (StartTest != null) StartTest(testName);
        }

        void OnEndTest(string testName) {
            if (EndTest != null) EndTest(testName, processor.TestStatus.Counts);
        }

        CellProcessor processor;
    }
}
