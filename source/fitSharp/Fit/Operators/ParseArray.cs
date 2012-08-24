// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections;
using System.Collections.Generic;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseArray: CellOperator, ParseOperator<Cell> {
        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return type != typeof (string)
                && typeof (IEnumerable).IsAssignableFrom(type)
                && parameters.Branches.Count == 0;
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            var content = Processor.Parse<Cell, string>(parameters.Value);
            var strings = !string.IsNullOrEmpty(content)
                ? content.Split(new[] { ',' })
                : new string[]{};

            if (type.IsArray) {
			    Array array = Array.CreateInstance(type.GetElementType(), strings.Length);
			    for (int i = 0; i < strings.Length; i++) {
                    //todo: use cellsubstring?
			        array.SetValue(Processor.ParseString(type.GetElementType(), strings[i].Trim()).Value, i);
			    }
                return new TypedValue(array);
            }
            Type elementType = type.GetGenericArguments()[0];
            Type resultType = typeof (List<>).MakeGenericType(new[] {elementType});
            var list = (IList) Activator.CreateInstance(resultType);
            foreach (string element in strings) {
                list.Add(Processor.ParseString(elementType, element.Trim()).Value);
            }
            return new TypedValue(list);
        }
    }
}
