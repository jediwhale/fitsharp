// Copyright © 2010 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Model;
using fitlibrary;
using fitlibrary.exception;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Fixtures {
    public class MethodPhrase {
        static readonly IdentifierName ourNewIdentifier = new IdentifierName("new"); 
        static readonly IdentifierName ourTypeIdentifier = new IdentifierName("type"); 
        static readonly IdentifierName ourCurrentIdentifier = new IdentifierName("current"); 

        readonly Parse myCells;
        readonly string keyword;

        public MethodPhrase(Parse theCells) {
            myCells = theCells;
            keyword = myCells.Text;
        }

        public object Evaluate(Fixture theFixture) {
            if (myCells.More == null) throw MakeException("missing cells");
            Parse restOfCells = myCells.More;
            if (ourNewIdentifier.Equals(restOfCells.Text)) {
                return new MethodPhrase(restOfCells).EvaluateNew(theFixture);
            }
            if (ourTypeIdentifier.Equals(restOfCells.Text)) {
                if (restOfCells.More == null) throw MakeException("missing cells");
                return theFixture.Processor.ParseTree(typeof (Type), restOfCells.More).Value;
            }
            if (ourCurrentIdentifier.Equals(restOfCells.Text)) {
                return theFixture.SystemUnderTest;
            }
            var fixture = theFixture as FlowFixtureBase;
            if (fixture == null) throw MakeException("flow fixture required");

            var symbols = fixture.Processor.Get<Symbols>();
            return symbols.HasValue(restOfCells.Text)
                ? symbols.GetValue(restOfCells.Text)
                : fixture.ExecuteEmbeddedMethod(myCells);
        }

        TableStructureException MakeException(string reason) {
            return new TableStructureException(string.Format("{0} for {1}.", reason, keyword));
        }

        public object EvaluateNew(Fixture theFixture) {
            Parse restOfCells = myCells.More;
            if (restOfCells == null) throw MakeException("missing cells");
            return theFixture.Processor.Create(restOfCells.Text, new CellRange(restOfCells.More)).Value;
        }
    }
}
