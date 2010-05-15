// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Model;
using fitSharp.IO;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Runner {
    public class SocketServer {
	    private static readonly IdentifierName suiteSetupIdentifier = new IdentifierName("suitesetup");

        private readonly FitSocket socket;
        private readonly Processor<Cell> service;
        private readonly ProgressReporter reporter;

	    private bool IMaybeProcessingSuiteSetup;

        public SocketServer(FitSocket socket, Processor<Cell> service, ProgressReporter reporter, bool suiteSetUpIsAnonymous) {
            this.service = service;
            this.reporter = reporter;
            this.socket = socket;
            IMaybeProcessingSuiteSetup = suiteSetUpIsAnonymous;
        }

		public void ProcessTestDocuments(Action<string, TestCounts> writer)
		{
			string document;

            while ((document = socket.ReceiveDocument()).Length > 0)
			{
                reporter.WriteLine("processing document of size: " + document.Length);
                ProcessTestDocument(document, writer);
		        IMaybeProcessingSuiteSetup = false;
			}
		    reporter.WriteLine("\ncompletion signal recieved");
		}

        private void ProcessTestDocument(string document, Action<string, TestCounts> writer)
		{
			try
			{
                Tree<Cell> result = service.Compose(new StoryTestString(document));
                var parse = result != null ? (Parse)result.Value : null;
			    var storyTest = new StoryTest(parse, (tables, counts) => WriteResults(tables, counts, writer));
			    reporter.WriteLine(parse.Leader);
			    if (suiteSetupIdentifier.IsStartOf(parse.Leader) || IMaybeProcessingSuiteSetup)
                    storyTest.ExecuteOnConfiguration(service.Configuration);
                else
				    storyTest.Execute(service.Configuration);
			}
			catch (Exception e)
			{
			    var testStatus = new TestStatus();
				var parse = new Parse("div", "Unable to parse input. Input ignored.", null, null);
			    testStatus.MarkException(parse, e);
			    WriteResults(parse, testStatus.Counts, writer);
			}
		}

        private void WriteResults(Tree<Cell> tables, TestCounts counts, Action<string, TestCounts> writer) {
            string testResult = service.ParseTree<Cell, StoryTestString>(tables).ToString();
            writer(testResult, counts);
        }
    }
}
