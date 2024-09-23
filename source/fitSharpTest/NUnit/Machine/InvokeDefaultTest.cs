// Copyright © 2015 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Samples.Fit;
using fitSharp.Test.Double;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class InvokeDefaultTest {

        [SetUp] public void SetUp() {
            processor = Builder.CellProcessor();
        }

        [Test]
        public void MethodIsInvoked() {
            CheckInvokeMethod(new SampleClass(), "methodwithparms", "stuff", "samplestuff");
        }

        [Test]
        public void FirstOverloadedMethodIsInvoked() {
            CheckInvokeMethod(new SampleClass(), "overloadmethod", "stuff", "sample1stuff");
        }

        [Test]
        public void SecondOverloadedMethodIsInvoked() {
            processor.Get<Symbols>().SaveValue<int>("symbol", 3);
            CheckInvokeMethod(new SampleClass(), "overloadmethod", "<<symbol", "sample23");
        }

        [Test]
        public void MethodIsInvokedViaDomainAdapter() {
            CheckInvokeMethod(new SampleDomainAdapter(new SampleClass()), "methodwithparms", "stuff", "samplestuff");
        }

        [Test]
        public void MethodIsInvokedWithParameterName() {
          var result = processor.Invoke(new TypedValue(new SampleClass()), new MemberName("methodwithparms").WithNamedParameters(), new CellTree("input", "stuff"));
          ClassicAssert.AreEqual(typeof (string), result.Type);
          ClassicAssert.AreEqual("samplestuff", result.Value);
        }

        void CheckInvokeMethod(object instance, string methodName, string input, string resultValue) {
            var result = processor.Invoke(new TypedValue(instance), new MemberName(methodName), new CellTree(input));
            ClassicAssert.AreEqual(typeof (string), result.Type);
            ClassicAssert.AreEqual(resultValue, result.Value);
        }

        class SampleDomainAdapter: DomainAdapter {
            private readonly SampleClass sampleClass;

            public SampleDomainAdapter(SampleClass sampleClass) { this.sampleClass = sampleClass; }

            public object SystemUnderTest {
                get { return sampleClass; }
            }
        }

        CellProcessor processor;
    }
}