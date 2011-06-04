// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Exception;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators
{
    public class CheckOperationDefault: CellOperator, InvokeOperator<Cell>
    {
        public bool CanInvoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            return instance.Type == typeof (CellOperationContext) && memberName == CellOperationContext.CheckCommand;
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            var context = instance.GetValue<CellOperationContext>();
            try {
                var actual = context.GetTypedActual(Processor);
                if (Processor.Compare(actual, parameters)) {
                    Processor.TestStatus.MarkRight(parameters.Value);
                }
                else {
                    var actualCell = Processor.Compose(actual);
                    Processor.TestStatus.MarkWrong(parameters.Value, actualCell.Value.Text);
                }
            }
            catch (IgnoredException) {}
            MarkCellWithLastResults(parameters, p => {});
            return TypedValue.Void;
        }
    }
}
