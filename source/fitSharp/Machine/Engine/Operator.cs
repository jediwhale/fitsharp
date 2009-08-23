// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Model;

// don't want to put processor in constructor? why not? could create the list initially with processor references
// then still reusable

namespace fitSharp.Machine.Engine {
    public interface Operator {}

    public interface CompareOperator<T>: Operator {
        bool CanCompare(Processor<T> processor, TypedValue actual, Tree<T> expected);
        bool Compare(Processor<T> processor, TypedValue actual, Tree<T> expected);
    }

    public interface ComposeOperator<T>: Operator {
        bool TryCompose(Processor<T> processor, TypedValue instance, ref Tree<T> result);
    }

    public interface ExecuteOperator<T>: Operator {
        bool IsMatch(Processor<T> processor, TypedValue instance, Tree<T> parameters);
        TypedValue Execute(Processor<T> processor, TypedValue instance, Tree<T> parameters);
    }

    public interface ParseOperator<T>: Operator {
        bool TryParse(Processor<T> processor, Type type, TypedValue instance, Tree<T> parameters, ref TypedValue result);
    }

    public interface RuntimeOperator<T>: Operator {
        bool TryCreate(Processor<T> processor, string memberName, Tree<T> parameters, ref TypedValue result);
        bool TryInvoke(Processor<T> processor, TypedValue instance, string memberName, Tree<T> parameters, ref TypedValue result);
    }
}