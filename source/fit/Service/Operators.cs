// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Operators;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Service {
    public class Operators: CellOperators, Copyable {

        public Operators() {
            Add(new ComposeDefault(), 0);

            Add(new RuntimeFlow(), 0);
            Add(new RuntimeProcedure(), 0);

            Add(new ComposeStoryTestString(), 0);
            Add(new ParseStoryTestString(), 0);

            Add(new ComposeTable(), 0);
            Add(new ExecuteList(), 0);
            Add(new ParseTable(), 0);
            Add(new ParseTree(), 0);
            Add(new ParseInterpreter(), 0);
        }

        public Operators(Operators<Cell, CellProcessor> other) {
            Copy(other);
        }

        public Copyable Copy() {
            return new Operators(this);
        }
    }
}
