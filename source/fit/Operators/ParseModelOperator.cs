// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class ParseModelOperator: CellOperator, InvokeOperator<Cell>, ParseOperator<Cell> { // todo: split, eliminate fit dependency
        public bool CanInvoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            var context = instance.Value as CellOperationContext;
            return context != null && memberName == CellOperationContext.CheckCommand
                   && typeof (Parse).IsAssignableFrom(context.GetTypedActual(Processor).Type);
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            var context = instance.GetValue<CellOperationContext>();
            TypedValue actualValue = context.GetTypedActual(Processor);

            var cell = (Parse) parameters.Value;
            var expected = new FixtureTable(cell.Parts);
            var tables = actualValue.GetValue<Parse>();
            var actual = new FixtureTable(tables);
            string differences = actual.Differences(expected);
            if (differences.Length == 0) {
				Processor.TestStatus.MarkRight(parameters.Value);
            }
            else {
                Processor.TestStatus.MarkWrong(parameters.Value, differences);
                cell.More = new Parse("td", string.Empty, tables, null);
            }
            return TypedValue.Void;
        }

        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return typeof (Parse).IsAssignableFrom(type);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return new TypedValue(((Parse)parameters).Parts.DeepCopy());
        }

    }
}
