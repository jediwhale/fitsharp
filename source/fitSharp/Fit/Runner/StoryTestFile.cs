// Copyright � 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.IO;
using fitSharp.Fit.Model;
using fitSharp.IO;
using System;

namespace fitSharp.Fit.Runner {

    public interface StoryTestPage {
        StoryPageName Name { get; }
        string Content { get; }

        void WriteTest(PageResult result);
        void WriteNonTest();
        string TestContent { get; }
        //void ExecuteStoryPage(StoryTestPageExecutor executor);
    }

    public interface StoryTestPageExecutor {
        //todo: too many args
        //void Do(StoryPageName pageName, StoryTestString pageContent, Action<PageResult> handleResult, Action handleNoTest);
        void Do(StoryTestPage page);
        void DoNoTest();
    }

    public class StoryTestFile: StoryTestPage {
        private readonly StoryFileName myPath;
        private readonly StoryTestFolder myFolder;
        private readonly FolderModel myFolderModel;
        private string myContent;

        private const string StyleName = "fit.css";
        private static readonly string StyleContent =
            ".pass {background-color: #AAFFAA;}" + Environment.NewLine +
            ".fail {background-color: #FFAAAA;}" + Environment.NewLine +
            ".error {background-color: #FFFFAA;}" + Environment.NewLine +
            ".ignore {background-color: #CCCCCC;}" + Environment.NewLine +
            ".fit_stacktrace {font-size: 0.7em;}" + Environment.NewLine +
            ".fit_label {font-style: italic; color: #C08080;}" + Environment.NewLine +
            ".fit_grey {color: #808080;}" + Environment.NewLine +
            ".fit_extension {border: solid 1px grey;}" + Environment.NewLine +
            ".fit_table {border: solid 1px grey; border-collapse: collapse; margin: 2px 0px;}" + Environment.NewLine +
            "table.fit_table tr td {border: solid 1px grey; padding: 2px 2px 2px 2px;}" + Environment.NewLine;

        public StoryTestFile(string thePath, StoryTestFolder theFolder, FolderModel theFolderModel) {
            myPath = new StoryFileName(thePath);
            myFolder = theFolder;
            myFolderModel = theFolderModel;
        }

        public StoryPageName Name { get { return myPath; } }

        public string TestContent {
            get {
                if (HasTestName) return DecoratedContent.ToString();
                if (myPath.IsSuiteSetUp || myPath.IsSuiteTearDown) return PlainContent.ToString();
                return string.Empty;
            }
        }

        public void WriteNonTest() {
            // Setup/teardown contents are already inlined into every executed page
            if (myPath.IsSetUp || myPath.IsTearDown)
                return;

            myFolderModel.CopyFile(myPath.Name, Path.Combine(myFolder.OutputPath, myPath.CopyFileName));
        }

        public void WriteTest(PageResult result) {
            MakeStylesheet();

            string outputFile = Path.Combine(myFolder.OutputPath, myPath.OutputFileName);
            myFolderModel.MakeFile(outputFile, result.Content);
            myFolder.ListFile(outputFile, result.TestCounts, result.ElapsedTime);
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

        private void MakeStylesheet() {
            string filePath = Path.Combine(myFolder.OutputPath, StyleName);
            if (myFolderModel.FileContent(filePath) == null) {
                myFolderModel.MakeFile(filePath, StyleContent);
            }
        }
    }
}
