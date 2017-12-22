// Copyright © 2017 Syterra Software Inc. All rights reserved.
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

	    public SuiteRunner(Memory memory, ProgressReporter theReporter, Func<Memory, CellProcessor> newService, FolderModel folderModel) {
		    TestCounts = new TestCounts();
		    myReporter = theReporter;
		    this.memory = memory;
		    this.newService = newService;
	        this.folderModel = folderModel;
	        myReport = new Report(memory.GetItem<Settings>().OutputFolder);
		}

	    public TestCounts TestCounts { get; }

	    public void Run(StoryTestSuite theSuite, string theSelectedFile) {
            var now = DateTime.Now;
            resultWriter = CreateResultWriter();

	        RunFolder(theSuite, memory.GetItem<Settings>().DryRun);

            resultWriter.WriteFinalCount(TestCounts);
            resultWriter.Close();

            if (!memory.GetItem<Settings>().DryRun)
                myReporter.Write($"\n{TestCounts.Description}, time: {DateTime.Now - now}\n");
	    }

	    void TestStarted(string testName) {
	        StartTest?.Invoke(testName);
	    }

	    void TestEnded(string testName) {
	        EndTest?.Invoke(testName);
	    }

	    ResultWriter CreateResultWriter() {
	        if (memory.GetItem<Settings>().XmlOutput != null) {
	            return new XmlResultWriter(memory.GetItem<Settings>().XmlOutput,
	                                       new FileSystemModel(memory.GetItem<Settings>().CodePageNumber));
	        }
	        return new NullResultWriter();
	    }

        void RunFolder(StoryTestSuite theSuite, bool dryRun) {
	        var executor = SelectExecutor(dryRun);
            foreach (var testPage in theSuite.AllPages) {
                if (!executor.SuiteIsAbandoned) {
                    try {
                        TestStarted(testPage.Name.Name);
                        executor.Do(testPage);
                        TestEnded(testPage.Name.Name);
                    }
                    catch (System.Exception e) {
	                    myReporter.Write(e.ToString());
	                }
                }
            }
            if (!dryRun) {
                myReport.Finish(folderModel);
            }
	    }

	    StoryTestPageExecutor SelectExecutor(bool dryRun) {
	        if (dryRun) return new ReportPage(myReporter);
	        return new ExecutePage(memory, resultWriter, HandleTestStatus, newService, myReport);
	    }

	    void HandleTestStatus(TestCounts counts) {
	        myReporter.Write(counts.Letter);
	        TestCounts.TallyCounts(counts);
        }

	    readonly ProgressReporter myReporter;
	    readonly Memory memory;
	    readonly Func<Memory, CellProcessor> newService;
	    readonly FolderModel folderModel;
	    readonly Report myReport;

	    ResultWriter resultWriter;

        class ExecutePage: StoryTestPageExecutor {
            public ExecutePage(Memory memory, ResultWriter resultWriter,  Action<TestCounts> handleCounts, Func<Memory, CellProcessor> newService, Report report) {
                this.memory = memory;
                this.resultWriter = resultWriter;
                this.handleCounts = handleCounts;
                this.newService = newService;
                this.report = report;
            }

            public bool SuiteIsAbandoned { get; private set; }

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
                report.ListFile(page.OutputFile, writer.Counts, elapsedTime);
                handleCounts(writer.Counts);
                resultWriter.WritePageResult(pageResult);
            }

            public void DoNoTest() {
                handleCounts(new TestCounts());
            }

            void StoreCurrentlyExecutingPagePath(string path) {
                memory.GetItem<Context>().TestPagePath = new FilePath(path);
            }


            readonly ResultWriter resultWriter;
            readonly Memory memory;
            readonly Action<TestCounts> handleCounts;
            readonly Func<Memory, CellProcessor> newService;
            readonly Report report;
        }

        class ReportPage: StoryTestPageExecutor {
            public ReportPage(ProgressReporter reporter) {
                this.reporter = reporter;
            }

            public bool SuiteIsAbandoned => false;

            public void Do(StoryTestPage page) {
                if (string.IsNullOrEmpty(page.TestContent)) return;
	            reporter.WriteLine(page.Name.Name);
            }

            public void DoNoTest() {}

            readonly ProgressReporter reporter;
        }
	}
}
