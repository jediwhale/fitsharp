// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public delegate bool CanDoOperation<T>(T anOperator);
    public delegate void DoOperation<T>(T anOperator);
    public class Operators<T, P> where P: Processor<T> {
        private readonly P processor;
        private readonly List<List<Operator<T, P>>> operators = new List<List<Operator<T, P>>>();

        public Operators(P processor) {
            this.processor = processor;
            Add(new DefaultParse<T,P>());
            Add(new ParseType<T,P>());
            Add(new DefaultRuntime<T,P>());
        }

        public void Add(string operatorName) { Add((Operator<T, P>)processor.Create(operatorName, new TreeList<T>()).Value); }

        public void Add(Operator<T, P> anOperator) { Add(anOperator, 0); }

        public void Add(Operator<T, P> anOperator, int priority) {
            while (operators.Count <= priority) operators.Add(new List<Operator<T, P>>());
            anOperator.Processor = processor;
            operators[priority].Add(anOperator);
        }

        public void Copy(Operators<T,P> from) {
            operators.Clear();
            for (int priority = 0; priority < from.operators.Count; priority++) {
                foreach (Operator<T, P> anOperator in from.operators[priority]) {
                    Add((Operator<T, P>)Activator.CreateInstance(anOperator.GetType()), priority);
                }
            }
        }

        public void Remove(string operatorName) {
            foreach (List<Operator<T, P>> list in operators)
                foreach (Operator<T, P> item in list)
                    if (item.GetType().FullName == operatorName) {
                        list.Remove(item);
                        return;
                    }
        }

        public void Do<O>(CanDoOperation<O> canDoOperation, DoOperation<O> doOperation) where O: class {
            for (int priority = operators.Count - 1; priority >= 0; priority--) {
                for (int i = operators[priority].Count - 1; i >= 0; i--) {
                    var candidate = operators[priority][i] as O;
                    if (candidate == null || !canDoOperation(candidate)) continue;
                    doOperation(candidate);
                    return;
                }
            }
            throw new ApplicationException(string.Format("No default for {0}", typeof(T).Name));
        }
    }
}
