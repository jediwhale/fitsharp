// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Fixtures;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fitSharp.Fit.Runner {
	public class SuiteRunner {
        public delegate void StartTestHandler(string testName);
        public event StartTestHandler StartTest;

        public delegate void EndTestHandler(string testName);
        public event EndTestHandler EndTest;

	    public TestCounts TestCounts { get; private set; }
	    private readonly ProgressReporter myReporter;
	    private ResultWriter resultWriter;
	    private readonly Memory memory;
	    readonly Func<Memory, CellProcessor> newService;

	    public SuiteRunner(Memory memory, ProgressReporter theReporter, Func<Memory, CellProcessor> newService) {
		    TestCounts = new TestCounts();
		    myReporter = theReporter;
		    this.memory = memory;
		    this.newService = newService;
		}

	    public void Run(StoryTestSuite theSuite, string theSelectedFile) {
            var now = DateTime.Now;
            resultWriter = CreateResultWriter();
            if (!string.IsNullOrEmpty(theSelectedFile)) theSuite.Select(theSelectedFile);

	        RunFolder(theSuite, memory.GetItem<Settings>().DryRun);

            resultWriter.WriteFinalCount(TestCounts);
            resultWriter.Close();

            if (!memory.GetItem<Settings>().DryRun)
                myReporter.Write(string.Format("\n{0}, time: {1}\n", TestCounts.Description, DateTime.Now - now));
	    }

	    void TestStarted(string testName) {
	        if (StartTest != null) StartTest(testName);
	    }

	    void TestEnded(string testName) {
	        if (EndTest != null) EndTest(testName);
	    }

	    private ResultWriter CreateResultWriter() {
	        if (memory.GetItem<Settings>().XmlOutput != null) {
	            return new XmlResultWriter(memory.GetItem<Settings>().XmlOutput,
	                                       new FileSystemModel(memory.GetItem<Settings>().CodePageNumber));
	        }
	        return new NullResultWriter();
	    }

	    private void RunFolder(StoryTestSuite theSuite, bool dryRun) {
	        var executor = SelectExecutor(dryRun);

	        StoryTestPage suiteSetUp = theSuite.SuiteSetUp;
            if (suiteSetUp != null) executor.Do(suiteSetUp);
	        foreach (StoryTestPage testPage in theSuite.Pages) {
                try {
                    TestStarted(testPage.Name.Name);
                    executor.Do(testPage);
                    TestEnded(testPage.Name.Name);
                }
                catch (System.Exception e) {
	                myReporter.Write(e.ToString());
	            }
                if (executor.SuiteIsAbandoned) break;
            }

            if (!executor.SuiteIsAbandoned) {
                foreach (StoryTestSuite childSuite in theSuite.Suites) {
                    RunFolder(childSuite, dryRun);
                }
                StoryTestPage suiteTearDown = theSuite.SuiteTearDown;
                if (suiteTearDown != null) executor.Do(suiteTearDown);
            }

	        if (!dryRun) theSuite.Finish();
	    }

	    private StoryTestPageExecutor SelectExecutor(bool dryRun) {
	        if (dryRun) return new ReportPage(myReporter);
	        return new ExecutePage(memory, resultWriter, HandleTestStatus, newService);
	    }

	    private void HandleTestStatus(TestCounts counts) {
	        myReporter.Write(counts.Letter);
	        TestCounts.TallyCounts(counts);
        }

        class ExecutePage: StoryTestPageExecutor {
            public ExecutePage(Memory memory, ResultWriter resultWriter,  Action<TestCounts> handleCounts, Func<Memory, CellProcessor> newService) {
                this.memory = memory;
                this.resultWriter = resultWriter;
                this.handleCounts = handleCounts;
                this.newService = newService;
            }

            public void Do(StoryTestPage page) {
                var elapsedTime = new ElapsedTime();
                var input = page.TestContent;
                if (string.IsNullOrEmpty(input)) {
                    page.WriteNonTest();
                    DoNoTest();
                }

                StoreCurrentlyExecutingPagePath(page.Name.Name);

	            var service = newService(memory);
                var writer = new StoryTestStringWriter(service);
                var storyTest = new StoryTest(service, writer).WithInput(input);

                if (!storyTest.IsExecutable) {
                    page.WriteNonTest();
                    DoNoTest();
	                return;
	            }

                storyTest.OnAbandonSuite(() => { SuiteIsAbandoned = true; });

                if (page.Name.IsSuitePage) {
                    storyTest.Execute();
                }
                else {
                    storyTest.Execute(newService(service.Memory.Copy()));
                }

                var pageResult = new PageResult(page.Name.Name, writer.Tables, writer.Counts, elapsedTime);
                page.WriteTest(pageResult);
                handleCounts(writer.Counts);
                resultWriter.WritePageResult(pageResult);
            }

            public void DoNoTest() {
                handleCounts(new TestCounts());
            }

            private void StoreCurrentlyExecutingPagePath(string path) {
                memory.GetItem<Context>().TestPagePath = new FilePath(path);
            }

            public bool SuiteIsAbandoned { get; private set; }

            readonly ResultWriter resultWriter;
            readonly Memory memory;
            readonly Action<TestCounts> handleCounts;
            readonly Func<Memory, CellProcessor> newService;
        }

        class ReportPage: StoryTestPageExecutor {
            public ReportPage(ProgressReporter reporter) {
                this.reporter = reporter;
            }

            public void Do(StoryTestPage page) {
                if (string.IsNullOrEmpty(page.TestContent)) return;
	            reporter.WriteLine(page.Name.Name);
            }

            public void DoNoTest() {}

            public bool SuiteIsAbandoned { get { return false; } }

            readonly ProgressReporter reporter;
        }
	}
}
