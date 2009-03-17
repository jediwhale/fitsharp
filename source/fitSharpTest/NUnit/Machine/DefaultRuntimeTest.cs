// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class DefaultRuntimeTest {
        private DefaultRuntime<string> runtime;
        private readonly BasicProcessor processor = new BasicProcessor();

        [SetUp] public void SetUp() {
            runtime = new DefaultRuntime<string>();
        }

        [Test] public void InstanceIsCreated() {
            TypedValue result = TypedValue.Void;
            runtime.TryCreate(processor, typeof(SampleClass).FullName, new TreeList<string>(), ref result);
            Assert.IsTrue(result.Value is SampleClass);
        }

        [Test] public void StandardInstanceIsCreated() {
            TypedValue result = TypedValue.Void;
            runtime.TryCreate(processor, "System.Boolean", new TreeList<string>(), ref result);
            Assert.IsTrue(result.Value is bool);
        }

        [Test]
        public void MethodIsInvoked() {
            CheckInvokeMethod(new SampleClass());
        }

        [Test]
        public void MethodIsInvokedViaDomainAdapter() {
            CheckInvokeMethod(new SampleDomainAdapter(new SampleClass()));
        }

        private void CheckInvokeMethod(object instance) {
            var result = new TypedValue();
            runtime.TryInvoke(processor, new TypedValue(instance), "methodwithparms", new TreeList<string>().AddBranchValue("stuff"), ref result);
            Assert.AreEqual(typeof (string), result.Type);
            Assert.AreEqual("samplestuff", result.Value);
        }

        private class SampleDomainAdapter: DomainAdapter {
            private readonly SampleClass sampleClass;

            public SampleDomainAdapter(SampleClass sampleClass) { this.sampleClass = sampleClass; }

            public object SystemUnderTest {
                get { return sampleClass; }
            }
        }
    }
}