// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {

    public interface Processor<T> {
        ApplicationUnderTest ApplicationUnderTest { get;}
        TypedValue Create(string memberName, Tree<T> parameters);
        bool Compare(TypedValue instance, Tree<T> parameters);
        Tree<T> Compose(TypedValue instance);
        TypedValue Parse(Type type, TypedValue instance, Tree<T> parameters);
    }


    public class ProcessorImpl<T, P>: Processor<T>, Copyable where P: ProcessorImpl<T, P> {
        //todo: add setup and teardown?
        //todo: this is turning into a facade so push everything else out?

        public ApplicationUnderTest ApplicationUnderTest { get; set;}
        private readonly Operators<T,P> operators;
        private readonly Dictionary<Type, object> memoryBanks = new Dictionary<Type, object>();

        public ProcessorImpl(ApplicationUnderTest applicationUnderTest) {
            ApplicationUnderTest = applicationUnderTest;
            operators = new Operators<T,P>((P)this);
        }

        public ProcessorImpl(): this(new ApplicationUnderTest()) {}

        public ProcessorImpl(ProcessorImpl<T,P> other): this(new ApplicationUnderTest(other.ApplicationUnderTest)) {
            operators.Copy(other.operators);
            memoryBanks = new Dictionary<Type, object>(other.memoryBanks);
        }

        public void AddOperator(string operatorName) { operators.Add(operatorName); }
        public void AddOperator(Operator<T, P> anOperator) { operators.Add(anOperator); }
        public void AddOperator(Operator<T, P> anOperator, int priority) { operators.Add(anOperator, priority); }
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
            return new ProcessorImpl<T,P>(this);
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