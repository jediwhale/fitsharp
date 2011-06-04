// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.


using System.Collections;
using System.Linq;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators
{
    public class CompareList: CellOperator, CompareOperator<Cell>
    {
        public bool CanCompare(TypedValue actual, Tree<Cell> expected) {
            if (!typeof(IList).IsAssignableFrom(actual.Type)) return false;
            return !expected.IsLeaf;
        }

        public bool Compare(TypedValue actual, Tree<Cell> expected) {
            var cell = (Parse)expected.Value;
            var matcher = new ListMatcher(Processor, new ArrayMatchStrategy(Processor, cell.Parts.Parts));
            return matcher.IsEqual(actual.GetValue<IEnumerable>().Cast<object>(), cell);
        }
    }
}
