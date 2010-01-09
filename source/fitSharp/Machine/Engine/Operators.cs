// Copyright © 2009,2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Application;

namespace fitSharp.Machine.Engine {
    public delegate bool CanDoOperation<T>(T anOperator);
    public delegate void DoOperation<T>(T anOperator);
    public class Operators<T, P> where P: class, Processor<T> {
        private readonly List<List<Operator<T, P>>> operators = new List<List<Operator<T, P>>>();
        protected readonly Configuration createConfiguration = new Configuration();

        public Operators() {
            Add(new DefaultRuntime<T,P>(), 0);
        }

        public Operators(P processor): this() {
            Processor = processor;
        }

        public P Processor { private get; set; }

        public Operator<T, P> Add(string operatorName) {
            return Add((Operator<T, P>)(Processor == null ? new BasicProcessor(createConfiguration).Create(operatorName).Value : Processor.Create(operatorName).Value));
        }

        public Operator<T, P> Add(Operator<T, P> anOperator) { return Add(anOperator, 1); }

        public Operator<T, P> Add(Operator<T, P> anOperator, int priority) {
            AddPriorityList(priority);
            operators[priority].Add(anOperator);
            return anOperator;
        }

        private void AddPriorityList(int priority) {
            while (operators.Count <= priority) operators.Add(new List<Operator<T, P>>());
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
                    Operator<T, P> anOperator = operators[priority][i];
                    anOperator.Processor = Processor;
                    var candidate = anOperator as O;
                    if (candidate == null) continue;
                    if (!canDoOperation(candidate)) continue;
                    doOperation(candidate);
                    return;
                }
            }
            throw new ApplicationException(string.Format("No default for {0}", typeof(T).Name));
        }
    }
}
