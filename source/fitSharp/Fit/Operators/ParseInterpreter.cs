// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Fixtures;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseInterpreter: CellOperator, ParseOperator<Cell> {
        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return typeof (Interpreter).IsAssignableFrom(type);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            var classCell = parameters.ValueAt(0);
            var interpreter = CreateInterpreter(classCell.Text.Trim(), instance);
            return new TypedValue(interpreter);
        }

        Interpreter CreateInterpreter(string className, TypedValue instance) {
            return WithSystemUnderTest(MakeInterpreter(className), instance);
        }

        Interpreter MakeInterpreter(string className) {
            if (className.Length == 0 || !char.IsLetter(className[0])) return new CommentFixture();

            var result = Processor.Create(className);
            return result.GetValueAs<Interpreter>() ?? WithSystemUnderTest(Processor.Create("fitlibrary.DoFixture").GetValueAs<Interpreter>(), result);
        }

        Interpreter WithSystemUnderTest(Interpreter interpreter, TypedValue systemUnderTest) {
            if (!systemUnderTest.IsVoid) {
                Processor.CallStack.DomainAdapter = systemUnderTest;
                var adapter = interpreter as MutableDomainAdapter;
                if (adapter != null) adapter.SetSystemUnderTest(systemUnderTest.Value);
            }
            return interpreter;
        }
    }
}
