// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class CompareDefault: CellOperator, CompareOperator<Cell> {
        public bool CanCompare(TypedValue actual, Tree<Cell> expected) {
            return true;
        }

        public bool Compare(TypedValue actualValue, Tree<Cell> expected) {
            expected.Value.ClearAttribute(CellAttribute.Difference);
            if (actualValue.IsVoid) {
                return false;
            }
            actualValue.ThrowExceptionIfNotValid();
            var expectedValue = Processor.ParseTree(actualValue.Type, expected);

            return AreEqual(actualValue, expectedValue, expected.Value);
        }

        bool AreEqual(TypedValue actualValue, TypedValue expectedValue, Cell expectedCell) {
            if (expectedValue.Type == typeof(DateTime) && actualValue.Type == typeof(DateTime))
                return expectedValue.ValueString == actualValue.ValueString;

            if (expectedValue.IsNull)
                return actualValue.IsNull;

            if (actualValue.IsNull) return false;

            if (expectedValue.GetValueAs<Array>() != null && actualValue.GetValueAs<Array>() != null)
                return ArraysAreEqual(expectedValue.GetValueAs<Array>(), actualValue.GetValueAs<Array>());

            if (expectedValue.Type == typeof(string)) {
                if (expectedValue.ValueString == actualValue.ValueString) return true;
                if (expectedCell != null) {
                    var difference = new StringDifference(expectedValue.ValueString, actualValue.ValueString).ToString();
                    if (difference.Length > 0) expectedCell.SetAttribute(CellAttribute.Difference, difference);
                }
                return false;
            }

            return expectedValue.Value.Equals(actualValue.Value);
        }

        bool ArraysAreEqual(Array a1, Array a2) { //todo: for any IEnumerable
            if (a1.Length != a2.Length)
                return false;
            for (var i = 0; i < a1.Length; i++) {
                if (!AreEqual(new TypedValue(a1.GetValue(i), a1.GetType().GetElementType()), new TypedValue(a2.GetValue(i), a2.GetType().GetElementType()), null))
                    return false;
            }
            return true;
        }
    }
}
