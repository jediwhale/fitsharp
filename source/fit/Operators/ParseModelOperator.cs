// Copyright © 2013 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class ParseModelOperator: CellOperator, CheckOperator, ParseOperator<Cell> { // todo: split, eliminate fit dependency
        public bool CanCheck(CellOperationValue actualValue, Tree<Cell> expectedCell) {
            return typeof (Parse).IsAssignableFrom(actualValue.GetTypedActual(Processor).Type);
        }

        public TypedValue Check(CellOperationValue actualValue, Tree<Cell> expectedCell) {
            var result = false;
            var cell = (Parse) expectedCell.Value;
            var expectedTable = new FixtureTable(cell.Parts);
            var tables = actualValue.GetActual<Parse>(Processor);
            var actualTable = new FixtureTable(tables);
            string differences = actualTable.Differences(expectedTable);
            if (differences.Length == 0) {
				Processor.TestStatus.MarkRight(expectedCell.Value);
                result = true;
            }
            else {
                Processor.TestStatus.MarkWrong(expectedCell.Value, differences);
                cell.More = new Parse("td", string.Empty, tables, null);
            }
            return new TypedValue(result);
        }

        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return typeof (Parse).IsAssignableFrom(type);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return new TypedValue(((Parse)parameters).Parts.DeepCopy());
        }

    }
}
