// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Operators;

namespace fitSharp.Slim.Service {
    public class Service: Processor<string> {

        public Service() {
            AddMemory<SavedInstance>();
            AddMemory<Symbol>();
            AddOperator(new ExecuteDefault());
            AddOperator(new ExecuteImport());
            AddOperator(new ExecuteMake());
            AddOperator(new ExecuteCall());
            AddOperator(new ExecuteCallAndAssign());
            AddOperator(new ParseList());
            AddOperator(new ParseSymbol(), 1);
            AddOperator(new ComposeDefault());
            AddOperator(new ComposeException());
            AddOperator(new ComposeBoolean());
            AddOperator(new ComposeList());
        }

        public Tree<string> ExecuteInstructions(Tree<string> instructions) {
            var results = new TreeList<string>();
            foreach (Tree<string> statement in instructions.Branches) {
                var result = (Tree<string>) Execute(statement).Value;
                results.AddBranchValue(result);
                if (ExecuteBase.WasAborted(result)) break;
            }
            return results;
        }
    }
}