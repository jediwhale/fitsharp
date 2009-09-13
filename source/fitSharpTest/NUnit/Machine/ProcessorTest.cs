// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
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

        [Test] public void NoOperatorIsFound() {
            try {
                processor.Execute(new TreeList<string>());
                Assert.Fail();
            }
            catch (ApplicationException) {
                Assert.IsTrue(true);
            }
        }

        [Test] public void DefaultOperatorIsFound() {
            processor.AddOperator(defaultTest);
            object result = processor.Execute(new TreeList<string>()).Value;
            Assert.AreEqual("defaultexecute", result.ToString());
        }

        [Test] public void SpecificOperatorIsFound() {
            processor.AddOperator(defaultTest);
            processor.AddOperator(specificTestA);
            processor.AddOperator(specificTestB);
            object result = processor.Execute(new TreeLeaf<string>("A")).Value;
            Assert.AreEqual("executeA", result.ToString());
        }

        [Test] public void TypeIsCreated() {
            TypedValue result = processor.Create(typeof(SampleClass).FullName);
            Assert.IsTrue(result.Value is SampleClass);
        }

        [Test] public void MethodIsInvoked() {
            var instance = new TypedValue(new SampleClass());
            TypedValue result = processor.TryInvoke(instance, "methodnoparms", new TreeList<string>());
            Assert.AreEqual("samplereturn", result.Value);
        }

        [Test] public void MethodWithParameterIsInvoked() {
            var instance = new TypedValue(new SampleClass());
            TypedValue result = processor.TryInvoke(instance, "MethodWithParms", new TreeList<string>().AddBranchValue("stringparm0"));
            Assert.AreEqual("samplestringparm0", result.Value);
        }

        [Test] public void OperatorIsRemoved() {
            processor.AddOperator(defaultTest);
            processor.AddOperator(specificTestA);
            object result = processor.Execute(new TreeLeaf<string>("A")).Value;
            Assert.AreEqual("executeA", result.ToString());
            processor.RemoveOperator(specificTestA.GetType().FullName);
            result = processor.Execute(new TreeList<string>("A")).Value;
            Assert.AreEqual("defaultexecute", result.ToString());
        }

        [Test] public void EmptyMemoryContainsNothing() {
            processor.AddMemory<string>();
            Assert.IsFalse(processor.Contains("anything"));
        }

        [Test] public void StoredDataIsLoaded() {
            processor.AddMemory<KeyValueMemory<string, string>>();
            processor.Store(new KeyValueMemory<string, string>("something", "stuff"));
            Assert.IsTrue(processor.Contains(new KeyValueMemory<string, string>("something")));
            Assert.AreEqual("stuff", processor.Load(new KeyValueMemory<string, string>("something")).Instance);
        }

        [Test] public void ProcessorCopyHasIndependentOperators() {
            processor.AddOperator(new MemoryOperator());
            var copy = new BasicProcessor(processor);
            processor.AddMemory<KeyValueMemory<string, string>>();
            processor.Store(new KeyValueMemory<string, string>("something", "stuff"));
            copy.AddMemory<KeyValueMemory<string, string>>();
            copy.Store(new KeyValueMemory<string, string>("something", "else"));
            Assert.AreEqual("stuff", processor.Execute(null).Value);
            Assert.AreEqual("else", copy.Execute(null).Value);
        }

        private class MemoryOperator: Operator<BasicProcessor>, ExecuteOperator<string> {
            public bool CanExecute(TypedValue instance, Tree<string> parameters) {
                return true;
            }

            public TypedValue Execute(TypedValue instance, Tree<string> parameters) {
                return new TypedValue(Processor.Load(new KeyValueMemory<string, string>("something")).Instance);
            }
        }

        private class DefaultTest: Operator<BasicProcessor>, ExecuteOperator<string> {
            public bool CanExecute(TypedValue instance, Tree<string> parameters) {
                return true;
            }

            public TypedValue Execute(TypedValue instance, Tree<string> parameters) {
                return new TypedValue("defaultexecute");
            }
        }

        private class SpecificTest: Operator<BasicProcessor>, ExecuteOperator<string> {
            private readonly string name;

            public SpecificTest(string name) {
                this.name = name;
            }

            public bool CanExecute(TypedValue instance, Tree<string> parameters) {
                return parameters.Value == name;
            }

            public TypedValue Execute(TypedValue instance, Tree<string> parameters) {
                return new TypedValue("execute" + name);
            }
        }

    }
}