// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class CheckOperationError: CellOperator, InvokeOperator<Cell> {

        public bool CanInvoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            return instance.Type == typeof (CellOperationContext) && memberName == CellOperationContext.CheckCommand
                && errorIdentifier.Equals(parameters.Value.Text);
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            var context = instance.GetValue<CellOperationContext>();
            try {
                var actual = context.GetActual(Processor);
                Processor.TestStatus.MarkWrong(parameters.Value, actual.ToString()); //todo: compose??
            }
            catch {
                Processor.TestStatus.MarkRight(parameters.Value);
            }
            return TypedValue.Void;
        }

        static readonly IdentifierName errorIdentifier = new IdentifierName("error");
    }
}