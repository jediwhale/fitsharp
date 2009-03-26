// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public abstract class Converter<T>: ParseOperator<string>, ComposeOperator<string> {
        public bool TryParse(Processor<string> processor, Type type, TypedValue instance, Tree<string> parameters, ref TypedValue result) {
            if (!IsMatch(type)) return false;
            result = new TypedValue(Parse(parameters.Value), type);
            return true;
        }

        public bool TryCompose(Processor<string> processor, TypedValue instance, ref Tree<string> result) {
            if (!IsMatch(instance.Type)) return false;
            result = new TreeLeaf<string>(Compose((T)instance.Value));
            return true;
        }

        private static bool IsMatch(Type type) {
            return type == typeof(T);
        }

        protected abstract T Parse(string input);
        protected abstract string Compose(T input);
    }
}