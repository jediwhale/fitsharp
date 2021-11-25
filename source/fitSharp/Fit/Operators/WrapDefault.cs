// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using fitSharp.Fit.Engine;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class WrapDefault: CellOperator, WrapOperator {
        public bool CanWrap(TypedValue result) {
            return true;
        }

        public TypedValue Wrap(TypedValue result) {
            if (!result.HasValue) return result;
            if (result.Type.IsPrimitive) return result;
            if (result.Type == typeof(string)) return result;
            var wrapInterpreter = result.GetValueAs<Interpreter>();
            if (wrapInterpreter != null) return result;
            if (typeof (IEnumerable<object>).IsAssignableFrom(result.Type))
                return MakeInterpreter("fitlibrary.ArrayFixture", typeof(IEnumerable<object>), result.Value);
            if (typeof (IDictionary).IsAssignableFrom(result.Type))
                return MakeInterpreter("fitlibrary.SetFixture", typeof(IEnumerable), result.GetValue<IDictionary>().Values);
            if (typeof (DataTable).IsAssignableFrom(result.Type))
                return MakeInterpreter("fitlibrary.ArrayFixture", typeof(DataTable), result.Value);
            if (typeof (XmlDocument).IsAssignableFrom(result.Type))
                return MakeInterpreter("fitlibrary.XmlFixture", typeof(XmlDocument), result.Value);
            if (typeof (IEnumerable).IsAssignableFrom(result.Type))
                return MakeInterpreter("fitlibrary.ArrayFixture", typeof(IEnumerable), result.Value);
            if (typeof (IEnumerator).IsAssignableFrom(result.Type))
                return MakeInterpreter("fitlibrary.ArrayFixture", typeof(IEnumerator), result.Value);
            return new TypedValue(new DefaultFlowInterpreter(result.Value));
        }

        TypedValue MakeInterpreter(string fixtureName, Type parameterType, object parameter) {
            var runtimeType = new RuntimeType(Processor.ApplicationUnderTest.FindType(fixtureName));
            foreach (var runtimeMember in runtimeType.FindConstructor(new [] {parameterType}).Value) {
                return runtimeMember.Invoke(new [] {parameter});
            }
            throw new ConstructorMissingException(parameterType, 1);
        }
    }
}
