// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Exception;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Fixtures {
    public class UseFixture: Interpreter {
        public void Interpret(CellProcessor processor, Tree<Cell> table) {
            var firstRow = table.Branches[0];
            if (firstRow.Branches.Count < 2) throw new TableStructureException("Missing cells for use.");

            var fixtureName = firstRow.ValueAt(1).Text;
            var targetFixture = GetNamedFixture(processor, fixtureName)
                ?? MakeNewFixture(processor, firstRow);

            targetFixture.Interpret(processor, table);
        }

        static Interpreter GetNamedFixture(CellProcessor processor, string theName) {
            if (!processor.Get<Symbols>().HasValue(theName)) return null;

            var result =  processor.Operate<WrapOperator>(new TypedValue(processor.Get<Symbols>().GetValue(theName)));
            result.AsNot<Interpreter>(() => { throw new FitFailureException("Result is not a Fixture."); });
            return result.GetValueAs<Interpreter>();
        }

        static Interpreter MakeNewFixture(CellProcessor processor, Tree<Cell> firstRow) {
            var fixture = processor.ParseTree<Cell, Interpreter>(firstRow.Skip(1));
            if (firstRow.Branches.Count > 2) {
                var adapter = fixture as MutableDomainAdapter;
                if (adapter != null) {
                    var parent = processor.CallStack.DomainAdapter.GetValueAs<DomainAdapter>();
                    adapter.SetSystemUnderTest(new MethodPhrase(firstRow.Skip(2)).Evaluate(parent, processor));
                }
            }
            return fixture;
        }
    }
}
