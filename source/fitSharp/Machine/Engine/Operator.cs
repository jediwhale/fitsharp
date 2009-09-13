// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public abstract class Operator<T, P> where P: Processor<T> {
        public P Processor;

        public Tree<T> Compose(object instance)  {
            return Processor.Compose(new TypedValue(instance));
        }

        public TypedValue Create(string membername) {
            return Processor.Create(membername, new TreeList<T>());
        }

        public TypedValue Execute(Tree<T> parameters) {
            return Processor.Execute(TypedValue.Void, parameters);
        }

        public TypedValue InvokeWithThrow(TypedValue instance, string memberName, Tree<T> parameters) {
            TypedValue result = Processor.Invoke(instance, memberName, parameters);
            result.ThrowExceptionIfNotValid();
            return result;
        }

        public TypedValue Parse(Type type, Tree<T> parameters) {
            return Processor.Parse(type, TypedValue.Void, parameters);
        }

        public TypedValue Parse(Type type, T input) {
            return Parse(type, new TreeLeaf<T>(input));
        }

        public V ParseTree<V>(Tree<T> input) {
            return (V) Parse(typeof (V), input).Value;
        }

        public V Parse<V>(T input) {
            return (V) Parse(typeof (V), input).Value;
        }

        public TypedValue ParseString(Type type, string input) {
            return Parse(type, Processor.Compose(new TypedValue(input, typeof(string))));
        }

        public V ParseString<V>(string input) {
            return (V) ParseString(typeof (V), input).Value;
        }
    }

    public interface CompareOperator<T> {
        bool CanCompare(TypedValue actual, Tree<T> expected);
        bool Compare(TypedValue actual, Tree<T> expected);
    }

    public interface ComposeOperator<T> {
        bool CanCompose(TypedValue instance);
        Tree<T> Compose(TypedValue instance);
    }

    public interface ExecuteOperator<T> {
        bool CanExecute(TypedValue instance, Tree<T> parameters);
        TypedValue Execute(TypedValue instance, Tree<T> parameters);
    }

    public interface ParseOperator<T> {
        bool CanParse(Type type, TypedValue instance, Tree<T> parameters);
        TypedValue Parse(Type type, TypedValue instance, Tree<T> parameters);
    }

    public interface RuntimeOperator<T> {
        bool CanCreate(string memberName, Tree<T> parameters);
        TypedValue Create(string memberName, Tree<T> parameters);
        bool CanInvoke(TypedValue instance, string memberName, Tree<T> parameters);
        TypedValue Invoke(TypedValue instance, string memberName, Tree<T> parameters);
    }
}