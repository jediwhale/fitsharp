// Copyright � 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fitSharp.Fit.Model;
using fitSharp.Fit.Application;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fitSharp.Fit.Runner {
    public interface StoryTestSuite {
        string Name { get; }
        void Select(string theTestPage);
        IEnumerable<StoryTestPage> Pages { get; }
        StoryTestPage SuiteSetUp { get; }
        StoryTestPage SuiteTearDown { get; }
        IEnumerable<StoryTestSuite> Suites { get; }
        void Finish();
    }

    public class StoryTestFolder: StoryTestSuite {
        private readonly Memory memory;
        private readonly List<StoryTestPageFilter> pageFilters = new List<StoryTestPageFilter>();
 
        public StoryTestFolder(Memory memory, FolderModel theFolderModel)
            : this(memory, 
                   memory.GetItem<Settings>().InputFolder,
                   memory.GetItem<Settings>().OutputFolder, null, theFolderModel, null) {
            myReport = new Report(OutputPath);
        }

        public StoryTestFolder(Memory memory, string theInputPath, string theOutputPath, string theSelection, FolderModel theFolderModel, StoryTestFolder theParentFolder) {
            this.memory = memory;
            Name = theInputPath;
            OutputPath = theOutputPath;
            myFolderModel = theFolderModel;
            myParent = theParentFolder;
            mySelection = theSelection;
        }

        public string Name { get; private set; }

        public void Select(string theTestPage) {
            mySelection = Path.Combine(Name, theTestPage);
        }

        public IEnumerable<StoryTestPage> Pages {
            get {
                foreach (string filePath in myFolderModel.GetFiles(Name)) {
                    string fileName = Path.GetFileName(filePath);
                    if (new StoryFileName(fileName).IsSuiteSetUp) continue;
                    if (new StoryFileName(fileName).IsSuiteTearDown) continue;
                    if (memory.GetItem<FileExclusions>().IsExcluded(fileName)) continue;
                    if (mySelection != null && !filePath.EndsWith(mySelection)) continue;
                    var file = new StoryTestFile(filePath, this, myFolderModel);
                    if (!SatisfiesFilters(file)) continue;
                    yield return file;
                }
            }
        }

        public void AddPageFilter(StoryTestPageFilter filter) {
            pageFilters.Add(filter);
        }

        private bool SatisfiesFilters(StoryTestPage page) {
            return pageFilters.All(filter => filter.Matches(page));
        }

        public StoryTestPage SuiteSetUp {
            get {
                return FindFile(name => name.IsSuiteSetUp);
            }
        }

        public StoryTestPage SuiteTearDown {
            get {
                return FindFile(name => name.IsSuiteTearDown);
            }
        }

        public IEnumerable<StoryTestSuite> Suites {
            get {
                foreach (string inputFolder in myFolderModel.GetFolders(Name)) {
                    string relativeFolder = inputFolder.Substring(Name.Length + 1);
                    if (memory.GetItem<FileExclusions>().IsExcluded(relativeFolder)) continue;
                    if (mySelection != null && !mySelection.StartsWith(inputFolder)) continue;
                    yield return CreateChildSuite(inputFolder, relativeFolder);
                }
            }
        }

        private StoryTestSuite CreateChildSuite(string inputFolder, string relativeFolderPath) {
            var folder = new StoryTestFolder(memory, inputFolder, Path.Combine(OutputPath, relativeFolderPath), mySelection, myFolderModel, this);

            foreach (var pageFilter in pageFilters)
                folder.AddPageFilter(pageFilter);

            return folder;
        }

        public void Finish() {
            if (myReport != null) {
                myReport.Finish();
                myFolderModel.MakeFile(Path.Combine(OutputPath, ourReportName), myReport.Content);
            }
        }

        public void ListFile(string theFileName, TestCounts counts, ElapsedTime elapsedTime) {
            if (myReport != null) {
                myReport.ListFile(theFileName, counts, elapsedTime);
            }
            else {
                myParent.ListFile(theFileName, counts, elapsedTime);
            }
        }

        public TestPageDecoration Decoration {
            get {
                if (myDecoration == null) {
                    myDecoration = myParent == null
                                       ? new TestPageDecoration(GetSetUp(), GetTearDown())
                                       : myParent.Decoration.MakeChild(GetSetUp(), GetTearDown());
                }
                return myDecoration;
            }
        }

        public string OutputPath { get; private set; }

        private delegate bool FileFilter(StoryFileName name);

        private StoryTestFile FindFile(FileFilter filter) {
            foreach (string filePath in myFolderModel.GetFiles(Name)) {
                if (filter(new StoryFileName(filePath))) {
                    return new StoryTestFile(/*configuration,*/ filePath, this, myFolderModel);
                }
            }
            return null;
        }

        private string GetSetUp() {
            StoryTestFile file = FindFile(name => name.IsSetUp);
            return file == null ? string.Empty : file.Content;
        }
	    
        private string GetTearDown() {
            StoryTestFile file = FindFile(name => name.IsTearDown);
            return file == null ? string.Empty : file.Content;
        }

        private const string ourReportName = "reportIndex.html";

        private string mySelection;
        private readonly StoryTestFolder myParent;
        private readonly FolderModel myFolderModel;
        private TestPageDecoration myDecoration;
        private readonly Report myReport;

        private class Report {
	        
            public Report(string theReportPath) {
                myReportPath = theReportPath;
                DateTime now = Clock.Instance.Now;
                myReport = new StringWriter();
                myReport.WriteLine("<html><head><link href=\"fit.css\" type=\"text/css\" rel=\"stylesheet\">");
                myReport.WriteLine("<title>Folder Runner Report</title></head>");
                myReport.WriteLine("<body><h1>Folder Runner Report {0:yyyy-MM-dd HH:mm:ss}</h1>", now);
            }
	        
            public string Content {get { return myReport.ToString(); }}
	        
            public void ListFile(string thePath, TestCounts counts, ElapsedTime elapsedTime) {
                myReport.WriteLine("<br /><a href=\"{0}\">{0}</a> <span class=\"{2}\">{1}</span> in {3}",
                                   thePath.Substring(myReportPath.Length + 1).Replace('\\', '/'),
                                   counts.Description,
                                   counts.Style,
                                   elapsedTime);
            }
	        
            public void Finish() {
                myReport.WriteLine("</body></html>");
            }
	        
            private readonly StringWriter myReport;
            private readonly string myReportPath;
        }
    }
}