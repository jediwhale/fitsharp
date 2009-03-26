// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Engine;
using fitlibrary;
using fitlibrary.exception;
using fitSharp.Machine.Application;
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
            else if (ourTypeIdentifier.Equals(restOfCells.Text)) {
                if (restOfCells.More == null) throw new TableStructureException("missing cells for with.");
                //return new TypeName(restOfCells.More.Text).Type;
                return Context.Configuration.GetItem<Service>().ParseTree<Type>(restOfCells.More);
            }
            else if (ourCurrentIdentifier.Equals(restOfCells.Text)) {
                return theFixture.SystemUnderTest;
            }
            else {
                FlowFixtureBase fixture = theFixture as FlowFixtureBase;
                if (fixture == null) throw new TableStructureException("flow fixture required.");
                object namedObject = fixture.NamedFixture(restOfCells.Text);
                if (namedObject != null) return namedObject;
                return fixture.ExecuteEmbeddedMethod(myCells, 0);
            }
        }

        public object EvaluateNew(Fixture theFixture) {
            Parse restOfCells = myCells.More;
            if (restOfCells == null) throw new TableStructureException("missing cells for with.");
            return Context.Configuration.GetItem<Service>().Create(restOfCells.Text, new CellRange(restOfCells.More)).Value;
            //Type newType = new TypeName(restOfCells.Text).Type;
            //Method method = Method.FindFirst(newType, new IdentifierName(".ctor"), restOfCells.Size - 1);
            //return method.Invoke(theFixture, restOfCells.More);
        }

        private static readonly IdentifierName ourNewIdentifier = new IdentifierName("new"); 
        private static readonly IdentifierName ourTypeIdentifier = new IdentifierName("type"); 
        private static readonly IdentifierName ourCurrentIdentifier = new IdentifierName("current"); 

        private Parse myCells;
    }
}
