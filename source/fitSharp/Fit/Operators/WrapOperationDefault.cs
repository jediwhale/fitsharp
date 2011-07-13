// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators
{
    public class WrapOperationDefault: CellOperator, InvokeOperator<Cell>
    {
        public bool CanInvoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            return instance.Type == typeof (CellOperationContext) && memberName == CellOperationContext.WrapCommand;
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            var context = instance.GetValue<CellOperationContext>();
            var result = context.GetTypedActual(Processor);
            if (!result.HasValue) return result;
            if (result.Type.IsPrimitive) return result;
            if (result.Type == typeof(string)) return result;
            var wrapInterpreter = result.GetValueAs<Interpreter>();
            if (wrapInterpreter != null) return result;
                if (typeof (IEnumerable<object>).IsAssignableFrom(result.Type))
                    return MakeInterpreter("fitlibrary.ArrayFixture", typeof(IEnumerable<object>), result.Value);
            if (typeof (IDictionary).IsAssignableFrom(result.Type))
                return MakeInterpreter("fitlibrary.SetFixture", typeof(IEnumerable), result.GetValue<IDictionary>().Values);
            if (typeof (IEnumerator).IsAssignableFrom(result.Type))
                return MakeInterpreter("fitlibrary.ArrayFixture", typeof(IEnumerator), result.Value);
            if (typeof (DataTable).IsAssignableFrom(result.Type))
                return MakeInterpreter("fitlibrary.ArrayFixture", typeof(DataTable), result.Value);
            if (typeof (XmlDocument).IsAssignableFrom(result.Type))
                return MakeInterpreter("fitlibrary.XmlFixture", typeof(XmlDocument), result.Value);
            if (typeof (IEnumerable).IsAssignableFrom(result.Type))
                return MakeInterpreter("fitlibrary.ArrayFixture", typeof(IEnumerable), result.Value);
            return MakeInterpreter("fitlibrary.DoFixture", typeof (object), result.Value);
        }

        TypedValue MakeInterpreter(string fixtureName, Type parameterType, object parameter) {
            var runtimeType = Processor.ApplicationUnderTest.FindType(fixtureName);
            var runtimeMember = runtimeType.FindConstructor(new [] {parameterType});
            return runtimeMember.Invoke(new [] {parameter});
        }
    }
}
