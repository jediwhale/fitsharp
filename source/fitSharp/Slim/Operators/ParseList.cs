// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections;
using System.Collections.Generic;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Slim.Operators {
    public class ParseList: ParseOperator<string> { // todo: handle any IList type
        public bool TryParse(Processor<string> processor, Type type, TypedValue instance, Tree<string> parameters, ref TypedValue result) {
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof (List<>)) return false;
            var list = (IList)Activator.CreateInstance(type);
            foreach (Tree<string> branch in parameters.Branches) {
                list.Add(processor.Parse(type.GetGenericArguments()[0], branch).Value);
            }
            result = new TypedValue(list);
            return true;
        }
    }
}