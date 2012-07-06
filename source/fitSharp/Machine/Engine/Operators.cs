// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Linq;

namespace fitSharp.Machine.Engine {
    public delegate bool CanDoOperation<in T>(T anOperator);
    public delegate void DoOperation<in T>(T anOperator);
    public class Operators<T, P> where P: class, Processor<T> {
        private readonly List<List<Operator<T, P>>> operators = new List<List<Operator<T, P>>>();
        protected readonly Memory createMemory = new TypeDictionary();

        public Operators() {
            Add(new InvokeDefault<T,P>(), 0);
            Add(new FindMemberDefault<T,P>(), 0);
            Add(new CreateDefault<T,P>(), 0);
        }

        public Operators(P processor): this() {
            Processor = processor;
        }

        public P Processor { private get; set; }

        public Operator<T, P> Add(string operatorName) {
            return Add(CreateOperator(operatorName));
        }

        public Operator<T, P> AddFirst(string operatorName) {
            // Add an operator at the current highest priority
            int priority = (operators.Count == 0) ? 0 : operators.Count - 1;
            return Add(CreateOperator(operatorName), priority);
        }

        public Operator<T, P> Add(Operator<T, P> anOperator) { return Add(anOperator, 1); }

        public Operator<T, P> Add(Operator<T, P> anOperator, int priority) {
            AddPriorityList(priority);
            operators[priority].Add(anOperator);
            return anOperator;
        }

        private Operator<T, P> CreateOperator(string operatorName) {
            return (Operator<T, P>)(Processor == null ? new BasicProcessor(createMemory).Create(operatorName).Value : Processor.Create(operatorName).Value);
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
            foreach (var candidate in Values<O>().Where(candidate => canDoOperation(candidate))) {
                doOperation(candidate);
                return;
            }
            throw new ApplicationException(string.Format("No default for {0}", typeof(T).Name));
        }

        public O FindOperator<O>(object[] parameters) where O: class {
            var operationType = typeof (O).Name;
            var operationName = operationType.Substring(0, operationType.IndexOf("Operator"));
            foreach (var candidate in
                    Values<O>().Where(candidate => CanDoOperation(candidate, operationName, parameters))) {
                return candidate;
            }
            throw new ApplicationException(string.Format("No default for {0}", typeof (O).Name));
        }

        static bool CanDoOperation<O>(O candidate, string operationName, object[] parameters) {
            var member = MemberQuery.GetDirectInstance(candidate,
                    new MemberSpecification("Can" + operationName, parameters.Length));
            return member.Invoke(parameters).GetValue<bool>();
        }

        IEnumerable<O> Values<O>() where O: class {
            for (var priority = operators.Count - 1; priority >= 0; priority--) {
                for (var i = operators[priority].Count - 1; i >= 0; i--) {
                    var anOperator = operators[priority][i];
                    var candidate = anOperator as O;
                    if (candidate == null) continue;
                    anOperator.Processor = Processor;
                    yield return candidate;
                }
            }
        }
    }
}
