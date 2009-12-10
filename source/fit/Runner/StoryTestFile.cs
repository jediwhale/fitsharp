// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.IO;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Runner {
    public class StoryTestFile: StoryTestPage {
        private readonly Configuration configuration;
        private readonly StoryFileName myPath;
        private readonly StoryTestFolder myFolder;
        private readonly FolderModel myFolderModel;
        private ElapsedTime elapsedTime;
        private ResultWriter resultWriter;
        private TestCountsHandler handler;
        private string myContent;
        private Parse myTables;

        public StoryTestFile(Configuration configuration, string thePath, StoryTestFolder theFolder, FolderModel theFolderModel) {
            this.configuration = configuration;
            myPath = new StoryFileName(thePath);
            myFolder = theFolder;
            myFolderModel = theFolderModel;
        }

        public string Name { get { return Path.GetFileName(myPath.Name); }}

        public void ExecuteStoryPage(ResultWriter resultWriter, TestCountsHandler handler) {
            elapsedTime = new ElapsedTime();
            this.resultWriter = resultWriter;
            this.handler = handler;
            if (IsTest) {
                new StoryTest(Tables, WriteFile).Execute();
                return;
            }
            if (myPath.IsSuiteSetUp || myPath.IsSuiteTearDown) {
                new StoryTest(RawTables, WriteFile).Execute();
                return;
            }
            CopyFile();
            handler(new TestCounts());
        }

        private void WriteFile(Tree<Cell> theTables, TestCounts counts) {
            WriteResult(theTables, counts, elapsedTime);
            resultWriter.WritePageResult(new PageResult(myPath.Name, theTables.ToString(), counts)); // todo: use the processor parse result not tostring
            handler(counts);
        }

        public bool IsTest {
            get {
                if (myPath.IsSetUp || myPath.IsTearDown || myPath.IsSuiteSetUp || myPath.IsSuiteTearDown) return false;
                return (RawTables != null);
            }
        }

        public string Content {
            get {
                if (myContent == null) myContent = myFolderModel.FileContent(myPath.Name);
                return myContent;
            }
        }

        private void CopyFile() {
            myFolderModel.CopyFile(myPath.Name, OutputPath);
        }

        private void WriteResult(Tree<Cell> theTables, TestCounts counts, ElapsedTime elapsedTime) {
            string outputFile = OutputPath;
            var output = new StringWriter();
            output.Write(configuration.GetItem<Service.Service>().ParseTree(typeof(StoryTestString), theTables).ValueString);
            output.Close();
            myFolderModel.MakeFile(outputFile, output.ToString());
            myFolder.ListFile(outputFile, counts, elapsedTime);
        }

        private Parse Tables {
            get {
                return myFolder.Decoration.IsEmpty
                           ? RawTables
                           : Parse(myFolder.Decoration.Decorate(Content));
            }
        }

        private Parse RawTables {
            get {
                if (myTables == null) {
                    FitVersionFixture.Reset();
                    myTables = Parse(Content);
                }
                return myTables;
            }
        }

        private Parse Parse(string content) {
            Tree<Cell> result = configuration.GetItem<Service.Service>().Compose(new StoryTestString(content));
            return result != null ? (Parse)result.Value : null;
        }

        private string OutputPath {
            get {
                return Path.Combine(myFolder.OutputPath, Path.GetFileName(myPath.Name));
            }
        }
    }

    public class StoryFileName {
        public StoryFileName(string theName) {
            myName = theName;
        }

        public string Name { get { return myName; }}

        public bool IsSetUp {
            get {
                string name = Path.GetFileName(myName);
                return ourSetupIdentifier1.Equals(name) || ourSetupIdentifier2.Equals(name);
            }
        }

        public bool IsSuiteSetUp {
            get {
                string name = Path.GetFileName(myName);
                return ourSuiteSetupIdentifier1.Equals(name) || ourSuiteSetupIdentifier2.Equals(name);
            }
        }

        public bool IsSuiteTearDown {
            get {
                string name = Path.GetFileName(myName);
                return ourSuiteTearDownIdentifier1.Equals(name) || ourSuiteTearDownIdentifier2.Equals(name);
            }
        }

        public bool IsTearDown {
            get {
                string name = Path.GetFileName(myName);
                return ourTeardownIdentifier1.Equals(name) || ourTeardownIdentifier2.Equals(name);
            }
        }

        private static readonly IdentifierName ourSetupIdentifier1 = new IdentifierName("setup.html");
        private static readonly IdentifierName ourSetupIdentifier2 = new IdentifierName("setup.htm");
        private static readonly IdentifierName ourTeardownIdentifier1 = new IdentifierName("teardown.html");
        private static readonly IdentifierName ourTeardownIdentifier2 = new IdentifierName("teardown.htm");
        private static readonly IdentifierName ourSuiteSetupIdentifier1 = new IdentifierName("suitesetup.html");
        private static readonly IdentifierName ourSuiteSetupIdentifier2 = new IdentifierName("suitesetup.htm");
        private static readonly IdentifierName ourSuiteTearDownIdentifier1 = new IdentifierName("suiteteardown.html");
        private static readonly IdentifierName ourSuiteTearDownIdentifier2 = new IdentifierName("suiteteardown.htm");
        private readonly string myName;
    }
}