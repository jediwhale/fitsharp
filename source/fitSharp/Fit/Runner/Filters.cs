// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.IO;
using fitSharp.Fit.Application;

namespace fitSharp.Fit.Runner {
    public class Filters {

        public Filters(string tagList, FileExclusions fileExclusions, string selection) {
            this.tagList = tagList;
            this.fileExclusions = fileExclusions;
            this.selection = selection;
        }

        public bool Matches(StoryTestPage page) {
            if (page.Name.IsSuiteSetUp) return false;
            if (page.Name.IsSuiteTearDown) return false;
            if (!string.IsNullOrEmpty(tagList) && !new TagFilter(tagList).Matches(page)) return false;
            if (!string.IsNullOrEmpty(selection) && !page.Name.Name.EndsWith(selection)) return false;
            return !fileExclusions.IsExcluded(Path.GetFileName(page.Name.Name));
        }

        public bool Matches(StoryTestSuite suite) {
            return !fileExclusions.IsExcluded(Path.GetFileName(suite.FullName));
        }

        readonly string tagList;
        readonly FileExclusions fileExclusions;
        readonly string selection;
    }
}
