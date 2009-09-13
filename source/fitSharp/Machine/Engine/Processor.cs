// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class Processor<T, P>: Copyable where P: Processor<T, P> {
        //todo: add setup and teardown?
        //todo: this is turning into a facade so push everything else out?

        private readonly Operators<T,P> operators;
        private readonly Dictionary<Type, object> memoryBanks = new Dictionary<Type, object>();

        public ApplicationUnderTest ApplicationUnderTest { get; set; }

        public Processor(ApplicationUnderTest applicationUnderTest) {
            ApplicationUnderTest = applicationUnderTest;
            operators = new Operators<T,P>((P)this);
        }

        public Processor(): this(new ApplicationUnderTest()) {}

        public Processor(Processor<T,P> other): this(new ApplicationUnderTest(other.ApplicationUnderTest)) {
            operators.Copy(other.operators);
            memoryBanks = new Dictionary<Type, object>(other.memoryBanks);
        }

        public void AddOperator(string operatorName) { operators.Add(operatorName); }
        public void AddOperator(Operator<P> anOperator) { operators.Add(anOperator); }
        public void AddOperator(Operator<P> anOperator, int priority) { operators.Add(anOperator, priority); }
        public void RemoveOperator(string operatorName) { operators.Remove(operatorName); }

        public void AddNamespace(string namespaceName) {
            ApplicationUnderTest.AddNamespace(namespaceName);
        }

        public bool Compare(TypedValue instance, Tree<T> parameters) {
            bool result = false;
            operators.Do<CompareOperator<T>>(
                o => o.CanCompare(instance, parameters),
                o => result = o.Compare(instance, parameters));
            return result;
        }

        public Tree<T> Compose(object instance) {
            return Compose(new TypedValue(instance));
        }

        public Tree<T> Compose(TypedValue instance) {
            Tree<T> result = null;
            operators.Do<ComposeOperator<T>>(
                o => o.CanCompose(instance),
                o => result = o.Compose(instance));
            return result;
        }

        public TypedValue Execute(TypedValue instance, Tree<T> parameters) {
            TypedValue result = TypedValue.Void;
            operators.Do<ExecuteOperator<T>>(
                o => o.CanExecute(instance, parameters),
                o => result = o.Execute(instance, parameters));
            return result;
        }

        public TypedValue Execute(Tree<T> parameters) {
            return Execute(TypedValue.Void, parameters);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<T> parameters) {
            TypedValue result = TypedValue.Void;
            operators.Do<ParseOperator<T>>(
                o => o.CanParse(type, instance, parameters),
                o => result = o.Parse(type, instance, parameters));
            return result;
        }

        public TypedValue Parse(Type type, Tree<T> parameters) {
            return Parse(type, TypedValue.Void, parameters);
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
            return Parse(type, Compose(new TypedValue(input, typeof(string))));
        }

        public V ParseString<V>(string input) {
            return (V) ParseString(typeof (V), input).Value;
        }

        public TypedValue Create(string membername) {
            return Create(membername, new TreeList<T>());
        }

        public TypedValue Create(string memberName, Tree<T> parameters) {
            TypedValue result = TypedValue.Void;
            operators.Do<RuntimeOperator<T>>(
                o => o.CanCreate(memberName, parameters),
                o => result = o.Create(memberName, parameters));
            return result;
        }

        public TypedValue TryInvoke(TypedValue instance, string memberName, Tree<T> parameters) {
            TypedValue result = TypedValue.Void;
            operators.Do<RuntimeOperator<T>>(
                o => o.CanInvoke(instance, memberName, parameters),
                o => result = o.Invoke(instance, memberName, parameters));
            return result;
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<T> parameters) {
            TypedValue result = TryInvoke(instance, memberName, parameters);
            result.ThrowExceptionIfNotValid();
            return result;
        }

        Copyable Copyable.Copy() {
            return new Processor<T,P>(this);
        }

        public void AddMemory<V>() {
            memoryBanks[typeof (V)] = new List<V>();
        }

        public void Store<V>(V newItem) {
            List<V> memory = GetMemory<V>();
            foreach (V item in memory) {
                if (!newItem.Equals(item)) continue;
                memory.Remove(item);
                break;
            }
            memory.Add(newItem);
        }

        public V Load<V>(V matchItem) {
            foreach (V item in GetMemory<V>()) {
                if (matchItem.Equals(item)) return item;
            }
            throw new MemoryMissingException<V>(matchItem);
        }

        public bool Contains<V>(V matchItem) {
            foreach (V item in GetMemory<V>()) {
                if (matchItem.Equals(item)) return true;
            }
            return false;
        }

        public void Clear<V>() {
            GetMemory<V>().Clear();
        }

        private List<V> GetMemory<V>() { return (List<V>) memoryBanks[typeof (V)];}
    }
}