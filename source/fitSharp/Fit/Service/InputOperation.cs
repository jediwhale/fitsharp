// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service {
    public class InputOperation {
        public object SystemUnderTest { get; private set; }
        public Tree<Cell> Member { get; private set; }
        public Tree<Cell> Cells { get; private set; }

        public InputOperation(CellProcessor processor, object systemUnderTest, Tree<Cell> member, Tree<Cell> cells) {
            this.processor = processor;
            SystemUnderTest = systemUnderTest;
            Member = member;
            Cells = cells;
        }

        public void Do() {
            var beforeCounts = new TestCounts(processor.TestStatus.Counts);
            processor.InvokeWithThrow(new TypedValue(SystemUnderTest), GetMemberName(Member),
                             new CellTree(Cells));
            MarkCellWithLastResults(beforeCounts);
        }

        string GetMemberName(Tree<Cell> members) {
            return processor.ParseTree<Cell, MemberName>(members).ToString();
        }

        void MarkCellWithLastResults(TestCounts testCounts) {
            if (Cells != null && !string.IsNullOrEmpty(processor.TestStatus.LastAction)) {
                Cells.Value.SetAttribute(CellAttribute.Folded, processor.TestStatus.LastAction);
                MarkCellWithCounts(testCounts);
            }
            processor.TestStatus.LastAction = null;
        }

        void MarkCellWithCounts(TestCounts beforeCounts) {
            var style = processor.TestStatus.Counts.Subtract(beforeCounts).Style;
            if (!string.IsNullOrEmpty(style) && string.IsNullOrEmpty(Cells.Value.GetAttribute(CellAttribute.Status))) {
                Cells.Value.SetAttribute(CellAttribute.Status, style);
            }
        }

        readonly CellProcessor processor;
    }
}
