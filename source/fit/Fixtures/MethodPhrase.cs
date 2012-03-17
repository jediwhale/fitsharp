// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitlibrary.exception;
using fitSharp.Fit.Engine;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Fixtures {
    public class MethodPhrase {
        public MethodPhrase(Tree<Cell> theCells) {
            cells = theCells;
            keyword = cells.ValueAt(0).Text;
        }

        public object Evaluate(DomainAdapter theFixture, CellProcessor processor) {
            var cellCount = cells.Branches.Count;
            if (cellCount < 2) throw MakeException("missing cells");
            var identifier = cells.ValueAt(1).Text;
            if (newIdentifier.Equals(identifier)) {
                return new MethodPhrase(cells.Skip(1)).EvaluateNew(processor);
            }
            if (typeIdentifier.Equals(identifier)) {
                if (cellCount < 3) throw MakeException("missing cells");
                return processor.ParseTree(typeof (Type), cells.Branches[2]).Value;
            }
            if (currentIdentifier.Equals(identifier)) {
                return theFixture.SystemUnderTest;
            }
            var fixture = theFixture as FlowInterpreter;
            if (fixture == null) throw MakeException("flow fixture required");

            return processor.Get<Symbols>().HasValue(identifier)
                ? processor.Get<Symbols>().GetValue(identifier)
                : fixture.ExecuteFlowRowMethod(processor, cells);
        }

        TableStructureException MakeException(string reason) {
            return new TableStructureException(string.Format("{0} for {1}.", reason, keyword));
        }

        public object EvaluateNew(CellProcessor processor) {
            if (cells.Branches.Count < 2) throw MakeException("missing cells");
            return processor.Create(
                    cells.ValueAt(1).Text,
                    cells.Skip(2))
                .Value;
        }

        static readonly IdentifierName newIdentifier = new IdentifierName("new"); 
        static readonly IdentifierName typeIdentifier = new IdentifierName("type"); 
        static readonly IdentifierName currentIdentifier = new IdentifierName("current"); 

        readonly Tree<Cell> cells;
        readonly string keyword;
    }
}
