// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class ParseStoryTestString: Operator<CellProcessor>, ParseOperator<Cell> {
        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return type == typeof(StoryTestString);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            var source = (Parse) parameters.Value;
            return new TypedValue(new StoryTestString(source.ToString()));
        }
    }
}
