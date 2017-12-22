// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

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

        public StoryTestFolder(string theInputPath, string theOutputPath, FolderModel theFolderModel, StoryTestFolder theParentFolder, Filters filters) {
            FullName = theInputPath;
            OutputPath = theOutputPath;
            myFolderModel = theFolderModel;
            myParent = theParentFolder;
            this.filters = filters;
        }

        public string FullName { get; private set; }

        public IEnumerable<StoryTestPage> AllPages {
            get {
                var suiteSetUp = SuiteSetUp;
                if (suiteSetUp != null) yield return suiteSetUp;
                foreach (var page in Pages) yield return page;
                foreach (var page in Suites.SelectMany(s => s.AllPages)) yield return page;
                var suiteTearDown = SuiteTearDown;
                if (suiteTearDown != null) yield return suiteTearDown;
            }
        }

        IEnumerable<StoryTestPage> Pages {
            get {
                return myFolderModel
                    .GetFiles(FullName)
                    .Select(filePath => new StoryTestFile(filePath, this, myFolderModel))
                    .Where(file => filters.Matches(file));
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
                    .Where(suite => filters.Matches(suite));
            }
        }

        private StoryTestSuite CreateChildSuite(string inputFolder) {
            string relativeFolder = inputFolder.Substring(FullName.Length + 1);
            return new StoryTestFolder(inputFolder, Path.Combine(OutputPath, relativeFolder), myFolderModel, this, filters);
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

        public string OutputPath { get; }

        private delegate bool FileFilter(StoryFileName name);

        private StoryTestFile FindFile(FileFilter filter) {
            foreach (string filePath in myFolderModel.GetFiles(FullName)) {
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

        private readonly StoryTestFolder myParent;
        readonly Filters filters;
        private readonly FolderModel myFolderModel;
        private TestPageDecoration myDecoration;
    }
}