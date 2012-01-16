// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public interface Processor<T> {
        Memory Memory { get; }
        Configuration Configuration { get; }

        ApplicationUnderTest ApplicationUnderTest { get; }
        void AddNamespace(string namespaceName);

        void AddOperator(string operatorName);
        void RemoveOperator(string operatorName);

        bool Compare(TypedValue instance, Tree<T> parameters);
        Tree<T> Compose(TypedValue instance);
        TypedValue Parse(Type type, TypedValue instance, Tree<T> parameters);
        TypedValue Invoke(TypedValue instance, MemberName memberName, Tree<T> parameters);

        TypedValue Operate<O>(params object[] parameters) where O : class;
    }

    public static class ProcessorExtension {
        public static Tree<T> Compose<T>(this Processor<T> processor, object instance) {
            return processor.Compose(new TypedValue(instance));
        }

        public static TypedValue Create<T>(this Processor<T> processor, string membername) {
            return processor.Create(membername, new TreeList<T>());
        }

        public static TypedValue InvokeWithThrow<T>(this Processor<T> processor, TypedValue instance, MemberName memberName,
                                                    Tree<T> parameters) {
            TypedValue result = processor.Invoke(instance, memberName, parameters);
            result.ThrowExceptionIfNotValid();
            return result;
        }

        public static TypedValue Invoke<T>(this Processor<T> processor, object instance, MemberName memberName,
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

        public static TypedValue Create<T>(this Processor<T> processor, string memberName, Tree<T> parameters) {
            return processor.Operate<CreateOperator<T>>(memberName, parameters);
        }
    }

    public abstract class ProcessorBase<T, P> : Processor<T> where P : class, Processor<T> {
        protected abstract Operators<T, P> Operators { get; }

        public Configuration Configuration {
            get { return (Configuration)Memory; }
        }

        public Memory Memory { get; protected set; }

        public ApplicationUnderTest ApplicationUnderTest {
            get { return Memory.GetItem<ApplicationUnderTest>(); }
        }

        protected ProcessorBase(Memory memory) {
            Memory = memory;
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

        public TypedValue Operate<O>(params object[] parameters) where O: class {
            var operationType = typeof (O).Name;
            var operationName = operationType.Substring(0, operationType.IndexOf("Operator"));
            var logging = new OperationLogging(Memory);
            try {
                logging.Start(operationName);
                logging.LogParameters(parameters);
                var candidate = Operators.FindOperator<O>(parameters);
                var member = RuntimeType.FindDirectInstance(candidate, new MemberName(operationName), parameters.Length);
                var result = member.Invoke(parameters).GetValue<TypedValue>();
                logging.LogResult(candidate, result);
                return result;
            }
            finally {
                logging.End();
            }
        }


        public TypedValue Invoke(TypedValue instance, MemberName memberName, Tree<T> parameters) {
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

        R DoLoggedOperation<R>(string startMessage, Func<OperationLogging, R> operation) {
            var logging = new OperationLogging(Memory);
            try {
                logging.Start(startMessage);
                return operation(logging);
            }
            finally {
                logging.End();
            }
        }

        class OperationLogging {
            readonly Memory memory;
            Logging logging;

            public OperationLogging(Memory memory) {
                this.memory = memory;
            }

            public void Start(string message) {
                if (message.Length <= 0) return;
                logging = memory.GetItem<Logging>();
                logging.StartWrite(message);
            }

            public void LogParameters(IEnumerable<object> parameters) {
                foreach (var parameter in parameters) {
                    if (parameter != null) logging.Write(" " + parameter);
                }
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