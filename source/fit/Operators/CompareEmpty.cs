// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class CompareEmpty: CompareOperator<Cell> {
        public bool CanCompare(Processor<Cell> processor, TypedValue actual, Tree<Cell> expected) {
            return (string.IsNullOrEmpty(expected.Value.Text))
                   && ((Parse) expected.Value).Parts == null;
        }

        public bool Compare(Processor<Cell> processor, TypedValue actual, Tree<Cell> expected) {
            return actual.IsNullOrEmpty;
        }
    }
}
