// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Test.Double;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class InvokeDefaultTest {
        private InvokeDefault<string, BasicProcessor> runtime;
        private readonly BasicProcessor processor = new BasicProcessor();

        [SetUp] public void SetUp() {
            runtime = new InvokeDefault<string, BasicProcessor> { Processor = processor};
        }

        [Test]
        public void MethodIsInvoked() {
            CheckInvokeMethod(new SampleClass());
        }

        [Test]
        public void MethodIsInvokedViaDomainAdapter() {
            CheckInvokeMethod(new SampleDomainAdapter(new SampleClass()));
        }

        [Test]
        public void MethodIsInvokedWithParameterName() {
          TypedValue result = runtime.Invoke(new TypedValue(new SampleClass()), "!methodwithparms", new TreeList<string>().AddBranchValue("input").AddBranchValue("stuff"));
          Assert.AreEqual(typeof (string), result.Type);
          Assert.AreEqual("samplestuff", result.Value);
        }

        private void CheckInvokeMethod(object instance) {
            TypedValue result = runtime.Invoke(new TypedValue(instance), "methodwithparms", new TreeList<string>().AddBranchValue("stuff"));
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