// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class Processor<U>: Copyable { //todo: add setup and teardown
        //todo: this is turning into a facade so push everything else out

        //todo: make this a class
        private readonly List<List<Operator<U>>> operators = new List<List<Operator<U>>>();

        private readonly Dictionary<Type, object> memoryBanks = new Dictionary<Type, object>();

        public ApplicationUnderTest ApplicationUnderTest { get; set; }

        public Processor(ApplicationUnderTest applicationUnderTest) {
            ApplicationUnderTest = applicationUnderTest;
            AddOperator(new DefaultParse<U>());
            AddOperator(new ParseType<U>());
            AddOperator(new DefaultRuntime<U>());
        }

        public Processor(): this(new ApplicationUnderTest()) {}

        public Processor(Processor<U> other): this(new ApplicationUnderTest(other.ApplicationUnderTest)) {
            operators.Clear();
            for (int priority = 0; priority < other.operators.Count; priority++) {
                foreach (Operator<U> anOperator in other.operators[priority]) {
                    AddOperator((Operator<U>)Activator.CreateInstance(anOperator.GetType()), priority);
                }
            }
            memoryBanks = new Dictionary<Type, object>(other.memoryBanks);
        }

        public void AddOperator(string operatorName) {
            AddOperator((Operator<U>)Create(operatorName).Value);
        }

        public void AddOperator(Operator<U> anOperator) { AddOperator(anOperator, 0); }

        public void AddOperator(Operator<U> anOperator, int priority) {
            while (operators.Count <= priority) operators.Add(new List<Operator<U>>());
            anOperator.Processor = this;
            operators[priority].Add(anOperator);
        }

        public void RemoveOperator(string operatorName) {
            foreach (List<Operator<U>> list in operators)
                foreach (Operator<U> item in list)
                    if (item.GetType().FullName == operatorName) {
                        list.Remove(item);
                        return;
                    }
        }

        public void AddNamespace(string namespaceName) {
            ApplicationUnderTest.AddNamespace(namespaceName);
        }

        public bool Compare(TypedValue instance, Tree<U> parameters) {
            bool result = false;
            Do<CompareOperator<U>>(
                o => o.CanCompare(instance, parameters),
                o => result = o.Compare(instance, parameters));
            return result;
        }

        public Tree<U> Compose(object instance) {
            return Compose(new TypedValue(instance));
        }

        public Tree<U> Compose(TypedValue instance) {
            Tree<U> result = null;
            Do<ComposeOperator<U>>(
                o => o.CanCompose(instance),
                o => result = o.Compose(instance));
            return result;
        }

        public TypedValue Execute(TypedValue instance, Tree<U> parameters) {
            TypedValue result = TypedValue.Void;
            Do<ExecuteOperator<U>>(
                o => o.CanExecute(instance, parameters),
                o => result = o.Execute(instance, parameters));
            return result;
        }

        public TypedValue Execute(Tree<U> parameters) {
            return Execute(TypedValue.Void, parameters);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<U> parameters) {
            TypedValue result = TypedValue.Void;
            Do<ParseOperator<U>>(
                o => o.CanParse(type, instance, parameters),
                o => result = o.Parse(type, instance, parameters));
            return result;
        }

        public TypedValue Parse(Type type, Tree<U> parameters) {
            return Parse(type, TypedValue.Void, parameters);
        }

        public TypedValue Parse(Type type, U input) {
            return Parse(type, new TreeLeaf<U>(input));
        }

        public T ParseTree<T>(Tree<U> input) {
            return (T) Parse(typeof (T), input).Value;
        }

        public T Parse<T>(U input) {
            return (T) Parse(typeof (T), input).Value;
        }

        public TypedValue ParseString(Type type, string input) {
            return Parse(type, Compose(new TypedValue(input, typeof(string))));
        }

        public T ParseString<T>(string input) {
            return (T) ParseString(typeof (T), input).Value;
        }

        public TypedValue Create(string membername) {
            return Create(membername, new TreeList<U>());
        }

        public TypedValue Create(string memberName, Tree<U> parameters) {
            TypedValue result = TypedValue.Void;
            Do<RuntimeOperator<U>>(
                o => o.CanCreate(memberName, parameters),
                o => result = o.Create(memberName, parameters));
            return result;
        }

        public TypedValue TryInvoke(TypedValue instance, string memberName, Tree<U> parameters) {
            TypedValue result = TypedValue.Void;
            Do<RuntimeOperator<U>>(
                o => o.CanInvoke(instance, memberName, parameters),
                o => result = o.Invoke(instance, memberName, parameters));
            return result;
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<U> parameters) {
            TypedValue result = TryInvoke(instance, memberName, parameters);
            result.ThrowExceptionIfNotValid();
            return result;
        }

        public delegate bool CanDoOperation<T>(T anOperator);
        public delegate void DoOperation<T>(T anOperator);

        public void Do<T>(CanDoOperation<T> canDoOperation, DoOperation<T> doOperation) where T: class {
            for (int priority = operators.Count - 1; priority >= 0; priority--) {
                for (int i = operators[priority].Count - 1; i >= 0; i--) {
                    var candidate = operators[priority][i] as T;
                    if (candidate == null || !canDoOperation(candidate)) continue;
                    doOperation(candidate);
                    return;
                }
            }
            throw new ApplicationException(string.Format("No default for {0}", typeof(T).Name));
        }

        Copyable Copyable.Copy() {
            return new Processor<U>(this);
        }

        public void AddMemory<T>() {
            memoryBanks[typeof (T)] = new List<T>();
        }

        public void Store<T>(T newItem) {
            List<T> memory = GetMemory<T>();
            foreach (T item in memory) {
                if (!newItem.Equals(item)) continue;
                memory.Remove(item);
                break;
            }
            memory.Add(newItem);
        }

        public T Load<T>(T matchItem) {
            foreach (T item in GetMemory<T>()) {
                if (matchItem.Equals(item)) return item;
            }
            throw new MemoryMissingException<T>(matchItem);
        }

        public bool Contains<T>(T matchItem) {
            foreach (T item in GetMemory<T>()) {
                if (matchItem.Equals(item)) return true;
            }
            return false;
        }

        public void Clear<T>() {
            GetMemory<T>().Clear();
        }

        private List<T> GetMemory<T>() { return (List<T>) memoryBanks[typeof (T)];}
    }
}