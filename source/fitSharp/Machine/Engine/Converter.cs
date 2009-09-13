// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Model;
using fitSharp.Slim.Service;

namespace fitSharp.Machine.Engine {
    public abstract class Converter<T>: Operator<Service>, ParseOperator<string>, ComposeOperator<string> {
        public bool CanCompose(TypedValue instance) {
            return IsMatch(instance.Type);
        }

        public Tree<string> Compose(TypedValue instance) {
            return new TreeLeaf<string>(Compose((T)instance.Value));
        }

        public bool CanParse(Type type, TypedValue instance, Tree<string> parameters) {
            return IsMatch(type);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<string> parameters) {
            return new TypedValue(Parse(parameters.Value), type);
        }

        private static bool IsMatch(Type type) {
            return type == typeof(T);
        }

        protected abstract T Parse(string input);
        protected abstract string Compose(T input);
    }
}