// Copyright © 2011 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Runner {
    public class SocketServer {
        private const string parseError = "Unable to parse input. Input ignored.";
        private static readonly IdentifierName suiteSetupIdentifier = new IdentifierName("suitesetup");

        private readonly FitSocket socket;
        private readonly CellProcessor service;
        private readonly ProgressReporter reporter;

	    private bool IMaybeProcessingSuiteSetup;

        public SocketServer(FitSocket socket, CellProcessor service, ProgressReporter reporter, bool suiteSetUpIsAnonymous) {
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

        private void ProcessTestDocument(string document, Action<string, TestCounts> writer) {
            var storyTestWriter = new Writer(service, writer);
			try
			{
                Tree<Cell> result = service.Compose(new StoryTestString(document));
                var parse = result != null ? (Parse)result.Value : null;
			    var storyTest = new StoryTest(parse, storyTestWriter);
			    reporter.WriteLine(parse.Leader);
			    if (suiteSetupIdentifier.IsStartOf(parse.Leader) || IMaybeProcessingSuiteSetup)
                    storyTest.ExecuteOnConfiguration(service.Configuration);
                else
				    storyTest.Execute(service.Configuration);
			}
			catch (Exception e)
			{
			    var testStatus = new TestStatus();
			    var parse = new CellBase(parseError, "div");
                parse.SetAttribute(CellAttribute.Body, parseError );
			    testStatus.MarkException(parse, e);
                storyTestWriter.WriteTable(new CellTree(parse));
			    storyTestWriter.WriteTest(new CellTree().AddBranchValue(parse), testStatus.Counts); 
			}
		}

        class Writer: StoryTestWriter {
            public Writer(CellProcessor processor, Action<string, TestCounts> writer) {
                this.processor = processor;
                this.writer = writer;
            }

            public void WriteTable(Tree<Cell> table) {
                var testResult = processor.ParseTree<Cell, StoryTableString>(table).ToString();
                if (testResult.Length > 0) {
                    writer(testResult, null); //todo: clean up null
                    writesTables = true;
                }
            }

            public void WriteTest(Tree<Cell> test, TestCounts counts) {
                var testResult = writesTables ? string.Empty : processor.ParseTree<Cell, StoryTestString>(test).ToString();
                writer(testResult, counts);
            }

            readonly CellProcessor processor;
            readonly Action<string, TestCounts> writer;
            bool writesTables;
        }
    }
}
