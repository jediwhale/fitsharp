// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.IO;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Machine.Extension;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Runner {

    public interface StoryTestPage {
        string Name { get; }
        void ExecuteStoryPage(Action<StoryTestString, Action<StoryTestString, TestCounts>, Action> executor, ResultWriter resultWriter, Action<TestCounts> handler);
    }

    public class StoryTestFile: StoryTestPage {
        private readonly StoryFileName myPath;
        private readonly StoryTestFolder myFolder;
        private readonly FolderModel myFolderModel;
        private ElapsedTime elapsedTime;
        private ResultWriter resultWriter;
        private Action<TestCounts> handler;
        private string myContent;

        public StoryTestFile(string thePath, StoryTestFolder theFolder, FolderModel theFolderModel) {
            myPath = new StoryFileName(thePath);
            myFolder = theFolder;
            myFolderModel = theFolderModel;
        }

        public string Name { get { return Path.GetFileName(myPath.Name); }}

        public void ExecuteStoryPage(Action<StoryTestString, Action<StoryTestString, TestCounts>, Action> executor, ResultWriter resultWriter, Action<TestCounts> handler) {
            elapsedTime = new ElapsedTime();
            this.resultWriter = resultWriter;
            this.handler = handler;
            if (HasTestName) {
                executor(DecoratedContent, WriteFile, HandleNoTest);
                return;
            }
            if (myPath.IsSuiteSetUp || myPath.IsSuiteTearDown) {
                executor(PlainContent, WriteFile, HandleNoTest);
                return;
            }
            HandleNoTest();
        }

        public void HandleNoTest() {
            myFolderModel.CopyFile(myPath.Name, OutputPath);
            handler(new TestCounts());
        }

        private void WriteFile(StoryTestString testResult, TestCounts counts) {
            WriteResult(testResult, counts, elapsedTime);
            resultWriter.WritePageResult(new PageResult(myPath.Name, testResult.ToString(), counts));
            handler(counts);
        }

        private bool HasTestName {
            get {
                return !(myPath.IsSetUp || myPath.IsTearDown || myPath.IsSuiteSetUp || myPath.IsSuiteTearDown);
            }
        }

        public string Content {
            get {
                if (myContent == null) myContent = myFolderModel.FileContent(myPath.Name);
                return myContent;
            }
        }

        private StoryTestString DecoratedContent {
            get {
                return new StoryTestString(myFolder.Decoration.IsEmpty
                                               ? Content
                                               : myFolder.Decoration.Decorate(Content));
            }
        }

        private StoryTestString PlainContent {
            get {
                return new StoryTestString(Content);
            }
        }

        private void WriteResult(StoryTestString testResult, TestCounts counts, ElapsedTime elapsedTime) {
            string outputFile = OutputPath;
            var output = new StringWriter();
            output.Write(testResult);
            output.Close();
            myFolderModel.MakeFile(outputFile, output.ToString());
            myFolder.ListFile(outputFile, counts, elapsedTime);
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