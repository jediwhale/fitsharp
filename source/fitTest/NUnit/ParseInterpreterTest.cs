// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Operators;
using fit.Test.Double;
using fitlibrary;
using fitSharp.Fit.Engine;
using fitSharp.Machine.Model;
using Moq;
using NUnit.Framework;
using TestStatus=fitSharp.Fit.Model.TestStatus;

namespace fit.Test.NUnit {
    [TestFixture] public class ParseInterpreterTest{

        private static SampleDomain sampleDomain;
        private static SampleDoFixture sampleDo;
        private static readonly TestStatus testStatus = new TestStatus();

        private Mock<CellProcessor> processor;

        [Test] public void FixtureIsCreated() {
            sampleDo = new SampleDoFixture();
            TypedValue result = Parse("<table><tr><td>sample do</td></tr></table>", "sample do", sampleDo);
            Assert.AreEqual(sampleDo, VerifyFixture<SampleDoFixture>(result));
        }

        private TypedValue Parse(string inputTables, string fixtureName, object fixtureObject) {
            return Parse(inputTables, TypedValue.Void, fixtureName, fixtureObject);
        }

        private TypedValue Parse(string inputTables, TypedValue target, string fixtureName, object fixtureObject) {
            processor = new Mock<CellProcessor>();
            processor.Setup(p => p.TestStatus).Returns(testStatus);
            processor.Setup(p => p.Create(fixtureName, It.IsAny<TreeList<Cell>>())).Returns(new TypedValue(fixtureObject));
            var parser = new ParseInterpreter { Processor = processor.Object };
            Parse table = fit.Parse.ParseFrom(inputTables);
            return parser.Parse(typeof(Interpreter), target, table.Parts);
        }

         private T VerifyFixture<T>(TypedValue result) where T: Fixture {
            var fixture = result.Value as T;
            Assert.IsNotNull(fixture);
            Assert.AreEqual(testStatus, fixture.TestStatus);
            Assert.AreEqual(processor.Object, fixture.Processor);
            return fixture;
        }

        [Test] public void FixtureWithArgumentsIsCreated() {
            sampleDo = new SampleDoFixture();
            TypedValue result = Parse("<table><tr><td>sample do</td><td>hi</td></tr></table>", "sample do", sampleDo);
            var fixture = VerifyFixture<SampleDoFixture>(result);
            Assert.AreEqual(1, fixture.ArgumentCount);
        }

        [Test] public void DomainClassIsWrappedInDoFixture() {
            sampleDomain = new SampleDomain();
            TypedValue result = Parse("<table><tr><td>sample domain</td></tr></table>", "sample domain", sampleDomain);
            var doFixture = VerifyFixture<DoFixture>(result);
            Assert.AreEqual(sampleDomain, doFixture.SystemUnderTest);
        }

        [Test] public void FixtureWithSUTIsCreated() {
            sampleDo = new SampleDoFixture();
            TypedValue result = Parse("<table><tr><td>sample do</td></tr></table>", new TypedValue("target"), "sample do", sampleDo);
            Assert.AreEqual(sampleDo, VerifyFixture<SampleDoFixture>(result));
            Assert.AreEqual("target", sampleDo.SystemUnderTest.ToString());
        }

    }
}