// Copyright © 2020 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using Path = System.IO.Path;

namespace fitSharp.Fit.Runner {
    public interface StoryTestSuite {
        IEnumerable<StoryTestPage> AllPages { get; }
        string FullName { get; }
    }

    public class StoryTestFolder: StoryTestSuite {
 
        public StoryTestFolder(Memory memory, FolderModel theFolderModel, Filters filters)
            : this(memory.GetItem<Settings>().InputFolder,
                   memory.GetItem<Settings>().OutputFolder, theFolderModel, null, filters) {
        }

        StoryTestFolder(string theInputPath, string theOutputPath, FolderModel theFolderModel, StoryTestFolder theParentFolder, Filters filters) {
            FullName = theInputPath;
            OutputPath = theOutputPath;
            myFolderModel = theFolderModel;
            myParent = theParentFolder;
            this.filters = filters;
        }

        public string FullName { get; }
        public string OutputPath { get; }

        public IEnumerable<StoryTestPage> AllPages {
            get {
                var count = 0;
                foreach (var page in Pages.Concat(Suites.SelectMany(s => s.AllPages))) {
                    if (count == 0) {
                        var suiteSetUp = SuiteSetUp;
                        if (suiteSetUp != null) yield return suiteSetUp;
                    }
                    yield return page;
                    count++;
                }
                if (count > 0) {
                    var suiteTearDown = SuiteTearDown;
                    if (suiteTearDown != null) yield return suiteTearDown;
                }
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

        IEnumerable<StoryTestPage> Pages =>
            myFolderModel
                .GetFiles(FullName)
                .Select(filePath => new StoryTestFile(filePath, this, myFolderModel))
                .Where(file => filters.Matches(file))
                .OrderBy(file => file.Name.Name);

        StoryTestPage SuiteSetUp => FindFile(name => name.IsSuiteSetUp);

        StoryTestPage SuiteTearDown => FindFile(name => name.IsSuiteTearDown);

        IEnumerable<StoryTestSuite> Suites =>
            myFolderModel
                .GetFolders(FullName)
                .Select(CreateChildSuite)
                .Where(suite => filters.Matches(suite))
                .OrderBy(suite => suite.FullName);

        StoryTestSuite CreateChildSuite(string inputFolder) {
            var relativeFolder = inputFolder.Substring(FullName.Length + 1);
            return new StoryTestFolder(inputFolder, Path.Combine(OutputPath, relativeFolder), myFolderModel, this, filters);
        }

        StoryTestFile FindFile(Func<StoryFileName, bool> filter) {
            return myFolderModel.GetFiles(FullName)
                .Where(filePath => filter(new StoryFileName(filePath)))
                .Select(filePath => new StoryTestFile(filePath, this, myFolderModel))
                .FirstOrDefault();
        }

        string GetSetUp() {
            var file = FindFile(name => name.IsSetUp);
            return file == null ? string.Empty : file.Content;
        }

        string GetTearDown() {
            var file = FindFile(name => name.IsTearDown);
            return file == null ? string.Empty : file.Content;
        }

        readonly StoryTestFolder myParent;
        readonly Filters filters;
        readonly FolderModel myFolderModel;

        TestPageDecoration myDecoration;
    }
}