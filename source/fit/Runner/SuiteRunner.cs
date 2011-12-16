// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Model;
using fitSharp.Fit.Runner;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fit.Runner {
	public class SuiteRunner {
	    
	    public TestCounts TestCounts { get; private set; }
	    private readonly ProgressReporter myReporter;
	    private ResultWriter resultWriter;
	    private readonly Memory memory;

		public SuiteRunner(Memory memory, ProgressReporter theReporter) {
		    TestCounts = new TestCounts();
		    myReporter = theReporter;
		    this.memory = memory;
		}

	    public void Run(StoryTestSuite theSuite, string theSelectedFile) {
            resultWriter = CreateResultWriter();
            if (!string.IsNullOrEmpty(theSelectedFile)) theSuite.Select(theSelectedFile);

	        RunFolder(theSuite, memory.GetItem<Settings>().DryRun);

            resultWriter.WriteFinalCount(TestCounts);
            resultWriter.Close();
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
                    executor.Do(testPage);
                }
                catch (Exception e) {
	                myReporter.Write(e.ToString());
	            }
            }

	        foreach (StoryTestSuite childSuite in theSuite.Suites) {
                RunFolder(childSuite, dryRun);
	        }
	        StoryTestPage suiteTearDown = theSuite.SuiteTearDown;
            if (suiteTearDown != null) executor.Do(suiteTearDown);
	        if (!dryRun) theSuite.Finish();
	    }

	    private StoryTestPageExecutor SelectExecutor(bool dryRun) {
	        if (dryRun) return new ReportPage(myReporter);
	        return new ExecutePage(memory, resultWriter, HandleTestStatus);
	    }

	    private void HandleTestStatus(TestCounts counts) {
	        myReporter.Write(counts.Letter);
	        TestCounts.TallyCounts(counts);
        }

        class ExecutePage: StoryTestPageExecutor {
            public ExecutePage(Memory memory, ResultWriter resultWriter,  Action<TestCounts> handleCounts) {
                this.memory = memory;
                this.resultWriter = resultWriter;
                this.handleCounts = handleCounts;
            }

            public void Do(StoryTestPage page) {
                var elapsedTime = new ElapsedTime();
                var input = page.TestContent;
                if (string.IsNullOrEmpty(input)) {
                    page.WriteNonTest();
                    DoNoTest();
                }

                StoreCurrentlyExecutingPagePath(page.Name.Name);

	            var service = new Service.Service(memory);
                var writer = new StoryTestStringWriter(service);
                var storyTest = new StoryTest(service, writer).WithInput(input);

                if (!storyTest.IsExecutable) {
                    page.WriteNonTest();
                    DoNoTest();
	                return;
	            }

                if (page.Name.IsSuitePage) {
                    storyTest.Execute();
                }
                else {
                    storyTest.Execute(new Service.Service(service));
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
                memory.GetItem<Context>().TestPagePath = path;
            }

            readonly ResultWriter resultWriter;
            readonly Memory memory;
            readonly Action<TestCounts> handleCounts;
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

            readonly ProgressReporter reporter;
        }
	}
}
