// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
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
        private readonly CellOperation operation;
        private readonly TargetObjectProvider targetProvider;
        private readonly Tree<Cell> memberCell;


        public InputBinding(CellOperation operation, TargetObjectProvider targetProvider, Tree<Cell> memberCell) {
            this.operation = operation;
            this.memberCell = memberCell;
            this.targetProvider = targetProvider;
        }
        public void Do(Tree<Cell> cell) {
            operation.Input(targetProvider.GetTargetObject(), memberCell, cell);
        }

        public bool IsCheck { get { return false; } }
    }

    public class CheckBinding: BindingOperation {
        private readonly CellOperation operation;
        private readonly TargetObjectProvider targetProvider;
        private readonly Tree<Cell> memberCell;

        public CheckBinding(CellOperation operation, TargetObjectProvider targetProvider, Tree<Cell> memberCell) {
            this.operation = operation;
            this.memberCell = memberCell;
            this.targetProvider = targetProvider;
        }

        public void Do(Tree<Cell> cell) {
            operation.Check(targetProvider.GetTargetObject(), memberCell, cell);
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
            //operation.Create(adapter, memberName, new CellTree(cell));
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