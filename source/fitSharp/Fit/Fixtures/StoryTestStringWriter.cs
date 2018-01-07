// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Text;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Fixtures {
    public class StoryTestStringWriter: StoryTestWriter {

        public string Tables { get { return tables.ToString(); } }
        public TestCounts Counts { get; private set; }

        public StoryTestStringWriter ForTables(Action<string> handleTables) {
            this.handleTables = handleTables;
            return this;
        }

        public StoryTestStringWriter ForCounts(Action<TestCounts> handleCounts) {
            this.handleCounts = handleCounts;
            return this;
        }

        public void WriteTable(Tree<Cell> table) {
            var tableResult = table.WriteTree();
            if (string.IsNullOrEmpty(tableResult)) return;

            HandleTableResult(tableResult);
            writesTables = true;
        }

        public void WriteTest(Tree<Cell> test, TestCounts counts) {
            if (!writesTables) {
                var testResult = test.WriteBranches();
                if (!string.IsNullOrEmpty(testResult)) HandleTableResult(testResult);
            }
            handleCounts(counts);
            Counts = counts;
        }

        void HandleTableResult(string tableResult) {
            handleTables(tableResult);
            tables.Append(tableResult);
        }

        readonly StringBuilder tables = new StringBuilder();

        Action<string> handleTables = s => {};
        Action<TestCounts> handleCounts = c => {};
        bool writesTables;
    }
}
