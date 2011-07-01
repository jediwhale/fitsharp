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
using fitSharp.Machine.Model;

namespace fit.Runner {
	public class SuiteRunner {
	    
	    public TestCounts TestCounts { get; private set; }
	    private readonly ProgressReporter myReporter;
	    private ResultWriter resultWriter;
	    private readonly Configuration configuration;

		public SuiteRunner(Configuration configuration, ProgressReporter theReporter) {
		    TestCounts = new TestCounts();
		    myReporter = theReporter;
		    this.configuration = configuration;
		}

	    public void Run(StoryTestSuite theSuite, string theSelectedFile) {
            resultWriter = CreateResultWriter();
            if (!string.IsNullOrEmpty(theSelectedFile)) theSuite.Select(theSelectedFile);

	        RunFolder(theSuite, configuration.GetItem<Settings>().DryRun);

            resultWriter.WriteFinalCount(TestCounts);
            resultWriter.Close();
	    }

	    private ResultWriter CreateResultWriter() {
	        if (configuration.GetItem<Settings>().XmlOutput != null) {
	            return new XmlResultWriter(configuration.GetItem<Settings>().XmlOutput,
	                                       new FileSystemModel(configuration.GetItem<Settings>().CodePageNumber));
	        }
	        return new NullResultWriter();
	    }

	    private void RunFolder(StoryTestSuite theSuite, bool dryRun) {
	        var pageAction = SelectPageAction(dryRun);
	        StoryTestPage suiteSetUp = theSuite.SuiteSetUp;
            if (suiteSetUp != null) pageAction(suiteSetUp);
            foreach (StoryTestPage testPage in theSuite.Pages) {
                pageAction(testPage);
            }

	        foreach (StoryTestSuite childSuite in theSuite.Suites) {
                RunFolder(childSuite, dryRun);
	        }
	        StoryTestPage suiteTearDown = theSuite.SuiteTearDown;
            if (suiteTearDown != null) pageAction(suiteTearDown);
	        if (!dryRun) theSuite.Finish();
	    }

        private Action<StoryTestPage> SelectPageAction(bool dryRun) {
            if (dryRun) return DryRunStoryPage;
            return ExecuteStoryPage;
        }

	    private void ExecuteStoryPage(StoryTestPage page) {
	        try {
                page.ExecuteStoryPage(ExecutePage, resultWriter, HandleTestStatus);
	        }
	        catch (Exception e) {
	            myReporter.Write(e.ToString());
	        }
	    }

	    private void DryRunStoryPage(StoryTestPage page) {
            page.ExecuteStoryPage(ReportPageName, new NullResultWriter(), ignore => {});
	    }

	    private void ReportPageName(StoryPageName pageName, StoryTestString input, Action<StoryTestString, TestCounts> handleResults,
	                             Action handleNoTest) {
	        myReporter.WriteLine(pageName.Name);
	    }

	    private void HandleTestStatus(TestCounts counts) {
	        myReporter.Write(counts.Letter);
	        TestCounts.TallyCounts(counts);
        }

	    private void ExecutePage(StoryPageName pageName, StoryTestString input, Action<StoryTestString, TestCounts> handleResults,
	                             Action handleNoTest) {
	        var service = new Service.Service(configuration);
	        Tree<Cell> result = service.Compose(input);
	        if (result == null || result.Branches.Count == 0) {
	            handleNoTest();
	            return;
	        }
	        var storyTest = new StoryTest((Parse) result,
	                                      (tables, counts) =>
	                                      handleResults(service.ParseTree<Cell, StoryTestString>(tables), counts));
            if (pageName.IsSuitePage) {
	            storyTest.ExecuteOnConfiguration(configuration);
            }
            else {
	            storyTest.Execute(configuration);
            }
	    }
	}
}
