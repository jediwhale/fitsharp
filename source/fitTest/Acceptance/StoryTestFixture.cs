// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fit.Test.Acceptance {
    public class StoryTestFixture: DoFixture {
        private Parse resultTables;

        public Parse TestResult(Parse theTest) {
            resultTables = null;
            var story = new StoryTest(theTest, SaveTestResult);
            story.Execute(Processor.Configuration);
            return resultTables;
        }

        private void SaveTestResult(Tree<Cell> theTables, TestCounts counts) {
            var tables = (Parse) theTables.Value;
            for (Parse table = tables; table != null; table = table.More) {
                Parse newTable = table.Copy();
                if (resultTables == null) {
                    resultTables = newTable;
                }
                else {
                    resultTables.Last.More = newTable;
                }
            }
        }
    }
}
