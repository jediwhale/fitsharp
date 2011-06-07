// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitlibrary;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class ParseInterpreter: CellOperator, ParseOperator<Cell> {
        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return typeof (Interpreter).IsAssignableFrom(type);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            Cell classCell = parameters.Branches[0].Value;
            object interpreter = CreateInterpreter(parameters, classCell, instance);
            return new TypedValue(interpreter);
        }

        private object CreateInterpreter(Tree<Cell> parameters, Cell classCell, TypedValue instance) {
            string className = classCell.Text.Trim();
            if (className.Length == 0 || !char.IsLetter(className[0])) return new CommentFixture();
            var result = Processor.Create(className);

            var interpreter = result.GetValueAs<Interpreter>() ?? new DoFixture(result.Value);

            interpreter.Prepare(Processor, null, parameters);

            if (!instance.IsVoid) {
                var adapter = interpreter as MutableDomainAdapter;
                if (adapter != null) adapter.SetSystemUnderTest(instance.Value);
            }
            return interpreter;
        }
    }
}
