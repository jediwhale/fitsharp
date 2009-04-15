// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using System.IO;
using fitSharp.Fit.Model;
using fitSharp.Fit.Application;
using fitSharp.Machine.Application;

namespace fit.Runner {
    public class StoryTestFolder: StoryTestSuite {
        public StoryTestFolder(FolderModel theFolderModel)
            : this(
                Context.Configuration.GetItem<Settings>().InputFolder,
                Context.Configuration.GetItem<Settings>().OutputFolder, null, theFolderModel, null) {
            myReport = new Report(OutputPath);
        }

        public StoryTestFolder(string theInputPath, string theOutputPath, string theSelection, FolderModel theFolderModel, StoryTestFolder theParentFolder) {
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
                    if (Context.Configuration.GetItem<FileExclusions>().IsExcluded(fileName)) continue;
                    if (mySelection != null && !filePath.EndsWith(mySelection)) continue;
                    var file = new StoryTestFile(filePath, this, myFolderModel);
                    yield return file;
                }
            }
        }

        public StoryTestPage SuiteSetUp {
            get {
                return FindFile(name => name.IsSuiteSetUp);
            }
        }

        public IEnumerable<StoryTestSuite> Suites {
            get {
                foreach (string inputFolder in myFolderModel.GetFolders(Name)) {
                    string relativeFolder = inputFolder.Substring(Name.Length + 1);
                    if (Context.Configuration.GetItem<FileExclusions>().IsExcluded(relativeFolder)) continue;
                    if (mySelection != null && !mySelection.StartsWith(inputFolder)) continue;
                    yield return new StoryTestFolder(inputFolder, Path.Combine(OutputPath, relativeFolder), mySelection, myFolderModel, this);
                }
            }
        }

        public void Finish() {
            if (myReport != null) {
                myReport.Finish();
                myFolderModel.MakeFile(Path.Combine(OutputPath, ourReportName), myReport.Content);
            }
            if (IHaveFiles) {
                MakeStylesheet();
            }
        }

        public void ListFile(string theFileName, TestStatus status, TimeSpan theElapsedTime) {
            IHaveFiles = true;
            if (myReport != null) {
                myReport.ListFile(theFileName, status, theElapsedTime);
            }
            else {
                myParent.ListFile(theFileName, status, theElapsedTime);
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
                    return new StoryTestFile(filePath, this, myFolderModel);
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

        private void MakeStylesheet() {
            string filePath = Path.Combine(OutputPath, ourStyleName);
            if (myFolderModel.FileContent(filePath) == null) {
                myFolderModel.MakeFile(filePath, ourStyleContent);
            }
        }

        private const string ourReportName = "reportIndex.html";
        private const string ourStyleName = "fit.css";
        private readonly string ourStyleContent =
            ".pass {background-color: #AAFFAA;}" + Environment.NewLine +
            ".fail {background-color: #FFAAAA;}" + Environment.NewLine +
            ".error {background-color: #FFFFAA;}" + Environment.NewLine +
            ".ignore {background-color: #CCCCCC;}" + Environment.NewLine +
            ".fit_stacktrace {font-size: 0.7em;}" + Environment.NewLine +
            ".fit_label {font-style: italic; color: #C08080;}" + Environment.NewLine +
            ".fit_grey {color: #808080;}" + Environment.NewLine;

        private string mySelection;
        private readonly StoryTestFolder myParent;
        private readonly FolderModel myFolderModel;
        private TestPageDecoration myDecoration;
        private readonly Report myReport;
        private bool IHaveFiles;

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
	        
            public void ListFile(string thePath, TestStatus status, TimeSpan theElapsedTime) {
                myReport.WriteLine("<br /><a href=\"{0}\">{0}</a> <span class=\"{2}\">{1}</span> in {3}",
                                   thePath.Substring(myReportPath.Length + 1).Replace('\\', '/'),
                                   status.CountDescription,
                                   status.Style,
                                   theElapsedTime);
            }
	        
            public void Finish() {
                myReport.WriteLine("</body></html>");
            }
	        
            private readonly StringWriter myReport;
            private readonly string myReportPath;
        }
    }
}