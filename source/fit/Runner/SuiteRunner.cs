// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using fitSharp.Fit.Model;
using fitSharp.Fit.Runner;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Extension;
using fitSharp.Machine.Model;

namespace fit.Runner {
	public class SuiteRunner { //: Runnable {
	    
	    public TestCounts TestCounts { get; private set; }
        private string mySelection = string.Empty;
	    private ProgressReporter myReporter;
	    private ResultWriter resultWriter;
	    private readonly Configuration configuration;

		public SuiteRunner(Configuration configuration, ProgressReporter theReporter) {
		    TestCounts = new TestCounts();
		    myReporter = theReporter;
		    this.configuration = configuration;
		}

	    public int Run(Configuration configuration, IEnumerable<string> arguments, ProgressReporter reporter) {
	        ParseArguments(arguments);
	        myReporter = reporter;
		    Run(new StoryTestFolder(configuration, new FileSystemModel()), mySelection);
	        return 0; //todo: return counts exceptions + wrong or whatever
	    }

        private void ParseArguments(IEnumerable<string> arguments) { //todo: throw argumentexception
            string lastArgument = string.Empty;
            foreach (string argument in arguments) {
                if (lastArgument == "-s") mySelection = argument;
                lastArgument = argument;
            }
        }

	    public void Run(StoryTestSuite theSuite, string theSelectedFile) {
            resultWriter = CreateResultWriter();
            if (theSelectedFile.Length > 0) theSuite.Select(theSelectedFile);

	        RunFolder(theSuite);

            resultWriter.WriteFinalCount(TestCounts);
            resultWriter.Close();
	    }

	    private ResultWriter CreateResultWriter() {
	        if (configuration.GetItem<Settings>().XmlOutput != null) {
	            return new XmlResultWriter(configuration.GetItem<Settings>().XmlOutput, new FileSystemModel());
	        }
	        return new NullResultWriter();
	    }

	    private void RunFolder(StoryTestSuite theSuite) {
	        StoryTestPage suiteSetUp = theSuite.SuiteSetUp;
            if (suiteSetUp != null) ExecuteStoryPage(suiteSetUp);
            foreach (StoryTestPage testPage in theSuite.Pages) {
                ExecuteStoryPage(testPage);
            }

	        foreach (StoryTestSuite childSuite in theSuite.Suites) {
                RunFolder(childSuite);
	        }
	        StoryTestPage suiteTearDown = theSuite.SuiteTearDown;
            if (suiteTearDown != null) ExecuteStoryPage(suiteTearDown);
	        theSuite.Finish();
	    }

	    private void ExecuteStoryPage(StoryTestPage page) {
	        try {
                page.ExecuteStoryPage(ExecutePage, resultWriter, HandleTestStatus);
	        }
	        catch (Exception e) {
	            myReporter.Write(e.ToString());
	        }
	    }

        private void HandleTestStatus(TestCounts counts) {
	        myReporter.Write(counts.Letter);
	        TestCounts.TallyCounts(counts);
        }

	    private void ExecutePage(StoryTestString input, Action<StoryTestString, TestCounts> handleResults,
	                             Action handleNoTest) {
	        var service = configuration.GetItem<Service.Service>();
            FitVersionFixture.Reset();
	        Tree<Cell> result = service.Compose(input);
	        if (result == null) {
	            handleNoTest();
	            return;
	        }
	        new StoryTest((Parse) result.Value,
	                      (tables, counts) => handleResults(service.ParseTree<Cell, StoryTestString>(tables), counts)).
	            Execute();
	    }
	}
}
