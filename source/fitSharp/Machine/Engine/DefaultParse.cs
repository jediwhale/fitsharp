// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class DefaultParse<T, P>: Operator<T, P>, ParseOperator<T> where P: class, Processor<T> {

        public bool CanParse(Type type, TypedValue instance, Tree<T> parameters) {
            return true;
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<T> parameters) {
            if (type.IsAssignableFrom(typeof(string))) {
                return new TypedValue(parameters.Value.ToString(), typeof(string));
            }

            foreach (var parse in new RuntimeType(type).FindStatic(MemberName.ParseMethod, new[] {typeof (string)}).Value) {
                if (parse.ReturnType == type) {
                    return parse.Invoke(new object[] {parameters.Value.ToString()});
                }
            }

            foreach (var construct in new RuntimeType(type).FindConstructor(new[] {typeof (string)}).Value) {
                return construct.Invoke(new object[] {parameters.Value.ToString()});
            }

            throw new InvalidOperationException(
                string.Format("Can't parse {0} because it doesn't have a static Parse method", type.FullName));
        }
    }
}
