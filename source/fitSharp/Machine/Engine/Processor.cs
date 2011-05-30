// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public interface Processor<T> {
        Configuration Configuration { get; }
        ApplicationUnderTest ApplicationUnderTest { get; }
        void AddNamespace(string namespaceName);
        void AddOperator(string operatorName);
        TypedValue Create(string memberName, Tree<T> parameters);
        bool Compare(TypedValue instance, Tree<T> parameters);
        Tree<T> Compose(TypedValue instance);
        bool Contains<V>(V matchItem);
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

        public static TypedValue InvokeWithThrow<T>(this Processor<T> processor, TypedValue instance, string memberName,
                                                    Tree<T> parameters) {
            TypedValue result = processor.Invoke(instance, memberName, parameters);
            result.ThrowExceptionIfNotValid();
            return result;
        }

        public static TypedValue Invoke<T>(this Processor<T> processor, object instance, string memberName,
                                           Tree<T> parameters) {
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
            return processor.ParseTree(type, processor.Compose(new TypedValue(input, typeof (string))));
        }

        public static V ParseString<T, V>(this Processor<T> processor, string input) {
            return processor.ParseString(typeof (V), input).GetValue<V>();
        }
    }

    public abstract class ProcessorBase<T, P> : Processor<T> where P : class, Processor<T> {
        protected abstract Operators<T, P> Operators { get; }

        public Configuration Configuration { get; protected set; }

        public ApplicationUnderTest ApplicationUnderTest {
            get { return Configuration.GetItem<ApplicationUnderTest>(); }
        }

        private Memory MemoryBanks {
            get { return Configuration.GetItem<Memory>(); }
        }

        protected ProcessorBase(Configuration configuration) {
            Configuration = configuration;
        }

        public void AddOperator(string operatorName) {
            Operators.Add(operatorName);
        }

        public void AddOperator(Operator<T, P> anOperator) {
            Operators.Add(anOperator);
        }

        public void AddOperator(Operator<T, P> anOperator, int priority) {
            Operators.Add(anOperator, priority);
        }

        public void RemoveOperator(string operatorName) {
            Operators.Remove(operatorName);
        }

        public void AddNamespace(string namespaceName) {
            ApplicationUnderTest.AddNamespace(namespaceName);
        }

        public bool Compare(TypedValue instance, Tree<T> parameters) {
            return DoLoggedOperation(
                string.Format("compare {0}", instance.ValueString),
                logging => {
                    bool result = false;
                    Operators.Do<CompareOperator<T>>(
                        o => o.CanCompare(instance, parameters),
                        o => {
                            result = o.Compare(instance, parameters);
                            logging.Write(string.Format(" by {0} = {1}", o.GetType(), result));
                        });
                    return result;
                });
        }


        public Tree<T> Compose(TypedValue instance) {
            return DoLoggedOperation(
                string.Format("compose {0}", instance.ValueString),
                logging => {
                    Tree<T> result = null;
                    Operators.Do<ComposeOperator<T>>(
                        o => o.CanCompose(instance),
                        o => {
                            result = o.Compose(instance);
                            logging.Write(string.Format(" by {0}", o.GetType()));
                        });
                    return result;
                });
        }

        public virtual TypedValue Parse(Type type, TypedValue instance, Tree<T> parameters) {
            return DoLoggedOperation(
                string.Format("parse {0}", type),
                logging => {
                    var result = TypedValue.Void;
                    Operators.Do<ParseOperator<T>>(
                        o => o.CanParse(type, instance, parameters),
                        o => {
                            result = o.Parse(type, instance, parameters);
                            logging.LogResult(o, result);
                        });
                    return result;
                });
        }

        public TypedValue Create(string memberName, Tree<T> parameters) {
            return DoLoggedOperation(
                string.Format("create {0}", memberName),
                logging => {
                    var result = TypedValue.Void;
                        Operators.Do<CreateOperator<T>>(
                            o => o.CanCreate(memberName, parameters),
                            o => {
                                result = o.Create(memberName, parameters);
                                logging.LogResult(o, result);
                            });
                        return result;
                });
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<T> parameters) {
            return DoLoggedOperation(
                instance.Type != typeof (Logging)
                    ? string.Format("invoke {0} {1}", instance.ValueString, memberName)
                    : string.Empty,
                logging => {
                    var result = TypedValue.Void;
                    Operators.Do<InvokeOperator<T>>(
                        o => o.CanInvoke(instance, memberName, parameters),
                        o => {
                            result = o.Invoke(instance, memberName, parameters);
                            logging.LogResult(o, result);
                        });
                    return result;
                });
        }

        public void AddMemory<V>() {
            MemoryBanks.Add<V>();
        }

        public void Store<V>(V newItem) {
            MemoryBanks.Store(newItem);
        }

        public V Load<V>(V matchItem) {
            return MemoryBanks.Load(matchItem);
        }

        public bool Contains<V>(V matchItem) {
            return MemoryBanks.Contains(matchItem);
        }

        public void Clear<V>() {
            MemoryBanks.Clear<V>();
        }

        R DoLoggedOperation<R>(string startMessage, Func<OperationLogging, R> operation) {
            var logging = new OperationLogging(Configuration);
            try {
                logging.Start(startMessage);
                return operation(logging);
            }
            finally {
                logging.End();
            }
        }

        class OperationLogging {
            readonly Configuration configuration;
            Logging logging;

            public OperationLogging(Configuration configuration) {
                this.configuration = configuration;
            }

            public void Start(string message) {
                if (message.Length <= 0) return;
                logging = configuration.GetItem<Logging>();
                logging.StartWrite(message);
            }

            public void LogResult(object o, TypedValue result) {
                Write(string.Format(" by {0} = {1}", o.GetType(), result.ValueString));
            }

            public void Write(string message) {
                if (logging != null) logging.Write(message);
            }

            public void End() {
                if (logging != null) logging.EndWrite(string.Empty);
            }
        }
    }
}