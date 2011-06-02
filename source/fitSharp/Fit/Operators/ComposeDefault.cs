// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Text;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ComposeDefault : CellOperator, ComposeOperator<Cell> {
        public bool CanCompose(TypedValue instance) {
            return true;
        }

        public Tree<Cell> Compose(TypedValue instance) {
            var newCell = Processor.MakeCell(GetValueString(instance));
            newCell.Value.SetAttribute(CellAttribute.Add, string.Empty);
            return newCell;
        }

        static string GetValueString(TypedValue instance) {
            string valueString;
            if (!instance.IsNullOrEmpty && instance.Value is Array) {
                var arrayString = new StringBuilder();
                foreach (object value in (IEnumerable)instance.Value) {
                    if (arrayString.Length > 0) arrayString.Append(", ");
                    arrayString.Append(value.ToString());
                }
                valueString =  arrayString.ToString();
            }
            else
                valueString = instance.ValueString;
            return valueString;
        }
    }
}
