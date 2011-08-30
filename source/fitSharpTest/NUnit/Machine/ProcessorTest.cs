// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Test.Double;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class ProcessorTest {

        private static readonly DefaultTest defaultTest = new DefaultTest();
        private static readonly SpecificTest specificTestA = new SpecificTest("A");
        private static readonly SpecificTest specificTestB = new SpecificTest("B");

        private BasicProcessor processor;

        [SetUp] public void SetUp() {
            processor = new BasicProcessor();
        }

        [Test] public void DefaultOperatorIsFound() {
            processor.AddOperator(defaultTest);
            object result = Execute();
            Assert.AreEqual("defaultexecute", result.ToString());
        }

        private object Execute() {
            return processor.Invoke(TypedValue.Void, string.Empty, new TreeList<string>()).Value;
        }

        [Test] public void SpecificOperatorIsFound() {
            processor.AddOperator(defaultTest);
            processor.AddOperator(specificTestA);
            processor.AddOperator(specificTestB);
            object result = Execute("A");
            Assert.AreEqual("executeA", result.ToString());
        }

        private object Execute(string parameter) {
            return processor.Invoke(TypedValue.Void, string.Empty, new TreeList<string>(parameter)).Value;
        }

        [Test] public void TypeIsCreated() {
            TypedValue result = processor.Create(typeof(SampleClass).FullName, new TreeList<string>());
            Assert.IsTrue(result.Value is SampleClass);
        }

        [Test] public void MethodIsInvoked() {
            var instance = new TypedValue(new SampleClass());
            TypedValue result = processor.Invoke(instance, "methodnoparms", new TreeList<string>());
            Assert.AreEqual("samplereturn", result.Value);
        }

        [Test] public void MethodWithParameterIsInvoked() {
            var instance = new TypedValue(new SampleClass());
            TypedValue result = processor.Invoke(instance, "MethodWithParms", new TreeList<string>().AddBranchValue("stringparm0"));
            Assert.AreEqual("samplestringparm0", result.Value);
        }

        [Test] public void ExceptionReturnedAsValue() {
            var instance = new TypedValue(new SampleClass());
            TypedValue result = processor.Invoke(instance, "throw", new TreeList<string>().AddBranchValue("oh no"));
            Assert.IsTrue(!result.IsValid);
            Assert.IsTrue(result.Value is ApplicationException);
        }

        [Test, ExpectedException(typeof(ApplicationException))] public void ExceptionIsThrown() {
            var instance = new TypedValue(new SampleClass());
            processor.InvokeWithThrow(instance, "throw", new TreeList<string>().AddBranchValue("oh no"));
        }

        [Test] public void OperatorIsRemoved() {
            processor.AddOperator(defaultTest);
            processor.AddOperator(specificTestA);
            object result = Execute("A");
            Assert.AreEqual("executeA", result.ToString());
            processor.RemoveOperator(specificTestA.GetType().FullName);
            result = Execute("A");
            Assert.AreEqual("defaultexecute", result.ToString());
        }

        [Test] public void EmptyMemoryContainsNothing() {
            Assert.IsFalse(processor.Configuration.GetItem<StringObjectMemory>().HasValue("anything"));
        }

        [Test] public void StoredDataIsLoaded() {
            processor.Configuration.GetItem<StringObjectMemory>().Save("something", "stuff");
            Assert.IsTrue(processor.Configuration.GetItem<StringObjectMemory>().HasValue("something"));
            Assert.AreEqual("stuff", processor.Configuration.GetItem<StringObjectMemory>().GetValue("something"));
        }

        private class DefaultTest: Operator<string, BasicProcessor>, InvokeOperator<string> {
            public bool CanInvoke(TypedValue instance, string member, Tree<string> parameters) {
                return true;
            }

            public TypedValue Invoke(TypedValue instance, string member, Tree<string> parameters) {
                return new TypedValue("defaultexecute");
            }
        }

        private class SpecificTest: Operator<string, BasicProcessor>, InvokeOperator<string> {
            private readonly string name;

            public SpecificTest(string name) {
                this.name = name;
            }

            public bool CanInvoke(TypedValue instance, string member, Tree<string> parameters) {
                return parameters.Value == name;
            }

            public TypedValue Invoke(TypedValue instance, string member, Tree<string> parameters) {
                return new TypedValue("execute" + name);
            }
        }

    }
}