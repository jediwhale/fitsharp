// FitNesse.NET
// Copyright © 2008 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class ExecuteParse: ExecuteBase, ParseOperator<Cell> {
        public override bool IsMatch(Processor<Cell> processor, ExecuteParameters parameters) {
            if (parameters.Verb != ExecuteParameters.Check) return false;
            TypedValue actualValue = parameters.GetTypedActual(processor);
            return typeof (Parse).IsAssignableFrom(actualValue.Type);
        }

        public bool TryParse(Processor<Cell> processor, Type type, TypedValue instance, Tree<Cell> parameters, ref TypedValue result) {
            if (!typeof (Parse).IsAssignableFrom(type)) return false;
            result = new TypedValue(((Parse)parameters).Parts.DeepCopy());
            return true;
        }

        public override TypedValue Execute(Processor<Cell> processor, ExecuteParameters parameters) {
            TypedValue actualValue = parameters.GetTypedActual(processor);

            var cell = (Parse) parameters.Cell;
            var expected = new FixtureTable(cell.Parts);
            var tables = (Parse) actualValue.Value;
            var actual = new FixtureTable(tables);
            string differences = actual.Differences(expected);
            if (differences.Length == 0) {
				parameters.TestStatus.MarkRight(parameters.Cell);
            }
            else {
                parameters.TestStatus.MarkWrong(parameters.Cell, differences);
                cell.More = new Parse("td", string.Empty, tables, null);
            }
            return TypedValue.Void;
        }
    }
}
