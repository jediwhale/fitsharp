// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.IO;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.IO;

namespace fitSharp.Fit.Runner {

    public interface StoryTestPage {
        StoryPageName Name { get; }
        string Content { get; }
        //todo: too many args
        void ExecuteStoryPage(Action<StoryPageName, StoryTestString, Action<StoryTestString, TestCounts>, Action> executor, ResultWriter resultWriter, Action<TestCounts> handler);
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

        public StoryPageName Name { get { return myPath; } }

        public void ExecuteStoryPage(Action<StoryPageName, StoryTestString, Action<StoryTestString, TestCounts>, Action> executor, ResultWriter resultWriter, Action<TestCounts> handler) {
            elapsedTime = new ElapsedTime();
            this.resultWriter = resultWriter;
            this.handler = handler;
            if (HasTestName) {
                executor(myPath, DecoratedContent, WriteFile, HandleNoTest);
                return;
            }
            if (myPath.IsSuiteSetUp || myPath.IsSuiteTearDown) {
                executor(myPath, PlainContent, WriteFile, HandleNoTest);
                return;
            }
            HandleNoTest();
        }

        public void HandleNoTest() {
            myFolderModel.CopyFile(myPath.Name, Path.Combine(myFolder.OutputPath, myPath.CopyFileName));
            handler(new TestCounts());
        }

        private void WriteFile(StoryTestString testResult, TestCounts counts) {
            WriteResult(testResult, counts);
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

        private void WriteResult(StoryTestString testResult, TestCounts counts) {
            string outputFile = Path.Combine(myFolder.OutputPath, myPath.OutputFileName);
            var output = new StringWriter();
            output.Write(testResult);
            output.Close();
            myFolderModel.MakeFile(outputFile, output.ToString());
            myFolder.ListFile(outputFile, counts, elapsedTime);
        }
    }
}
