// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service {
    public class InvokeOperation {
        public TypedValue Target { get; private set; }
        public Tree<Cell> Member { get; private set; }
        public Tree<Cell> Parameters { get; private set; }
        public Tree<Cell> Cells { get; private set; }

        public InvokeOperation(CellProcessor processor, TypedValue target, Tree<Cell> member, Tree<Cell> parameters, Tree<Cell> cells) {
            this.processor = processor;
            Target = target;
            Member = member;
            Parameters = parameters;
            Cells = cells;
        }

        public TypedValue Do() {
            var beforeCounts = new TestCounts(processor.TestStatus.Counts);
            var targetObjectProvider = Target.Value as TargetObjectProvider;
            string name = GetMemberName(Member);
            TypedValue result = processor.Invoke(
                    targetObjectProvider != null ? new TypedValue(targetObjectProvider.GetTargetObject()) : Target,
                    name, Parameters);
            MarkCellWithLastResults(beforeCounts);
            return result;
        }

        string GetMemberName(Tree<Cell> members) {
            return processor.ParseTree<Cell, MemberName>(members).ToString();
        }

        void MarkCellWithLastResults(TestCounts beforeCounts) {
            if (Cells != null && !string.IsNullOrEmpty(processor.TestStatus.LastAction)) {
                Cells.Value.SetAttribute(CellAttribute.Folded, processor.TestStatus.LastAction);
                MarkCellWithCounts(beforeCounts);
            }
            processor.TestStatus.LastAction = null;
        }

        void MarkCellWithCounts(TestCounts beforeCounts) {
            string style = processor.TestStatus.Counts.Subtract(beforeCounts).Style;
            if (!string.IsNullOrEmpty(style) && string.IsNullOrEmpty(Cells.Value.GetAttribute(CellAttribute.Status))) {
                Cells.Value.SetAttribute(CellAttribute.Status, style);
            }
        }

        readonly CellProcessor processor;
    }
}
