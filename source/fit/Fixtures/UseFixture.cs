// FitNesse.NET
// Copyright © 2008 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Fixtures;
using fitlibrary;
using fitlibrary.exception;

namespace fit {
    public class UseFixture: Fixture {

        public override void DoTable(Parse theTable) {
            Parse firstRowCells = theTable.Parts.Parts;
            Parse restOfTheCells = firstRowCells.More;
            if (restOfTheCells == null) throw new TableStructureException("Missing cells for use.");

            string fixtureName = restOfTheCells.Text;
            Fixture targetFixture = GetNamedFixture(fixtureName);
            if (targetFixture == null) targetFixture = MakeNewFixture(fixtureName, restOfTheCells.More);

            targetFixture.Prepare(this, theTable);
            targetFixture.DoTable(theTable);
        }

        private Fixture GetNamedFixture(string theName) {
            FlowFixtureBase parent = myParentFixture as FlowFixtureBase;
            if (parent == null) return null;
            object namedObject = parent.NamedFixture(theName);
            if (namedObject == null) return null;
            Fixture result = FixtureResult.Wrap(namedObject) as Fixture;
            if (result != null) return result;
            else throw new FitFailureException("Result is not a Fixture.");
        }

        private Fixture MakeNewFixture(string theFixtureName, Parse theRestOfTheCells) {
            Fixture fixture = LoadFixture(theFixtureName);
            if (theRestOfTheCells != null) {
                Fixture parent = myParentFixture as FlowFixtureBase;
                fixture.SetSystemUnderTest(new WithPhrase(theRestOfTheCells).Evaluate(parent != null ? parent : this));
            }
            return fixture;
        }
    }
}
