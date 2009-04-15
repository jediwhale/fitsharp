// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.IO;
using fitSharp.Fit.Model;
using fitnesse.fitserver;
using fitSharp.Machine.Model;

namespace fit.Runner {
    public class StoryTestFile: StoryTestPage {
        public StoryTestFile(string thePath, StoryTestFolder theFolder, FolderModel theFolderModel) {
            myPath = new StoryFileName(thePath);
            myFolder = theFolder;
            myFolderModel = theFolderModel;
        }

        public string Name { get { return Path.GetFileName(myPath.Name); }}

        public StoryCommand MakeStoryCommand(ResultWriter writer) {
            if (IsTest) {
                return new StoryTest(Tables, new FileListener(this, writer));
            }
            if (myPath.IsSuiteSetUp) {
                return new StoryTest(RawTables, new FileListener(this, writer));
            }
            return new Copy(this);
        }

        public bool IsTest {
            get {
                if (myPath.IsSetUp || myPath.IsTearDown) return false;
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

        private void WriteResult(Parse theTables, TestStatus status, TimeSpan theElapsedTime) {
            string outputFile = OutputPath;
            var output = new StringWriter();
            theTables.Print(output);
            output.Close();
            myFolderModel.MakeFile(outputFile, output.ToString());
            myFolder.ListFile(outputFile, status, theElapsedTime);
        }

        private Parse Tables {
            get {
                return myFolder.Decoration.IsEmpty
                           ? RawTables
                           : HtmlParser.Instance.Parse(myFolder.Decoration.Decorate(Content));
            }
        }

        private Parse RawTables {
            get {
                if (myTables == null) {
                    FitVersionFixture.Reset();
                    myTables = HtmlParser.Instance.Parse(Content);
                }
                return myTables;
            }
        }

        private string OutputPath {
            get {
                return Path.Combine(myFolder.OutputPath, Path.GetFileName(myPath.Name));
            }
        }
        private readonly StoryFileName myPath;
        private readonly StoryTestFolder myFolder;
        private readonly FolderModel myFolderModel;
        private string myContent;
        private Parse myTables;

        private class Copy: StoryCommand {
            public Copy(StoryTestFile theFile) {
                myFile = theFile;
            }
            public void Execute() {
                myFile.CopyFile();
            }
            public TestStatus TestStatus {get { return new TestStatus(); }}

            private readonly StoryTestFile myFile;
        }

        private class FileListener: FixtureListener {
            public FileListener(StoryTestFile theFile, ResultWriter resultWriter) {
                myFile = theFile;
                myStartTime = Clock.Instance.UtcNow;
                this.resultWriter = resultWriter;
                pageResult = new PageResult(theFile.myPath.Name);
            }

            public void TableFinished(Parse finishedTable) {}

            public void TablesFinished(Parse theTables, TestStatus status) {
                myFile.WriteResult(theTables, status, Clock.Instance.UtcNow - myStartTime);

                pageResult.Append(theTables.ToString());
                pageResult.TestStatus = status;
                resultWriter.WritePageResult(pageResult);
            }

            private readonly StoryTestFile myFile;
            private readonly DateTime myStartTime;
            private readonly ResultWriter resultWriter;
            private readonly PageResult pageResult;
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
        private readonly string myName;
    }
}