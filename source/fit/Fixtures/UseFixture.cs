// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Fixtures;
using fitlibrary;
using fitlibrary.exception;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit {
    public class UseFixture: Fixture {

        public override void DoTable(Parse theTable) {
            Parse firstRowCells = theTable.Parts.Parts;
            Parse restOfTheCells = firstRowCells.More;
            if (restOfTheCells == null) throw new TableStructureException("Missing cells for use.");

            string fixtureName = restOfTheCells.Text;
            var targetFixture = GetNamedFixture(fixtureName) ?? MakeNewFixture(fixtureName, restOfTheCells.More);

            targetFixture.Interpret(Processor, theTable);
        }

        Interpreter GetNamedFixture(string theName) {
            var parent = Processor.TestStatus.Parent;
            if (parent == null) return null;

            var symbol = new Symbol(theName);
            if (!Processor.Contains(symbol)) return null;

            var result =  CellOperation.Wrap(new TypedValue(Processor.Load(symbol).Instance));
            result.AsNot<Interpreter>(() => { throw new FitFailureException("Result is not a Fixture."); });
            return result.GetValueAs<Interpreter>();
        }

        Interpreter MakeNewFixture(string theFixtureName, Parse theRestOfTheCells) {
            var fixture = Processor.Create(theFixtureName.Trim()).Value as Interpreter; //todo: parse interpreter?
            if (fixture == null) {
                throw new FitFailureException(theFixtureName + " is not a Fixture.");
            }
            if (theRestOfTheCells != null) {
                var adapter = fixture as MutableDomainAdapter;
                if (adapter != null) {
                    Fixture parent = Processor.TestStatus.Parent as FlowFixtureBase;
                    adapter.SetSystemUnderTest(new MethodPhrase(theRestOfTheCells).Evaluate(parent ?? this));
                }
            }
            return fixture;
        }
    }
}
