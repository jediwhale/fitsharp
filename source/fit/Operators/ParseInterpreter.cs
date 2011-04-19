// Copyright © 2010 Syterra Software Inc.
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
            TypedValue result = Processor.Create(className);

            var fixture = result.Value as Fixture;
            if (fixture == null) {
                var interpreter = result.Value as Interpreter;
                if (interpreter != null) {
                    interpreter.Processor = Processor;
                    return interpreter;
                }
                fixture = new DoFixture(result.Value);
            }
            fixture.Processor = Processor;
            fixture.GetArgsForRow(parameters);
            if (!instance.IsVoid) fixture.SetSystemUnderTest(instance.Value);
            return fixture;
        }
    }
}
