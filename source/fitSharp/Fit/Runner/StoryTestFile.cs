// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.IO;
using System;
using fitSharp.Parser;
using Path = System.IO.Path;

namespace fitSharp.Fit.Runner {

    public interface StoryTestPage {
        StoryPageName Name { get; }
        string Content { get; }

        void WriteTest(PageResult result);
        void WriteNonTest();
        string TestContent { get; }
    }

    public interface StoryTestPageExecutor {
        void Do(StoryTestPage page);
        void DoNoTest();
        bool SuiteIsAbandoned { get; }
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
            ".fit_extension {border: solid 1px grey; background-color: white;}" + Environment.NewLine +
            ".fit_table {border: solid 1px grey; border-collapse: collapse; margin: 2px 0px;}" + Environment.NewLine +
            "table.fit_table tr td {border: solid 1px grey; padding: 2px 2px 2px 2px;}" + Environment.NewLine +
            ".fit_interpreter {font-style: italic; color: #808020;}" + Environment.NewLine +
            ".fit_keyword {color: #1010A0;}" + Environment.NewLine +
            ".fit_member {color: #208080;}" + Environment.NewLine +
            ".fit_SUT {color: #808020;}" + Environment.NewLine;

        private const string styleSheetLink = "<link href=\"fit.css\" type=\"text/css\" rel=\"stylesheet\">";

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

            var outputFile = Path.Combine(myFolder.OutputPath, myPath.OutputFileName);
            myFolderModel.MakeFile(outputFile, ResultComment(result.TestCounts) + AddStyleSheetLink( result.Content));
            myFolder.ListFile(outputFile, result.TestCounts, result.ElapsedTime);
        }

        static string AddStyleSheetLink(string input) {
            var scanner = new Scanner(input);
            while (true) {
                scanner.FindTokenPair("<link", ">");
                if (scanner.Body.IsEmpty) break;
                if (scanner.Body.Contains("fit.css")) return input;
            }
            return styleSheetLink + input;
        }

        static string ResultComment(TestCounts counts) {
            return "<!--"
                   + Clock.Instance.Now.ToString("yyyy-MM-dd HH:mm:ss,")
                   + counts.GetCount(TestStatus.Right) + ","
                   + counts.GetCount(TestStatus.Wrong) + ","
                   + counts.GetCount(TestStatus.Ignore) + ","
                   + counts.GetCount(TestStatus.Exception) + "-->"
                   + Environment.NewLine;
        }

        private bool HasTestName {
            get {
                return !(myPath.IsSetUp || myPath.IsTearDown || myPath.IsSuiteSetUp || myPath.IsSuiteTearDown);
            }
        }

        public string Content {
            get {
                if (myContent == null) myContent = myFolderModel.GetPageContent(myPath.Name);
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
            if (myFolderModel.GetPageContent(filePath) == null) {
                myFolderModel.MakeFile(filePath, StyleContent);
            }
        }
    }
}
