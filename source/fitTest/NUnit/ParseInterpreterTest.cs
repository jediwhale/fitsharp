// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Reflection;
using fit.Test.Double;
using fitlibrary;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class ParseInterpreterTest{

        private CellProcessor processor;

        [Test] public void FixtureIsCreated() {
            TypedValue result = Parse("<table><tr><td>sample do</td></tr></table>");
            VerifyResult<SampleDoFixture>(result);
        }

        private TypedValue Parse(string inputTables) {
            return Parse(inputTables, TypedValue.Void);
        }

        private TypedValue Parse(string inputTables, TypedValue target) {
            processor = new Service.Service();
            processor.ApplicationUnderTest.AddAssembly(Assembly.GetExecutingAssembly().CodeBase);
            processor.ApplicationUnderTest.AddNamespace(typeof(SampleDomain).Namespace);
            var parser = new ParseInterpreter { Processor = processor };
            Parse table = fit.Parse.ParseFrom(inputTables);
            return parser.Parse(typeof(Interpreter), target, table.Parts);
        }

         private static T VerifyResult<T>(TypedValue result) where T: class {
            var anObject = result.Value as T;
            Assert.IsNotNull(anObject);
            return anObject;
        }

        [Test] public void DomainClassIsWrappedInDoFixture() {
            TypedValue result = Parse("<table><tr><td>sample domain</td></tr></table>");
            var doFixture = VerifyResult<DoFixture>(result);
            Assert.IsNotNull(doFixture.SystemUnderTest as SampleDomain);
        }

        [Test] public void FixtureWithSUTIsCreated() {
            TypedValue result = Parse("<table><tr><td>sample do</td></tr></table>", new TypedValue("target"));
            var sampleDo = VerifyResult<SampleDoFixture>(result);
            Assert.AreEqual("target", sampleDo.SystemUnderTest.ToString());
        }

    }
}