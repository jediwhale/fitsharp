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
        private static readonly TestStatus testStatus = new TestStatus();

        [Test] public void DomainClassIsWrappedInDoFixture() {
            var processor = new TestProcessor();
            var parser = new ParseInterpreter { Processor = processor };
            Parse table = HtmlParser.Instance.Parse("<table><tr><td>sample domain</td></tr></table>");
            TypedValue result = parser.Parse(typeof(Interpreter), TypedValue.Void, table);
            var doFixture = result.Value as DoFixture;
            Assert.IsNotNull(doFixture);
            Assert.AreEqual(sampleDomain, doFixture.SystemUnderTest);
            Assert.AreEqual(testStatus, doFixture.TestStatus);
            Assert.AreEqual(processor, doFixture.Processor);
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