// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Operators;

namespace fitSharp.Slim.Service {
    public class Service: ProcessorBase<string, Processor<string>>, Copyable {
        private readonly Operators<string, Processor<string>> operators;

        public Service() {
            operators = new Operators<string, Processor<string>>(this);
            AddMemory<SavedInstance>();
            AddMemory<Symbol>();
            AddOperator(new ExecuteDefault());
            AddOperator(new ExecuteImport());
            AddOperator(new ExecuteMake());
            AddOperator(new ExecuteCall());
            AddOperator(new ExecuteCallAndAssign());
            AddOperator(new ParseDefault());
            AddOperator(new ParseType<string, Processor<string>>());
            AddOperator(new ParseList());
            AddOperator(new ParseSymbol(), 1);
            AddOperator(new ComposeDefault());
            AddOperator(new ComposeException());
            AddOperator(new ComposeBoolean());
            AddOperator(new ComposeList());
        }

        public Service(Service other): base(other) {
            operators = new Operators<string, Processor<string>>(this);
            operators.Copy(other.operators);
        }

        protected override Operators<string, Processor<string>> Operators {
            get { return operators; }
        }

        Copyable Copyable.Copy() {
            return new Service(this);
        }
    }
}