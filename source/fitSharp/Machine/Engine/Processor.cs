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
        void AddNamespace(string namespaceName);
        void AddOperator(string operatorName);
        TypedValue Create(string memberName, Tree<T> parameters);
        bool Compare(TypedValue instance, Tree<T> parameters);
        Tree<T> Compose(TypedValue instance);
        bool Contains<V>(V matchItem);
        TypedValue Execute(TypedValue instance, Tree<T> parameters);
        TypedValue Invoke(TypedValue instance, string memberName, Tree<T> parameters);
        V Load<V>(V matchItem);
        TypedValue Parse(Type type, TypedValue instance, Tree<T> parameters);
        void RemoveOperator(string operatorName);
        void Store<V>(V newItem);
    }

    public static class ProcessorExtension {

        public static Tree<T> Compose<T>(this Processor<T> processor, object instance)  {
            return processor.Compose(new TypedValue(instance));
        }

        public static TypedValue Create<T>(this Processor<T> processor, string membername) {
            return processor.Create(membername, new TreeList<T>());
        }

        public static TypedValue Execute<T>(this Processor<T> processor, Tree<T> parameters) {
            return processor.Execute(TypedValue.Void, parameters);
        }

        public static TypedValue InvokeWithThrow<T>(this Processor<T> processor, TypedValue instance, string memberName, Tree<T> parameters) {
            TypedValue result = processor.Invoke(instance, memberName, parameters);
            result.ThrowExceptionIfNotValid();
            return result;
        }
        public static TypedValue ParseTree<T>(this Processor<T> processor, Type type, Tree<T> parameters) {
            return processor.Parse(type, TypedValue.Void, parameters);
        }

        public static TypedValue Parse<T>(this Processor<T> processor, Type type, T input) {
            return processor.ParseTree(type, new TreeList<T>(input));
        }

        public static V ParseTree<T, V>(this Processor<T> processor, Tree<T> input) {
            return processor.ParseTree(typeof (V), input).GetValue<V>();
        }

        public static V Parse<T, V>(this Processor<T> processor, T input) {
            return processor.Parse(typeof (V), input).GetValue<V>();
        }

        public static TypedValue ParseString<T>(this Processor<T> processor, Type type, string input) {
            return processor.ParseTree(type, processor.Compose(new TypedValue(input, typeof(string))));
        }

        public static V ParseString<T, V>(this Processor<T> processor, string input) {
            return processor.ParseString(typeof (V), input).GetValue<V>();
        }
    }

    public abstract class ProcessorBase<T, P>: Processor<T> where P: Processor<T> {
        protected abstract Operators<T,P> Operators { get; }

        public ApplicationUnderTest ApplicationUnderTest { get; set;}
        private readonly Dictionary<Type, object> memoryBanks = new Dictionary<Type, object>();

        protected ProcessorBase(ApplicationUnderTest applicationUnderTest) {
            ApplicationUnderTest = applicationUnderTest;
        }

        protected ProcessorBase(): this(new ApplicationUnderTest()) {}

        protected ProcessorBase(ProcessorBase<T,P> other): this(new ApplicationUnderTest(other.ApplicationUnderTest)) {
            memoryBanks = new Dictionary<Type, object>(other.memoryBanks);
        }

        public void AddOperator(string operatorName) { Operators.Add(operatorName); }
        public void AddOperator(Operator<T, P> anOperator) { Operators.Add(anOperator); }
        public void AddOperator(Operator<T, P> anOperator, int priority) { Operators.Add(anOperator, priority); }
        public void RemoveOperator(string operatorName) { Operators.Remove(operatorName); }

        public void AddNamespace(string namespaceName) {
            ApplicationUnderTest.AddNamespace(namespaceName);
        }

        public bool Compare(TypedValue instance, Tree<T> parameters) {
            bool result = false;
            Operators.Do<CompareOperator<T>>(
                o => o.CanCompare(instance, parameters),
                o => result = o.Compare(instance, parameters));
            return result;
        }

        public Tree<T> Compose(TypedValue instance) {
            Tree<T> result = null;
            Operators.Do<ComposeOperator<T>>(
                o => o.CanCompose(instance),
                o => result = o.Compose(instance));
            return result;
        }

        public TypedValue Execute(TypedValue instance, Tree<T> parameters) {
            TypedValue result = TypedValue.Void;
            Operators.Do<ExecuteOperator<T>>(
                o => o.CanExecute(instance, parameters),
                o => result = o.Execute(instance, parameters));
            return result;
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<T> parameters) {
            TypedValue result = TypedValue.Void;
            Operators.Do<ParseOperator<T>>(
                o => o.CanParse(type, instance, parameters),
                o => result = o.Parse(type, instance, parameters));
            return result;
        }

        public TypedValue Create(string memberName, Tree<T> parameters) {
            TypedValue result = TypedValue.Void;
            Operators.Do<RuntimeOperator<T>>(
                o => o.CanCreate(memberName, parameters),
                o => result = o.Create(memberName, parameters));
            return result;
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<T> parameters) {
            TypedValue result = TypedValue.Void;
            Operators.Do<RuntimeOperator<T>>(
                o => o.CanInvoke(instance, memberName, parameters),
                o => result = o.Invoke(instance, memberName, parameters));
            return result;
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