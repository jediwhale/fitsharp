// Copyright � 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        public StoryTestFolder(string theInputPath, string theOutputPath, FolderModel theFolderModel, StoryTestFolder theParentFolder, Filters filters) {
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

        IEnumerable<StoryTestPage> Pages {
            get {
                return myFolderModel
                    .GetFiles(FullName)
                    .Select(filePath => new StoryTestFile(filePath, this, myFolderModel))
                    .Where(file => filters.Matches(file))
                    .OrderBy(file => file.Name.Name);
            }
        }

        StoryTestPage SuiteSetUp {
            get {
                return FindFile(name => name.IsSuiteSetUp);
            }
        }

        StoryTestPage SuiteTearDown {
            get {
                return FindFile(name => name.IsSuiteTearDown);
            }
        }

        IEnumerable<StoryTestSuite> Suites {
            get {
                return myFolderModel
                    .GetFolders(FullName)
                    .Select(CreateChildSuite)
                    .Where(suite => filters.Matches(suite))
                    .OrderBy(suite => suite.FullName);
            }
        }

        StoryTestSuite CreateChildSuite(string inputFolder) {
            var relativeFolder = inputFolder.Substring(FullName.Length + 1);
            return new StoryTestFolder(inputFolder, Path.Combine(OutputPath, relativeFolder), myFolderModel, this, filters);
        }

        StoryTestFile FindFile(Func<StoryFileName, bool> filter) {
            foreach (var filePath in myFolderModel.GetFiles(FullName)) {
                if (filter(new StoryFileName(filePath))) {
                    return new StoryTestFile(filePath, this, myFolderModel);
                }
            }
            return null;
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