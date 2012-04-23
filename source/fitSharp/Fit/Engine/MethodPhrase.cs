// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Exception;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Engine {
    public class MethodPhrase {
        public MethodPhrase(Tree<Cell> theCells) {
            cells = theCells;
            keyword = cells.ValueAt(0).Text;
        }

        public object Evaluate(DomainAdapter theFixture, CellProcessor processor) {
            cells.ValueAt(0).SetAttribute(CellAttribute.Syntax, CellAttributeValue.SyntaxKeyword);

            var cellCount = cells.Branches.Count;
            if (cellCount < 2) throw MakeException("missing cells");
            var identifier = cells.ValueAt(1).Text;
            if (newIdentifier.Equals(identifier)) {
                cells.ValueAt(1).SetAttribute(CellAttribute.Syntax, CellAttributeValue.SyntaxKeyword);
                return new MethodPhrase(cells.Skip(1)).EvaluateNew(processor);
            }
            if (typeIdentifier.Equals(identifier)) {
                cells.ValueAt(1).SetAttribute(CellAttribute.Syntax, CellAttributeValue.SyntaxKeyword);
                if (cellCount < 3) throw MakeException("missing cells");
                cells.ValueAt(2).SetAttribute(CellAttribute.Syntax, CellAttributeValue.SyntaxSUT);
                return processor.ParseTree(typeof (Type), cells.Branches[2]).Value;
            }
            if (currentIdentifier.Equals(identifier)) {
                cells.ValueAt(1).SetAttribute(CellAttribute.Syntax, CellAttributeValue.SyntaxKeyword);
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
            cells.ValueAt(1).SetAttribute(CellAttribute.Syntax, CellAttributeValue.SyntaxSUT);
            return processor.Create(cells.Branches[1], cells.Skip(2)).Value;
        }

        static readonly IdentifierName newIdentifier = new IdentifierName("new"); 
        static readonly IdentifierName typeIdentifier = new IdentifierName("type"); 
        static readonly IdentifierName currentIdentifier = new IdentifierName("current"); 

        readonly Tree<Cell> cells;
        readonly string keyword;
    }
}
