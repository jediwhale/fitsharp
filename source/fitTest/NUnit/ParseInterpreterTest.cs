// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Operators;
using fit.Test.Double;
using fitlibrary;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class ParseInterpreterTest{

        private static readonly SampleDomain sampleDomain = new SampleDomain();
        private static readonly SampleDoFixture sampleDo = new SampleDoFixture();
        private static readonly TestStatus testStatus = new TestStatus();

        private TestProcessor processor;

        [Test] public void FixtureIsCreated() {
            TypedValue result = Parse("<table><tr><td>sample do</td></tr></table>");
            VerifyFixture<SampleDoFixture>(result);
        }

        private TypedValue Parse(string inputTables) {
            processor = new TestProcessor();
            var parser = new ParseInterpreter { Processor = processor };
            Parse table = HtmlParser.Instance.Parse(inputTables);
            return parser.Parse(typeof(Interpreter), TypedValue.Void, table);
        }

         private T VerifyFixture<T>(TypedValue result) where T: Fixture {
            var fixture = result.Value as T;
            Assert.IsNotNull(fixture);
            Assert.AreEqual(testStatus, fixture.TestStatus);
            Assert.AreEqual(processor, fixture.Processor);
            return fixture;
        }

        [Test] public void FixtureWithArgumentsIsCreated() {
            TypedValue result = Parse("<table><tr><td>sample do</td><td>hi</td></tr></table>");
            var fixture = VerifyFixture<SampleDoFixture>(result);
            Assert.AreEqual(1, fixture.ArgumentCount);
        }

        [Test] public void DomainClassIsWrappedInDoFixture() {
            TypedValue result = Parse("<table><tr><td>sample domain</td></tr></table>");
            var doFixture = VerifyFixture<DoFixture>(result);
            Assert.AreEqual(sampleDomain, doFixture.SystemUnderTest);
        }

        private class TestProcessor: CellProcessor {
            public ApplicationUnderTest ApplicationUnderTest {
                get { throw new NotImplementedException(); }
            }

            public void AddNamespace(string namespaceName) {
                throw new NotImplementedException();
            }

            public void AddOperator(string operatorName) {
                throw new NotImplementedException();
            }

            public TypedValue Create(string memberName, Tree<Cell> parameters) {
                if (memberName == "sample domain") return new TypedValue(sampleDomain);
                if (memberName == "sample do") return new TypedValue(sampleDo);
                throw new NotImplementedException();
            }

            public bool Compare(TypedValue instance, Tree<Cell> parameters) {
                throw new NotImplementedException();
            }

            public Tree<Cell> Compose(TypedValue instance) {
                throw new NotImplementedException();
            }

            public bool Contains<V>(V matchItem) {
                throw new NotImplementedException();
            }

            public TypedValue Execute(TypedValue instance, Tree<Cell> parameters) {
                throw new NotImplementedException();
            }

            public TypedValue Invoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
                throw new NotImplementedException();
            }

            public V Load<V>(V matchItem) {
                throw new NotImplementedException();
            }

            public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
                throw new NotImplementedException();
            }

            public void RemoveOperator(string operatorName) {
                throw new NotImplementedException();
            }

            public void Store<V>(V newItem) {
                throw new NotImplementedException();
            }

            public TestStatus TestStatus {
                get { return testStatus; }
                set { throw new NotImplementedException(); }
            }
        }
    }
}