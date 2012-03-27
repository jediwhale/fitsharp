// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service {
    public class Binding {
        private Action beforeAction = () => {};
        private readonly BindingOperation operation;

        public Binding(BindingOperation operation) {
            this.operation = operation;
        }

        public void Do(Tree<Cell> cell) {
            if (operation.IsCheck) beforeAction();
            operation.Do(cell);
        }

        public Binding BeforeCheck(Action beforeAction) {
            this.beforeAction = beforeAction;
            return this;
        }

        public bool IsCheck { get { return operation.IsCheck;} }
    }

    public interface BindingOperation {
        void Do(Tree<Cell> cell);
        bool IsCheck { get; }
    }

    public class InputBinding: BindingOperation {
        private readonly CellProcessor processor;
        private readonly TargetObjectProvider targetProvider;
        private readonly Tree<Cell> memberCell;


        public InputBinding(CellProcessor processor, TargetObjectProvider targetProvider, Tree<Cell> memberCell) {
            this.processor = processor;
            this.memberCell = memberCell;
            this.targetProvider = targetProvider;
        }

        public void Do(Tree<Cell> cell) {
            var instance = new TypedValue(targetProvider.GetTargetObject());
            if (cell.IsLeaf && cell.Value.Text.Length == 0) {
	            var actual = processor.Invoke(instance, GetMemberName(memberCell), new CellTree());
	            if (actual.IsValid) ShowActual(cell.Value, actual.Value);
            }
            else {
                var beforeCounts = new TestCounts(processor.TestStatus.Counts);
                processor.InvokeWithThrow(instance, GetMemberName(memberCell), new CellTree(cell));
                processor.TestStatus.MarkCellWithLastResults(cell.Value, beforeCounts);
            }
        }

        public bool IsCheck { get { return false; } }

        MemberName GetMemberName(Tree<Cell> members) {
            return processor.ParseTree<Cell, MemberName>(members);
        }

        static void ShowActual(Cell cell, object actual) {
            cell.SetAttribute(
                CellAttribute.InformationSuffix,
                actual == null ? "null"
	            : actual.ToString().Length == 0 ? "blank"
	            : actual.ToString());
	    }
    }

    public class CheckBinding: BindingOperation {
        private readonly CellProcessor processor;
        private readonly TargetObjectProvider targetProvider;
        private readonly Tree<Cell> memberCell;

        public CheckBinding(CellProcessor processor, TargetObjectProvider targetProvider, Tree<Cell> memberCell) {
            this.processor = processor;
            this.memberCell = memberCell;
            this.targetProvider = targetProvider;
        }

        public void Do(Tree<Cell> cell) {
            processor.Check(targetProvider.GetTargetObject(), memberCell, cell);
        }

        public bool IsCheck { get { return true; } }
    }

    public class CreateBinding: BindingOperation {
        private readonly CellProcessor processor;
        private readonly MutableDomainAdapter adapter;
        private readonly string memberName;


        public CreateBinding(CellProcessor processor, MutableDomainAdapter adapter, string memberName) {
            this.processor = processor;
            this.adapter = adapter;
            this.memberName = memberName;
        }

        public void Do(Tree<Cell> cell) {
            var instance = processor.Create(memberName, new CellTree(cell));
            adapter.SetSystemUnderTest(instance.Value);
        }

        public bool IsCheck { get { return false; } }
    }

    public class NoBinding: BindingOperation {
        public void Do(Tree<Cell> cell) {}
        public bool IsCheck { get { return false; } }
    }
}