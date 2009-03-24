// FitNesse.NET
// Copyright © 2006-2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class ParseTree : ParseOperator<Cell> {
        public bool TryParse(Processor<Cell> processor, Type type, TypedValue instance, Tree<Cell> parameters, ref TypedValue result) {
            if (!typeof(fitlibrary.tree.Tree).IsAssignableFrom(type)) return false;
            var cell = (Parse)parameters.Value;
            result = new TypedValue(new fitlibrary.tree.ParseTree(cell.Parts ?? new Parse("ul", cell.Text, null, null)));
            return true;
        }
    }
}