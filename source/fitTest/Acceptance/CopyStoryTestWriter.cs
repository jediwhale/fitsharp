// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Model;

namespace fit.Test.Acceptance {
    public class CopyStoryTestWriter: StoryTestWriter {
        public void WriteTable(Tree<Cell> table) {}

        public Parse ResultTables {get; private set; }

        public void WriteTest(Tree<Cell> test, TestCounts counts) {
            var tables = (Parse) test.Branches[0];
            for (var table = tables; table != null; table = table.More) {
                CopyTable(table);
            }
        }

        void CopyTable(Parse table) {
            var newTable = table.Copy();
            if (ResultTables == null) {
                ResultTables = newTable;
            }
            else {
                ResultTables.Last.More = newTable;
            }
        }
    }
}
