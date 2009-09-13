// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class CompareDefault: Operator<CellProcessor>, CompareOperator<Cell> {
        public bool CanCompare(TypedValue actual, Tree<Cell> expected) {
            return true;
        }

        public bool Compare(TypedValue actual, Tree<Cell> expected) {
            if (actual.IsVoid) {
                return false;
            }
            TypedValue expectedValue = Processor.Parse(actual.Type, expected);
            return AreEqual(expectedValue.Value, actual.Value);
        }

        private static bool AreEqual(object o1, object o2)
        {
            if (o1 is DateTime && o2 is DateTime)
                return o1.ToString().Equals(o2.ToString());
            if (o1 == null)
                return o2 == null;
            if (o2 == null) return false;
            if (o1 is Array && o2 is Array)
                return ArraysAreEqual(o1, o2);
            if (o1 is string)
                return o1.Equals(o2.ToString());
            return (o1.Equals(o2));
        }

        private static bool ArraysAreEqual(object o1, object o2)
        {
            var a1 = (Array) o1;
            var a2 = (Array) o2;
            if (a1.Length != a2.Length)
                return false;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!AreEqual(a1.GetValue(i), a2.GetValue(i)))
                    return false;
            }
            return true;
        }
    }
}