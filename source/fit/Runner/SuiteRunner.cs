// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Machine.Application;

namespace fit.Runner {
    public interface StoryTestSuite {
        string Name { get; }
        void Select(string theTestPage);
        IEnumerable<StoryTestPage> Pages { get; }
        StoryTestPage SuiteSetUp { get; }
        StoryTestPage SuiteTearDown { get; }
        IEnumerable<StoryTestSuite> Suites { get; }
        void Finish();
    }

    public delegate void TestCountsHandler(TestCounts counts);

    public interface StoryTestPage {
        string Name { get; }
        void ExecuteStoryPage(ResultWriter resultWriter, TestCountsHandler handler);
        bool IsTest { get; }
    }
    
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
                page.ExecuteStoryPage(resultWriter, HandleTestStatus);
	        }
	        catch (Exception e) {
	            myReporter.Write(e.ToString());
	        }
	    }

        private void HandleTestStatus(TestCounts counts) {
	        myReporter.Write(counts.Letter);
	        TestCounts.TallyCounts(counts);
        }
	}
}
