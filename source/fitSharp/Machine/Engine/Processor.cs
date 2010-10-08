// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {

    public interface Processor<T> {
        Configuration Configuration { get; }
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
        void Clear<V>();
    }

    public static class ProcessorExtension {

        public static Tree<T> Compose<T>(this Processor<T> processor, object instance) {
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

        public static TypedValue Invoke<T>(this Processor<T> processor, object instance, string memberName, Tree<T> parameters) {
            return processor.Invoke(new TypedValue(instance), memberName, parameters);
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

    public abstract class ProcessorBase<T, P>: Processor<T> where P: class, Processor<T> {
        protected abstract Operators<T,P> Operators { get; }

        public Configuration Configuration { get; protected set; }

        public ApplicationUnderTest ApplicationUnderTest { get { return Configuration.GetItem<ApplicationUnderTest>(); } }
        private Memory MemoryBanks { get { return Configuration.GetItem<Memory>(); } }

        protected ProcessorBase(Configuration configuration) {
            Configuration = configuration;
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

        public virtual TypedValue Parse(Type type, TypedValue instance, Tree<T> parameters) {
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

        public void AddMemory<V>() { MemoryBanks.Add<V>(); }

        public void Store<V>(V newItem) { MemoryBanks.Store(newItem); }

        public V Load<V>(V matchItem) { return MemoryBanks.Load(matchItem); }

        public bool Contains<V>(V matchItem) { return MemoryBanks.Contains(matchItem); }

        public void Clear<V>() { MemoryBanks.Clear<V>(); }
    }
}