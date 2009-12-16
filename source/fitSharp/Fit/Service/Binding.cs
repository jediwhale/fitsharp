// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Machine.Extension;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service {
    public class Binding {
        private Action beforeAction = () => {};
        private readonly BindingOperation operation;

        public Binding(BindingOperation operation) {
            this.operation = operation;
        }

        public void Do(Tree<Cell> cell) {
            operation.Do(this, cell);
        }

        public Binding BeforeCheck(Action beforeAction) {
            this.beforeAction = beforeAction;
            return this;
        }

        public void DoBeforeCheck() {
            beforeAction();
        }
    }

    public interface BindingOperation {
        void Do(Binding binding, Tree<Cell> cell);
    }

    public class InputBinding: BindingOperation {
        private readonly CellOperation operation;
        private readonly object target;
        private readonly Tree<Cell> memberCell;


        public InputBinding(CellOperation operation, object target, Tree<Cell> memberCell) {
            this.operation = operation;
            this.memberCell = memberCell;
            this.target = target;
        }
        public void Do(Binding binding, Tree<Cell> cell) {
            operation.Input(target, memberCell, cell);
        }
    }

    public class CheckBinding: BindingOperation {
        private readonly CellOperation operation;
        private readonly object target;
        private readonly Tree<Cell> memberCell;

        public CheckBinding(CellOperation operation, object target, Tree<Cell> memberCell) {
            this.operation = operation;
            this.memberCell = memberCell;
            this.target = target;
        }

        public void Do(Binding binding, Tree<Cell> cell) {
            binding.DoBeforeCheck();
            operation.Check(target, memberCell, cell);
        }
    }

    public class CreateBinding: BindingOperation {
        private readonly CellOperation operation;
        private readonly MutableDomainAdapter adapter;
        private readonly string memberName;

        public CreateBinding(CellOperation operation, MutableDomainAdapter adapter, string memberName) {
            this.operation = operation;
            this.adapter = adapter;
            this.memberName = memberName;
        }

        public void Do(Binding binding, Tree<Cell> cell) {
            operation.Create(adapter, memberName, new TreeList<Cell>().AddBranch(cell));
        }
    }

    public class NoBinding: BindingOperation {
        public void Do(Binding binding, Tree<Cell> cell) {}
    }
}