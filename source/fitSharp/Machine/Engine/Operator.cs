// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
//using System.Diagnostics.Contracts;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public abstract class Operator<T, P> where P: class, Processor<T> {
        public P Processor { get; set; }
    }

    public interface CompareOperator<T> {
        bool CanCompare(TypedValue actual, Tree<T> expected);
        bool Compare(TypedValue actual, Tree<T> expected);
    }

    public interface ComposeOperator<T> {
        bool CanCompose(TypedValue instance);
        Tree<T> Compose(TypedValue instance);
    }

    //[ContractClass(typeof(ParseOperatorContract<>))]
    public interface ParseOperator<T> {
        bool CanParse(Type type, TypedValue instance, Tree<T> parameters);
        TypedValue Parse(Type type, TypedValue instance, Tree<T> parameters);
    }

    /*[ContractClassFor(typeof(ParseOperator<>))] abstract class ParseOperatorContract<T>: ParseOperator<T> {
        public bool CanParse(Type type, TypedValue instance, Tree<T> parameters) {
            return false;
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<T> parameters) {
            Contract.Requires(type != null);
            return TypedValue.Void;
        }
    }*/

    public interface CreateOperator<T> {
        bool CanCreate(string memberName, Tree<T> parameters);
        TypedValue Create(string memberName, Tree<T> parameters);
    }

    public interface InvokeOperator<T> {
        bool CanInvoke(TypedValue instance, string memberName, Tree<T> parameters);
        TypedValue Invoke(TypedValue instance, string memberName, Tree<T> parameters);
    }
}