// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Model;
using fitlibrary;
using fitlibrary.exception;
using fitSharp.Machine.Model;


namespace fit.Fixtures {
    public class WithPhrase {
        public WithPhrase(Parse theCells) {
            myCells = theCells;
        }

        public object Evaluate(Fixture theFixture) {
            if (myCells.More == null) throw new TableStructureException("missing cells for with.");
            Parse restOfCells = myCells.More;
            if (ourNewIdentifier.Equals(restOfCells.Text)) {
                return new WithPhrase(restOfCells).EvaluateNew(theFixture);
            }
            if (ourTypeIdentifier.Equals(restOfCells.Text)) {
                if (restOfCells.More == null) throw new TableStructureException("missing cells for with.");
                return theFixture.Processor.Parse(typeof (Type), TypedValue.Void, restOfCells.More).Value;
            }
            if (ourCurrentIdentifier.Equals(restOfCells.Text)) {
                return theFixture.SystemUnderTest;
            }
            var fixture = theFixture as FlowFixtureBase;
            if (fixture == null) throw new TableStructureException("flow fixture required.");
            return fixture.NamedFixture(restOfCells.Text) ?? fixture.ExecuteEmbeddedMethod(myCells);
        }

        public object EvaluateNew(Fixture theFixture) {
            Parse restOfCells = myCells.More;
            if (restOfCells == null) throw new TableStructureException("missing cells for with.");
            return theFixture.Processor.Create(restOfCells.Text, new CellRange(restOfCells.More)).Value;
        }

        private static readonly IdentifierName ourNewIdentifier = new IdentifierName("new"); 
        private static readonly IdentifierName ourTypeIdentifier = new IdentifierName("type"); 
        private static readonly IdentifierName ourCurrentIdentifier = new IdentifierName("current"); 

        private readonly Parse myCells;
    }
}
